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
using Jitendex.JMnedict.Import.Models;
using Jitendex.JMnedict.Import.Tables;
using Jitendex.JMnedict.Import.Tables.EntryElements;
using Jitendex.JMnedict.Import.Tables.EntryElements.KanjiFormElements;
using Jitendex.JMnedict.Import.Tables.EntryElements.ReadingElements;
using Jitendex.JMnedict.Import.Tables.EntryElements.TranslationElements;

namespace Jitendex.JMnedict.Import;

internal sealed class Database(ILogger<Database> logger, JMnedictContext context)
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
    #endregion

    public void Initialize(Document document)
    {
        logger.LogInformation("Initializing database with data from {Date:yyyy-MM-dd}", document.Header.Date);

        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        FileHeaderTable.InsertItem(context, document.Header);
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertItems(context, document.GetSequences(fileHeaderId));

        PriorityTagTable.InsertItems(context, document.PriorityTags.Values);
        ReadingInfoTagTable.InsertItems(context, document.ReadingInfoTags.Values);
        KanjiFormInfoTagTable.InsertItems(context, document.KanjiFormInfoTags.Values);
        NameTypeTagTable.InsertItems(context, document.NameTypeTags.Values);

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
        logger.LogInformation("Updating {Count} entries with data from {Date:yyyy-MM-dd}", diff.SequenceIds.Count, diff.FileHeader.Date);

        var aSequences = DtoMapper.LoadSequencesWithoutRevisions(context, diff.SequenceIds);

        FileHeaderTable.InsertItem(context, diff.InsertDocument.Header);
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertOrIgnoreItems(context, diff.InsertDocument.GetSequences(fileHeaderId));

        PriorityTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.PriorityTags.Values);
        ReadingInfoTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.ReadingInfoTags.Values);
        KanjiFormInfoTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.KanjiFormInfoTags.Values);
        NameTypeTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.NameTypeTags.Values);

        EntryTable.InsertItems(context, diff.InsertDocument.Entries.Values);
        KanjiFormTable.InsertItems(context, diff.InsertDocument.KanjiForms.Values);
        ReadingTable.InsertItems(context, diff.InsertDocument.Readings.Values);
        TranslationTable.InsertItems(context, diff.InsertDocument.Translations.Values);
        KanjiFormInfoTable.InsertItems(context, diff.InsertDocument.KanjiFormInfos.Values);
        KanjiFormPriorityTable.InsertItems(context, diff.InsertDocument.KanjiFormPriorities.Values);
        ReadingInfoTable.InsertItems(context, diff.InsertDocument.ReadingInfos.Values);
        ReadingPriorityTable.InsertItems(context, diff.InsertDocument.ReadingPriorities.Values);
        RestrictionTable.InsertItems(context, diff.InsertDocument.Restrictions.Values);
        CrossReferenceTable.InsertItems(context, diff.InsertDocument.CrossReferences.Values);
        DetailTable.InsertItems(context, diff.InsertDocument.Details.Values);
        NameTypeTable.InsertItems(context, diff.InsertDocument.NameTypes.Values);

        EntryTable.UpdateItems(context, diff.UpdateDocument.Entries.Values);
        KanjiFormTable.UpdateItems(context, diff.UpdateDocument.KanjiForms.Values);
        ReadingTable.UpdateItems(context, diff.UpdateDocument.Readings.Values);
        TranslationTable.UpdateItems(context, diff.UpdateDocument.Translations.Values);
        KanjiFormInfoTable.UpdateItems(context, diff.UpdateDocument.KanjiFormInfos.Values);
        KanjiFormPriorityTable.UpdateItems(context, diff.UpdateDocument.KanjiFormPriorities.Values);
        ReadingInfoTable.UpdateItems(context, diff.UpdateDocument.ReadingInfos.Values);
        ReadingPriorityTable.UpdateItems(context, diff.UpdateDocument.ReadingPriorities.Values);
        RestrictionTable.UpdateItems(context, diff.UpdateDocument.Restrictions.Values);
        CrossReferenceTable.UpdateItems(context, diff.UpdateDocument.CrossReferences.Values);
        DetailTable.UpdateItems(context, diff.UpdateDocument.Details.Values);
        NameTypeTable.UpdateItems(context, diff.UpdateDocument.NameTypes.Values);

        NameTypeTable.DeleteItems(context, diff.DeleteDocument.NameTypes.Values);
        DetailTable.DeleteItems(context, diff.DeleteDocument.Details.Values);
        CrossReferenceTable.DeleteItems(context, diff.DeleteDocument.CrossReferences.Values);
        RestrictionTable.DeleteItems(context, diff.DeleteDocument.Restrictions.Values);
        ReadingPriorityTable.DeleteItems(context, diff.DeleteDocument.ReadingPriorities.Values);
        ReadingInfoTable.DeleteItems(context, diff.DeleteDocument.ReadingInfos.Values);
        KanjiFormPriorityTable.DeleteItems(context, diff.DeleteDocument.KanjiFormPriorities.Values);
        KanjiFormInfoTable.DeleteItems(context, diff.DeleteDocument.KanjiFormInfos.Values);
        TranslationTable.DeleteItems(context, diff.DeleteDocument.Translations.Values);
        ReadingTable.DeleteItems(context, diff.DeleteDocument.Readings.Values);
        KanjiFormTable.DeleteItems(context, diff.DeleteDocument.KanjiForms.Values);
        EntryTable.DeleteItems(context, diff.DeleteDocument.Entries.Values);

        var bSequences = DtoMapper.LoadSequencesWithoutRevisions(context, diff.SequenceIds);

        var sequences = context.Sequences
            .Where(seq => diff.SequenceIds.Contains(seq.Id))
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
    }
}
