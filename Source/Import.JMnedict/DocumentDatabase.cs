// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DocumentDatabase.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using Jitendex.Data.JMnedict;
using Jitendex.Import.JMnedict.TableRows;
using Jitendex.Import.JMnedict.Tables;
using Jitendex.Import.JMnedict.Tables.EntryElements;
using Jitendex.Import.JMnedict.Tables.EntryElements.KanjiFormElements;
using Jitendex.Import.JMnedict.Tables.EntryElements.ReadingElements;
using Jitendex.Import.JMnedict.Tables.EntryElements.TranslationElements;
using Jitendex.MinimalJsonDiff;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMnedict;

internal sealed class DocumentDatabase(ILogger<DocumentDatabase> logger, JMnedictContext context)
    : IDocumentDatabase<DateOnly, Document, DocumentDiff>
{
    private static readonly FileHeaderTable FileHeaderTable = new();
    private static readonly RevisionTable RevisionTable = new();
    private static readonly SequenceTable SequenceTable = new();
    private static readonly EntryTable EntryTable = new();

    #region Entry Element Tables
    private static readonly KanjiFormTable KanjiFormTable = new();
    private static readonly ReadingTable ReadingTable = new();
    private static readonly TranslationTable TranslationTable = new();
    #endregion

    #region Kanji Form Element Tables
    private static readonly KanjiFormInfoTable KanjiFormInfoTable = new();
    private static readonly KanjiFormPriorityTable KanjiFormPriorityTable = new();
    #endregion

    #region Reading Element Tables
    private static readonly ReadingInfoTable ReadingInfoTable = new();
    private static readonly ReadingPriorityTable ReadingPriorityTable = new();
    private static readonly RestrictionTable RestrictionTable = new();
    #endregion

    #region Translation Element Tables
    private static readonly CrossReferenceTable CrossReferenceTable = new();
    private static readonly DetailTable DetailTable = new();
    private static readonly NameTypeTable NameTypeTable = new();
    #endregion

    #region Keyword Tables
    private static readonly KeywordTable<PriorityTagRow> PriorityTagTable = new();
    private static readonly KeywordTable<ReadingInfoTagRow> ReadingInfoTagTable = new();
    private static readonly KeywordTable<KanjiFormInfoTagRow> KanjiFormInfoTagTable = new();
    private static readonly KeywordTable<NameTypeTagRow> NameTypeTagTable = new();
    private static readonly KeywordTable<DetailLanguageRow> DetailLanguageTable = new();
    #endregion

    public void EnsureCreated()
        => context.Database.EnsureCreated();

    public DateOnly? GetLastKey()
        => context.FileHeaders.Max(static x => (DateOnly?)x.Date);

    public void Initialize(Document document)
    {
        logger.LogInformation("Initializing database with data from {Date:yyyy-MM-dd}", document.ArchiveKey);

        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        FileHeaderTable.InsertItem(context, new(document.ArchiveKey));
        var fileHeaderId = (int)context.GetLastInsertRowId();

        #pragma warning disable format

        SequenceTable         .InsertItems(context, document.GetSequences(fileHeaderId));
        PriorityTagTable      .InsertItems(context, document.GetPriorityTags(fileHeaderId));
        ReadingInfoTagTable   .InsertItems(context, document.GetReadingInfoTags(fileHeaderId));
        KanjiFormInfoTagTable .InsertItems(context, document.GetKanjiFormInfoTags(fileHeaderId));
        NameTypeTagTable      .InsertItems(context, document.GetNameTypeTags(fileHeaderId));
        DetailLanguageTable   .InsertItems(context, document.GetDetailLanguages(fileHeaderId));

        EntryTable            .InsertItems(context, document.Entries.Values);
        KanjiFormTable        .InsertItems(context, document.KanjiForms.Values);
        ReadingTable          .InsertItems(context, document.Readings.Values);
        TranslationTable      .InsertItems(context, document.Translations.Values);
        KanjiFormInfoTable    .InsertItems(context, document.KanjiFormInfos.Values);
        KanjiFormPriorityTable.InsertItems(context, document.KanjiFormPriorities.Values);
        ReadingInfoTable      .InsertItems(context, document.ReadingInfos.Values);
        ReadingPriorityTable  .InsertItems(context, document.ReadingPriorities.Values);
        RestrictionTable      .InsertItems(context, document.Restrictions.Values);
        CrossReferenceTable   .InsertItems(context, document.CrossReferences.Values);
        DetailTable           .InsertItems(context, document.Details.Values);
        NameTypeTable         .InsertItems(context, document.NameTypes.Values);

        #pragma warning restore format

        transaction.Commit();
        context.ExecuteVacuum();
    }

    public void Update(DocumentDiff diff)
    {
        var sequenceIds = diff.SequenceIds();

        logger.LogInformation("Updating {Count} entries with data from {Date:yyyy-MM-dd}", sequenceIds.Count, diff.ArchiveKey);

        using var transaction = context.Database.BeginTransaction();

        var aSequences = DtoMapper.LoadSequencesWithoutRevisions(context, sequenceIds);

        FileHeaderTable.InsertItem(context, new(diff.ArchiveKey));
        var fileHeaderId = (int)context.GetLastInsertRowId();

        #pragma warning disable format

        SequenceTable         .InsertOrIgnoreItems(context, diff.Upserts.GetSequences(fileHeaderId));
        PriorityTagTable      .InsertOrIgnoreItems(context, diff.Upserts.GetPriorityTags(fileHeaderId));
        ReadingInfoTagTable   .InsertOrIgnoreItems(context, diff.Upserts.GetReadingInfoTags(fileHeaderId));
        KanjiFormInfoTagTable .InsertOrIgnoreItems(context, diff.Upserts.GetKanjiFormInfoTags(fileHeaderId));
        NameTypeTagTable      .InsertOrIgnoreItems(context, diff.Upserts.GetNameTypeTags(fileHeaderId));
        DetailLanguageTable   .InsertOrIgnoreItems(context, diff.Upserts.GetDetailLanguages(fileHeaderId));

        EntryTable            .UpsertItems(context, diff.Upserts.Entries.Values);
        KanjiFormTable        .UpsertItems(context, diff.Upserts.KanjiForms.Values);
        ReadingTable          .UpsertItems(context, diff.Upserts.Readings.Values);
        TranslationTable      .UpsertItems(context, diff.Upserts.Translations.Values);
        KanjiFormInfoTable    .UpsertItems(context, diff.Upserts.KanjiFormInfos.Values);
        KanjiFormPriorityTable.UpsertItems(context, diff.Upserts.KanjiFormPriorities.Values);
        ReadingInfoTable      .UpsertItems(context, diff.Upserts.ReadingInfos.Values);
        ReadingPriorityTable  .UpsertItems(context, diff.Upserts.ReadingPriorities.Values);
        RestrictionTable      .UpsertItems(context, diff.Upserts.Restrictions.Values);
        CrossReferenceTable   .UpsertItems(context, diff.Upserts.CrossReferences.Values);
        DetailTable           .UpsertItems(context, diff.Upserts.Details.Values);
        NameTypeTable         .UpsertItems(context, diff.Upserts.NameTypes.Values);

        NameTypeTable         .DeleteItems(context, diff.Deletes.NameTypes.Values);
        DetailTable           .DeleteItems(context, diff.Deletes.Details.Values);
        CrossReferenceTable   .DeleteItems(context, diff.Deletes.CrossReferences.Values);
        RestrictionTable      .DeleteItems(context, diff.Deletes.Restrictions.Values);
        ReadingPriorityTable  .DeleteItems(context, diff.Deletes.ReadingPriorities.Values);
        ReadingInfoTable      .DeleteItems(context, diff.Deletes.ReadingInfos.Values);
        KanjiFormPriorityTable.DeleteItems(context, diff.Deletes.KanjiFormPriorities.Values);
        KanjiFormInfoTable    .DeleteItems(context, diff.Deletes.KanjiFormInfos.Values);
        TranslationTable      .DeleteItems(context, diff.Deletes.Translations.Values);
        ReadingTable          .DeleteItems(context, diff.Deletes.Readings.Values);
        KanjiFormTable        .DeleteItems(context, diff.Deletes.KanjiForms.Values);
        EntryTable            .DeleteItems(context, diff.Deletes.Entries.Values);

        #pragma warning restore format

        var bSequences = DtoMapper.LoadSequencesWithoutRevisions(context, sequenceIds);
        var revisions = new List<DocumentRevision>(aSequences.Count);

        foreach (var id in sequenceIds)
        {
            if (aSequences.TryGetValue(id, out var aSeq))
            {
                var bSeq = bSequences[id];
                var baDiff = JsonDiffer.DiffToUtf8Bytes(a: bSeq, b: aSeq);
                revisions.Add(new(
                    SequenceId: id,
                    FileHeaderId: fileHeaderId,
                    DiffJson: baDiff
                ));
            }
        }

        RevisionTable.InsertItems(context, revisions);
        transaction.Commit();
    }
}
