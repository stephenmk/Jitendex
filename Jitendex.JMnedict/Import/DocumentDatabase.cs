/*
Copyright (c) 2026 Stephen Kraus
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
using Jitendex.Import;
using Jitendex.JMnedict.Import.Models;
using Jitendex.JMnedict.Import.Tables;
using Jitendex.JMnedict.Import.Tables.EntryElements;
using Jitendex.JMnedict.Import.Tables.EntryElements.KanjiFormElements;
using Jitendex.JMnedict.Import.Tables.EntryElements.ReadingElements;
using Jitendex.JMnedict.Import.Tables.EntryElements.TranslationElements;

namespace Jitendex.JMnedict.Import;

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
    private static readonly KeywordTable<PriorityTagElement> PriorityTagTable = new();
    private static readonly KeywordTable<ReadingInfoTagElement> ReadingInfoTagTable = new();
    private static readonly KeywordTable<KanjiFormInfoTagElement> KanjiFormInfoTagTable = new();
    private static readonly KeywordTable<NameTypeTagElement> NameTypeTagTable = new();
    private static readonly KeywordTable<DetailLanguageElement> DetailLanguageTable = new();
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

        PriorityTagTable.InsertItems(context, document.GetPriorityTags(fileHeaderId));
        ReadingInfoTagTable.InsertItems(context, document.GetReadingInfoTags(fileHeaderId));
        KanjiFormInfoTagTable.InsertItems(context, document.GetKanjiFormInfoTags(fileHeaderId));
        NameTypeTagTable.InsertItems(context, document.GetNameTypeTags(fileHeaderId));
        DetailLanguageTable.InsertItems(context, document.GetDetailLanguages(fileHeaderId));

        EntryTable.InsertItems(context, document.Entries.Values);
        KanjiFormTable.InsertItems(context, document.KanjiForms.Values);
        ReadingTable.InsertItems(context, document.Readings.Values);
        TranslationTable.InsertItems(context, document.Translations.Values);
        KanjiFormInfoTable.InsertItems(context, document.KanjiFormInfos.Values);
        KanjiFormPriorityTable.InsertItems(context, document.KanjiFormPriorities.Values);
        ReadingInfoTable.InsertItems(context, document.ReadingInfos.Values);
        ReadingPriorityTable.InsertItems(context, document.ReadingPriorities.Values);
        RestrictionTable.InsertItems(context, document.Restrictions.Values);
        CrossReferenceTable.InsertItems(context, document.CrossReferences.Values);
        DetailTable.InsertItems(context, document.Details.Values);
        NameTypeTable.InsertItems(context, document.NameTypes.Values);

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
        SequenceTable.InsertOrIgnoreItems(context, diff.Inserts.GetSequences(fileHeaderId));

        PriorityTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetPriorityTags(fileHeaderId));
        ReadingInfoTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetReadingInfoTags(fileHeaderId));
        KanjiFormInfoTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetKanjiFormInfoTags(fileHeaderId));
        NameTypeTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetNameTypeTags(fileHeaderId));
        DetailLanguageTable.InsertItems(context, diff.Inserts.GetDetailLanguages(fileHeaderId));

        EntryTable.InsertItems(context, diff.Inserts.Entries.Values);
        KanjiFormTable.InsertItems(context, diff.Inserts.KanjiForms.Values);
        ReadingTable.InsertItems(context, diff.Inserts.Readings.Values);
        TranslationTable.InsertItems(context, diff.Inserts.Translations.Values);
        KanjiFormInfoTable.InsertItems(context, diff.Inserts.KanjiFormInfos.Values);
        KanjiFormPriorityTable.InsertItems(context, diff.Inserts.KanjiFormPriorities.Values);
        ReadingInfoTable.InsertItems(context, diff.Inserts.ReadingInfos.Values);
        ReadingPriorityTable.InsertItems(context, diff.Inserts.ReadingPriorities.Values);
        RestrictionTable.InsertItems(context, diff.Inserts.Restrictions.Values);
        CrossReferenceTable.InsertItems(context, diff.Inserts.CrossReferences.Values);
        DetailTable.InsertItems(context, diff.Inserts.Details.Values);
        NameTypeTable.InsertItems(context, diff.Inserts.NameTypes.Values);

        EntryTable.UpdateItems(context, diff.Updates.Entries.Values);
        KanjiFormTable.UpdateItems(context, diff.Updates.KanjiForms.Values);
        ReadingTable.UpdateItems(context, diff.Updates.Readings.Values);
        TranslationTable.UpdateItems(context, diff.Updates.Translations.Values);
        KanjiFormInfoTable.UpdateItems(context, diff.Updates.KanjiFormInfos.Values);
        KanjiFormPriorityTable.UpdateItems(context, diff.Updates.KanjiFormPriorities.Values);
        ReadingInfoTable.UpdateItems(context, diff.Updates.ReadingInfos.Values);
        ReadingPriorityTable.UpdateItems(context, diff.Updates.ReadingPriorities.Values);
        RestrictionTable.UpdateItems(context, diff.Updates.Restrictions.Values);
        CrossReferenceTable.UpdateItems(context, diff.Updates.CrossReferences.Values);
        DetailTable.UpdateItems(context, diff.Updates.Details.Values);
        NameTypeTable.UpdateItems(context, diff.Updates.NameTypes.Values);

        NameTypeTable.DeleteItems(context, diff.Deletes.NameTypes.Values);
        DetailTable.DeleteItems(context, diff.Deletes.Details.Values);
        CrossReferenceTable.DeleteItems(context, diff.Deletes.CrossReferences.Values);
        RestrictionTable.DeleteItems(context, diff.Deletes.Restrictions.Values);
        ReadingPriorityTable.DeleteItems(context, diff.Deletes.ReadingPriorities.Values);
        ReadingInfoTable.DeleteItems(context, diff.Deletes.ReadingInfos.Values);
        KanjiFormPriorityTable.DeleteItems(context, diff.Deletes.KanjiFormPriorities.Values);
        KanjiFormInfoTable.DeleteItems(context, diff.Deletes.KanjiFormInfos.Values);
        TranslationTable.DeleteItems(context, diff.Deletes.Translations.Values);
        ReadingTable.DeleteItems(context, diff.Deletes.Readings.Values);
        KanjiFormTable.DeleteItems(context, diff.Deletes.KanjiForms.Values);
        EntryTable.DeleteItems(context, diff.Deletes.Entries.Values);

        var bSequences = DtoMapper.LoadSequencesWithoutRevisions(context, sequenceIds);

        var sequences = context.Sequences
            .Where(seq => sequenceIds.Contains(seq.Id))
            .Select(seq => new
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
