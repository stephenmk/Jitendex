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
using Jitendex.MinimalJsonDiff;
using Jitendex.Kanjidic2.Import.Models;
using Jitendex.Kanjidic2.Import.Tables;
using Jitendex.Kanjidic2.Import.Tables.Groups;
using Jitendex.Kanjidic2.Import.Tables.GroupElements;
using Jitendex.Kanjidic2.Import.Tables.SubgroupElements;

namespace Jitendex.Kanjidic2.Import;

internal sealed class Database(ILogger<Database> logger, Kanjidic2Context context)
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

    public void Initialize(Document document)
    {
        logger.LogInformation("Initializing database with data from {Date:yyyy-MM-dd}", document.Header.Date);

        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        FileHeaderTable.InsertItem(context, document.Header);
        SequenceTable.InsertItems(context, document.GetSequences());

        CodepointTypeTable.InsertItems(context, document.CodepointTypes.Values);
        DictionaryTypeTable.InsertItems(context, document.DictionaryTypes.Values);
        QueryCodeTypeTable.InsertItems(context, document.QueryCodeTypes.Values);
        MisclassificationTypeTable.InsertItems(context, document.MisclassificationTypes.Values);
        RadicalTypeTable.InsertItems(context, document.RadicalTypes.Values);
        ReadingTypeTable.InsertItems(context, document.ReadingTypes.Values);
        VariantTypeTable.InsertItems(context, document.VariantTypes.Values);

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
    }

    public void Update(DocumentDiff diff)
    {
        logger.LogInformation("Updating {Count} entries with data from {Date:yyyy-MM-dd}", diff.SequenceIds.Count, diff.FileHeader.Date);

        var aSequences = DtoMapper.LoadRevisionlessSequences(context, diff.SequenceIds);

        context.ExecuteDeferForeignKeysPragma();

        FileHeaderTable.InsertItem(context, diff.InsertDocument.Header);
        SequenceTable.InsertOrIgnoreItems(context, diff.InsertDocument.GetSequences());

        CodepointTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.CodepointTypes.Values);
        DictionaryTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.DictionaryTypes.Values);
        QueryCodeTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.QueryCodeTypes.Values);
        MisclassificationTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.MisclassificationTypes.Values);
        RadicalTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.RadicalTypes.Values);
        ReadingTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.ReadingTypes.Values);
        VariantTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.VariantTypes.Values);

        EntryTable.InsertItems(context, diff.InsertDocument.Entries.Values);
        CodepointGroupTable.InsertItems(context, diff.InsertDocument.CodepointGroups.Values);
        DictionaryGroupTable.InsertItems(context, diff.InsertDocument.DictionaryGroups.Values);
        MiscGroupTable.InsertItems(context, diff.InsertDocument.MiscGroups.Values);
        QueryCodeGroupTable.InsertItems(context, diff.InsertDocument.QueryCodeGroups.Values);
        RadicalGroupTable.InsertItems(context, diff.InsertDocument.RadicalGroups.Values);
        ReadingMeaningGroupTable.InsertItems(context, diff.InsertDocument.ReadingMeaningGroups.Values);
        CodepointTable.InsertItems(context, diff.InsertDocument.Codepoints.Values);
        DictionaryTable.InsertItems(context, diff.InsertDocument.Dictionaries.Values);
        NanoriTable.InsertItems(context, diff.InsertDocument.Nanoris.Values);
        QueryCodeTable.InsertItems(context, diff.InsertDocument.QueryCodes.Values);
        RadicalTable.InsertItems(context, diff.InsertDocument.Radicals.Values);
        RadicalNameTable.InsertItems(context, diff.InsertDocument.RadicalNames.Values);
        ReadingMeaningTable.InsertItems(context, diff.InsertDocument.ReadingMeanings.Values);
        StrokeCountTable.InsertItems(context, diff.InsertDocument.StrokeCounts.Values);
        VariantTable.InsertItems(context, diff.InsertDocument.Variants.Values);
        MeaningTable.InsertItems(context, diff.InsertDocument.Meanings.Values);
        ReadingTable.InsertItems(context, diff.InsertDocument.Readings.Values);

        EntryTable.UpdateItems(context, diff.UpdateDocument.Entries.Values);
        CodepointGroupTable.UpdateItems(context, diff.UpdateDocument.CodepointGroups.Values);
        DictionaryGroupTable.UpdateItems(context, diff.UpdateDocument.DictionaryGroups.Values);
        MiscGroupTable.UpdateItems(context, diff.UpdateDocument.MiscGroups.Values);
        QueryCodeGroupTable.UpdateItems(context, diff.UpdateDocument.QueryCodeGroups.Values);
        RadicalGroupTable.UpdateItems(context, diff.UpdateDocument.RadicalGroups.Values);
        ReadingMeaningGroupTable.UpdateItems(context, diff.UpdateDocument.ReadingMeaningGroups.Values);
        CodepointTable.UpdateItems(context, diff.UpdateDocument.Codepoints.Values);
        DictionaryTable.UpdateItems(context, diff.UpdateDocument.Dictionaries.Values);
        NanoriTable.UpdateItems(context, diff.UpdateDocument.Nanoris.Values);
        QueryCodeTable.UpdateItems(context, diff.UpdateDocument.QueryCodes.Values);
        RadicalTable.UpdateItems(context, diff.UpdateDocument.Radicals.Values);
        RadicalNameTable.UpdateItems(context, diff.UpdateDocument.RadicalNames.Values);
        ReadingMeaningTable.UpdateItems(context, diff.UpdateDocument.ReadingMeanings.Values);
        StrokeCountTable.UpdateItems(context, diff.UpdateDocument.StrokeCounts.Values);
        VariantTable.UpdateItems(context, diff.UpdateDocument.Variants.Values);
        MeaningTable.UpdateItems(context, diff.UpdateDocument.Meanings.Values);
        ReadingTable.UpdateItems(context, diff.UpdateDocument.Readings.Values);

        ReadingTable.DeleteItems(context, diff.DeleteDocument.Readings.Values);
        MeaningTable.DeleteItems(context, diff.DeleteDocument.Meanings.Values);
        VariantTable.DeleteItems(context, diff.DeleteDocument.Variants.Values);
        StrokeCountTable.DeleteItems(context, diff.DeleteDocument.StrokeCounts.Values);
        ReadingMeaningTable.DeleteItems(context, diff.DeleteDocument.ReadingMeanings.Values);
        RadicalNameTable.DeleteItems(context, diff.DeleteDocument.RadicalNames.Values);
        RadicalTable.DeleteItems(context, diff.DeleteDocument.Radicals.Values);
        QueryCodeTable.DeleteItems(context, diff.DeleteDocument.QueryCodes.Values);
        NanoriTable.DeleteItems(context, diff.DeleteDocument.Nanoris.Values);
        DictionaryTable.DeleteItems(context, diff.DeleteDocument.Dictionaries.Values);
        CodepointTable.DeleteItems(context, diff.DeleteDocument.Codepoints.Values);
        ReadingMeaningGroupTable.DeleteItems(context, diff.DeleteDocument.ReadingMeaningGroups.Values);
        RadicalGroupTable.DeleteItems(context, diff.DeleteDocument.RadicalGroups.Values);
        QueryCodeGroupTable.DeleteItems(context, diff.DeleteDocument.QueryCodeGroups.Values);
        MiscGroupTable.DeleteItems(context, diff.DeleteDocument.MiscGroups.Values);
        DictionaryGroupTable.DeleteItems(context, diff.DeleteDocument.DictionaryGroups.Values);
        CodepointGroupTable.DeleteItems(context, diff.DeleteDocument.CodepointGroups.Values);
        EntryTable.DeleteItems(context, diff.DeleteDocument.Entries.Values);

        var bSequences = DtoMapper.LoadRevisionlessSequences(context, diff.SequenceIds);

        var sequences = context.Sequences
            .Where(sequence => diff.SequenceIds.Contains(sequence.Id))
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
                    CreatedDate: diff.FileHeader.Date,
                    DiffJson: baDiff
                ));
            }
        }

        RevisionTable.InsertItems(context, revisions);
    }
}
