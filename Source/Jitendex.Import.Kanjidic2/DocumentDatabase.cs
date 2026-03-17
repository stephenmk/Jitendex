/*
Copyright (c) 2025-2026 Stephen Kraus
SPDX-License-Identifier: AGPL-3.0-or-later

This file is part of Jitendex.

Jitendex is free software: you can redistribute it and/or modify it under the terms of
the GNU Affero General Public License as published by the Free Software Foundation,
either version 3 of the License or (at your option) any later version.

Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with Jitendex.
If not, see <https://www.gnu.org/licenses/>.
*/

using Microsoft.Extensions.Logging;
using Jitendex.Data.Kanjidic2;
using Jitendex.MinimalJsonDiff;
using Jitendex.Import.Kanjidic2.Models;
using Jitendex.Import.Kanjidic2.Tables;
using Jitendex.Import.Kanjidic2.Tables.Groups;
using Jitendex.Import.Kanjidic2.Tables.GroupElements;
using Jitendex.Import.Kanjidic2.Tables.SubgroupElements;

namespace Jitendex.Import.Kanjidic2;

internal sealed class DocumentDatabase(ILogger<DocumentDatabase> logger, Kanjidic2Context context)
    : IDocumentDatabase<DateOnly, Document, DocumentDiff>
{
    private static readonly FileHeaderTable FileHeaderTable = new();
    private static readonly RevisionTable RevisionTable = new();
    private static readonly SequenceTable SequenceTable = new();
    private static readonly EntryTable EntryTable = new();

    #region Group Tables
    private static readonly CodepointGroupTable CodepointGroupTable = new();
    private static readonly DictionaryGroupTable DictionaryGroupTable = new();
    private static readonly MiscGroupTable MiscGroupTable = new();
    private static readonly QueryCodeGroupTable QueryCodeGroupTable = new();
    private static readonly RadicalGroupTable RadicalGroupTable = new();
    private static readonly ReadingMeaningGroupTable ReadingMeaningGroupTable = new();
    #endregion

    #region Group Element Tables
    private static readonly CodepointTable CodepointTable = new();
    private static readonly DictionaryTable DictionaryTable = new();
    private static readonly NanoriTable NanoriTable = new();
    private static readonly QueryCodeTable QueryCodeTable = new();
    private static readonly RadicalTable RadicalTable = new();
    private static readonly RadicalNameTable RadicalNameTable = new();
    private static readonly ReadingMeaningTable ReadingMeaningTable = new();
    private static readonly StrokeCountTable StrokeCountTable = new();
    private static readonly VariantTable VariantTable = new();
    #endregion

    #region Subgroup Element Tables
    private static readonly MeaningTable MeaningTable = new();
    private static readonly ReadingTable ReadingTable = new();
    #endregion

    #region Keyword tables
    private static readonly KeywordTable<CodepointTypeElement> CodepointTypeTable = new();
    private static readonly KeywordTable<DictionaryTypeElement> DictionaryTypeTable = new();
    private static readonly KeywordTable<QueryCodeTypeElement> QueryCodeTypeTable = new();
    private static readonly KeywordTable<MisclassificationTypeElement> MisclassificationTypeTable = new();
    private static readonly KeywordTable<RadicalTypeElement> RadicalTypeTable = new();
    private static readonly KeywordTable<ReadingTypeElement> ReadingTypeTable = new();
    private static readonly KeywordTable<VariantTypeElement> VariantTypeTable = new();
    #endregion

    public void EnsureCreated()
        => context.Database.EnsureCreated();

    public DateOnly? GetLastKey()
        => context.FileHeaders
            .OrderByDescending(static x => x.Id)
            .Take(1)
            .Select(static x => (DateOnly?)x.Date)
            .FirstOrDefault();

    public void Initialize(Document document)
    {
        logger.LogInformation("Initializing database with data from {Date:yyyy-MM-dd}", document.ArchiveKey);

        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        FileHeaderTable.InsertItem(context, new(document.ArchiveKey));
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertItems(context, document.GetSequences(fileHeaderId));

        CodepointTypeTable.InsertItems(context, document.GetCodepointTypes(fileHeaderId));
        DictionaryTypeTable.InsertItems(context, document.GetDictionaryTypes(fileHeaderId));
        QueryCodeTypeTable.InsertItems(context, document.GetQueryCodeTypes(fileHeaderId));
        MisclassificationTypeTable.InsertItems(context, document.GetMisclassificationTypes(fileHeaderId));
        RadicalTypeTable.InsertItems(context, document.GetRadicalTypes(fileHeaderId));
        ReadingTypeTable.InsertItems(context, document.GetReadingTypes(fileHeaderId));
        VariantTypeTable.InsertItems(context, document.GetVariantTypes(fileHeaderId));

        EntryTable.InsertItems(context, document.Entries.Values);

        CodepointGroupTable.InsertItems(context, document.CodepointGroups.Values);
        DictionaryGroupTable.InsertItems(context, document.DictionaryGroups.Values);
        MiscGroupTable.InsertItems(context, document.MiscGroups.Values);
        QueryCodeGroupTable.InsertItems(context, document.QueryCodeGroups.Values);
        RadicalGroupTable.InsertItems(context, document.RadicalGroups.Values);
        ReadingMeaningGroupTable.InsertItems(context, document.ReadingMeaningGroups.Values);

        CodepointTable.InsertItems(context, document.Codepoints.Values);
        DictionaryTable.InsertItems(context, document.Dictionaries.Values);
        NanoriTable.InsertItems(context, document.Nanoris.Values);
        QueryCodeTable.InsertItems(context, document.QueryCodes.Values);
        RadicalTable.InsertItems(context, document.Radicals.Values);
        RadicalNameTable.InsertItems(context, document.RadicalNames.Values);
        ReadingMeaningTable.InsertItems(context, document.ReadingMeanings.Values);
        StrokeCountTable.InsertItems(context, document.StrokeCounts.Values);
        VariantTable.InsertItems(context, document.Variants.Values);

        MeaningTable.InsertItems(context, document.Meanings.Values);
        ReadingTable.InsertItems(context, document.Readings.Values);

        transaction.Commit();
        context.ExecuteVacuum();
    }

    public void Update(DocumentDiff diff)
    {
        var sequenceIds = diff.SequenceIds();

        logger.LogInformation("Updating {Count} entries with data from {Date:yyyy-MM-dd}", sequenceIds.Count, diff.ArchiveKey);

        using var transaction = context.Database.BeginTransaction();
        var aSequences = DtoMapper.LoadRevisionlessSequences(context, sequenceIds);

        context.ExecuteDeferForeignKeysPragma();

        FileHeaderTable.InsertItem(context, new(diff.ArchiveKey));
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertOrIgnoreItems(context, diff.Upserts.GetSequences(fileHeaderId));

        CodepointTypeTable.InsertOrIgnoreItems(context, diff.Upserts.GetCodepointTypes(fileHeaderId));
        DictionaryTypeTable.InsertOrIgnoreItems(context, diff.Upserts.GetDictionaryTypes(fileHeaderId));
        QueryCodeTypeTable.InsertOrIgnoreItems(context, diff.Upserts.GetQueryCodeTypes(fileHeaderId));
        MisclassificationTypeTable.InsertOrIgnoreItems(context, diff.Upserts.GetMisclassificationTypes(fileHeaderId));
        RadicalTypeTable.InsertOrIgnoreItems(context, diff.Upserts.GetRadicalTypes(fileHeaderId));
        ReadingTypeTable.InsertOrIgnoreItems(context, diff.Upserts.GetReadingTypes(fileHeaderId));
        VariantTypeTable.InsertOrIgnoreItems(context, diff.Upserts.GetVariantTypes(fileHeaderId));

        EntryTable.UpsertItems(context, diff.Upserts.Entries.Values);
        CodepointGroupTable.UpsertItems(context, diff.Upserts.CodepointGroups.Values);
        DictionaryGroupTable.UpsertItems(context, diff.Upserts.DictionaryGroups.Values);
        MiscGroupTable.UpsertItems(context, diff.Upserts.MiscGroups.Values);
        QueryCodeGroupTable.UpsertItems(context, diff.Upserts.QueryCodeGroups.Values);
        RadicalGroupTable.UpsertItems(context, diff.Upserts.RadicalGroups.Values);
        ReadingMeaningGroupTable.UpsertItems(context, diff.Upserts.ReadingMeaningGroups.Values);
        CodepointTable.UpsertItems(context, diff.Upserts.Codepoints.Values);
        DictionaryTable.UpsertItems(context, diff.Upserts.Dictionaries.Values);
        NanoriTable.UpsertItems(context, diff.Upserts.Nanoris.Values);
        QueryCodeTable.UpsertItems(context, diff.Upserts.QueryCodes.Values);
        RadicalTable.UpsertItems(context, diff.Upserts.Radicals.Values);
        RadicalNameTable.UpsertItems(context, diff.Upserts.RadicalNames.Values);
        ReadingMeaningTable.UpsertItems(context, diff.Upserts.ReadingMeanings.Values);
        StrokeCountTable.UpsertItems(context, diff.Upserts.StrokeCounts.Values);
        VariantTable.UpsertItems(context, diff.Upserts.Variants.Values);
        MeaningTable.UpsertItems(context, diff.Upserts.Meanings.Values);
        ReadingTable.UpsertItems(context, diff.Upserts.Readings.Values);

        ReadingTable.DeleteItems(context, diff.Deletes.Readings.Values);
        MeaningTable.DeleteItems(context, diff.Deletes.Meanings.Values);
        VariantTable.DeleteItems(context, diff.Deletes.Variants.Values);
        StrokeCountTable.DeleteItems(context, diff.Deletes.StrokeCounts.Values);
        ReadingMeaningTable.DeleteItems(context, diff.Deletes.ReadingMeanings.Values);
        RadicalNameTable.DeleteItems(context, diff.Deletes.RadicalNames.Values);
        RadicalTable.DeleteItems(context, diff.Deletes.Radicals.Values);
        QueryCodeTable.DeleteItems(context, diff.Deletes.QueryCodes.Values);
        NanoriTable.DeleteItems(context, diff.Deletes.Nanoris.Values);
        DictionaryTable.DeleteItems(context, diff.Deletes.Dictionaries.Values);
        CodepointTable.DeleteItems(context, diff.Deletes.Codepoints.Values);
        ReadingMeaningGroupTable.DeleteItems(context, diff.Deletes.ReadingMeaningGroups.Values);
        RadicalGroupTable.DeleteItems(context, diff.Deletes.RadicalGroups.Values);
        QueryCodeGroupTable.DeleteItems(context, diff.Deletes.QueryCodeGroups.Values);
        MiscGroupTable.DeleteItems(context, diff.Deletes.MiscGroups.Values);
        DictionaryGroupTable.DeleteItems(context, diff.Deletes.DictionaryGroups.Values);
        CodepointGroupTable.DeleteItems(context, diff.Deletes.CodepointGroups.Values);
        EntryTable.DeleteItems(context, diff.Deletes.Entries.Values);

        var bSequences = DtoMapper.LoadRevisionlessSequences(context, sequenceIds);

        var sequences = context.Sequences
            .Where(sequence => sequenceIds.Contains(sequence.Id))
            .Select(static seq => new
            {
                seq.Id,
                RevisionCount = seq.Revisions.Count,
            });

        var revisions = new List<DocumentRevision>(aSequences.Count);

        foreach (var seq in sequences)
        {
            if (aSequences.TryGetValue(seq.Id, out var aSeq))
            {
                var bSeq = bSequences[seq.Id];
                var baDiff = JsonDiffer.Diff(a: bSeq, b: aSeq);
                revisions.Add(new(
                    SequenceId: seq.Id,
                    Number: seq.RevisionCount,
                    FileHeaderId: fileHeaderId,
                    DiffJson: baDiff
                ));
            }
        }

        RevisionTable.InsertItems(context, revisions);
        transaction.Commit();
    }
}
