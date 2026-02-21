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
using Jitendex.JMdict.Import.Models;
using Jitendex.JMdict.Import.Tables;
using Jitendex.JMdict.Import.Tables.EntryElements;
using Jitendex.JMdict.Import.Tables.EntryElements.KanjiFormElements;
using Jitendex.JMdict.Import.Tables.EntryElements.ReadingElements;
using Jitendex.JMdict.Import.Tables.EntryElements.SenseElements;

namespace Jitendex.JMdict.Import;

internal sealed class Database(ILogger<Database> logger, JmdictContext context)
{
    private static readonly FileHeaderTable FileHeaderTable = new();
    private static readonly RevisionTable RevisionTable = new();
    private static readonly SequenceTable SequenceTable = new();
    private static readonly EntryTable EntryTable = new();

    #region Entry Element Tables
    private static readonly KanjiFormTable KanjiFormTable = new();
    private static readonly ReadingTable ReadingTable = new();
    private static readonly SenseTable SenseTable = new();
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

    #region Sense Element Tables
    private static readonly CrossReferenceTable CrossReferenceTable = new();
    private static readonly DialectTable DialectTable = new();
    private static readonly FieldTable FieldTable = new();
    private static readonly GlossTable GlossTable = new();
    private static readonly KanjiFormRestrictionTable KanjiFormRestrictionTable = new();
    private static readonly LanguageSourceTable LanguageSourceTable = new();
    private static readonly MiscTable MiscTable = new();
    private static readonly NoteTable NoteTable = new();
    private static readonly PartOfSpeechTable PartOfSpeechTable = new();
    private static readonly ReadingRestrictionTable ReadingRestrictionTable = new();
    #endregion

    #region Keyword Tables
    private static readonly KeywordTable<ReadingInfoTagElement> ReadingInfoTagTable = new();
    private static readonly KeywordTable<KanjiFormInfoTagElement> KanjiFormInfoTagTable = new();
    private static readonly KeywordTable<PartOfSpeechTagElement> PartOfSpeechTagTable = new();
    private static readonly KeywordTable<FieldTagElement> FieldTagTable = new();
    private static readonly KeywordTable<MiscTagElement> MiscTagTable = new();
    private static readonly KeywordTable<DialectTagElement> DialectTagTable = new();
    private static readonly KeywordTable<GlossTypeElement> GlossTypeTable = new();
    private static readonly KeywordTable<CrossReferenceTypeElement> CrossReferenceTypeTable = new();
    private static readonly KeywordTable<LanguageSourceTypeElement> LanguageSourceTypeTable = new();
    private static readonly KeywordTable<PriorityTagElement> PriorityTagTable = new();
    private static readonly KeywordTable<LanguageElement> LanguageTable = new();
    #endregion

    public void Initialize(Document document)
    {
        logger.LogInformation("Initializing database with data from {Date:yyyy-MM-dd}", document.Header.Date);

        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        ReadingInfoTagTable.InsertItems(context, document.ReadingInfoTags.Values);
        KanjiFormInfoTagTable.InsertItems(context, document.KanjiFormInfoTags.Values);
        PartOfSpeechTagTable.InsertItems(context, document.PartOfSpeechTags.Values);
        FieldTagTable.InsertItems(context, document.FieldTags.Values);
        MiscTagTable.InsertItems(context, document.MiscTags.Values);
        DialectTagTable.InsertItems(context, document.DialectTags.Values);
        GlossTypeTable.InsertItems(context, document.GlossTypes.Values);
        CrossReferenceTypeTable.InsertItems(context, document.CrossReferenceTypes.Values);
        LanguageSourceTypeTable.InsertItems(context, document.LanguageSourceTypes.Values);
        PriorityTagTable.InsertItems(context, document.PriorityTags.Values);
        LanguageTable.InsertItems(context, document.Languages.Values);

        FileHeaderTable.InsertItem(context, document.Header);
        SequenceTable.InsertItems(context, document.GetSequences());

        EntryTable.InsertItems(context, document.Entries.Values);
        KanjiFormTable.InsertItems(context, document.KanjiForms.Values);
        ReadingTable.InsertItems(context, document.Readings.Values);
        SenseTable.InsertItems(context, document.Senses.Values);
        KanjiFormInfoTable.InsertItems(context, document.KanjiFormInfos.Values);
        KanjiFormPriorityTable.InsertItems(context, document.KanjiFormPriorities.Values);
        ReadingInfoTable.InsertItems(context, document.ReadingInfos.Values);
        ReadingPriorityTable.InsertItems(context, document.ReadingPriorities.Values);
        RestrictionTable.InsertItems(context, document.Restrictions.Values);
        CrossReferenceTable.InsertItems(context, document.CrossReferences.Values);
        DialectTable.InsertItems(context, document.Dialects.Values);
        FieldTable.InsertItems(context, document.Fields.Values);
        GlossTable.InsertItems(context, document.Glosses.Values);
        KanjiFormRestrictionTable.InsertItems(context, document.KanjiFormRestrictions.Values);
        LanguageSourceTable.InsertItems(context, document.LanguageSources.Values);
        MiscTable.InsertItems(context, document.Miscs.Values);
        NoteTable.InsertItems(context, document.Notes.Values);
        PartOfSpeechTable.InsertItems(context, document.PartsOfSpeech.Values);
        ReadingRestrictionTable.InsertItems(context, document.ReadingRestrictions.Values);

        transaction.Commit();
    }

    public void Update(DocumentDiff diff)
    {
        logger.LogInformation("Updating {Count} entries with data from {Date:yyyy-MM-dd}", diff.SequenceIds.Count, diff.FileHeader.Date);

        var aSequences = DtoMapper.LoadSequencesWithoutRevisions(context, diff.SequenceIds);

        ReadingInfoTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.ReadingInfoTags.Values);
        KanjiFormInfoTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.KanjiFormInfoTags.Values);
        PartOfSpeechTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.PartOfSpeechTags.Values);
        FieldTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.FieldTags.Values);
        MiscTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.MiscTags.Values);
        DialectTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.DialectTags.Values);
        GlossTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.GlossTypes.Values);
        CrossReferenceTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.CrossReferenceTypes.Values);
        LanguageSourceTypeTable.InsertOrIgnoreItems(context, diff.InsertDocument.LanguageSourceTypes.Values);
        PriorityTagTable.InsertOrIgnoreItems(context, diff.InsertDocument.PriorityTags.Values);
        LanguageTable.InsertOrIgnoreItems(context, diff.InsertDocument.Languages.Values);

        SequenceTable.InsertOrIgnoreItems(context, diff.InsertDocument.GetSequences());

        EntryTable.InsertItems(context, diff.InsertDocument.Entries.Values);
        KanjiFormTable.InsertItems(context, diff.InsertDocument.KanjiForms.Values);
        ReadingTable.InsertItems(context, diff.InsertDocument.Readings.Values);
        SenseTable.InsertItems(context, diff.InsertDocument.Senses.Values);
        KanjiFormInfoTable.InsertItems(context, diff.InsertDocument.KanjiFormInfos.Values);
        KanjiFormPriorityTable.InsertItems(context, diff.InsertDocument.KanjiFormPriorities.Values);
        ReadingInfoTable.InsertItems(context, diff.InsertDocument.ReadingInfos.Values);
        ReadingPriorityTable.InsertItems(context, diff.InsertDocument.ReadingPriorities.Values);
        RestrictionTable.InsertItems(context, diff.InsertDocument.Restrictions.Values);
        CrossReferenceTable.InsertItems(context, diff.InsertDocument.CrossReferences.Values);
        DialectTable.InsertItems(context, diff.InsertDocument.Dialects.Values);
        FieldTable.InsertItems(context, diff.InsertDocument.Fields.Values);
        GlossTable.InsertItems(context, diff.InsertDocument.Glosses.Values);
        KanjiFormRestrictionTable.InsertItems(context, diff.InsertDocument.KanjiFormRestrictions.Values);
        LanguageSourceTable.InsertItems(context, diff.InsertDocument.LanguageSources.Values);
        MiscTable.InsertItems(context, diff.InsertDocument.Miscs.Values);
        NoteTable.InsertItems(context, diff.InsertDocument.Notes.Values);
        PartOfSpeechTable.InsertItems(context, diff.InsertDocument.PartsOfSpeech.Values);
        ReadingRestrictionTable.InsertItems(context, diff.InsertDocument.ReadingRestrictions.Values);

        EntryTable.UpdateItems(context, diff.UpdateDocument.Entries.Values);
        KanjiFormTable.UpdateItems(context, diff.UpdateDocument.KanjiForms.Values);
        ReadingTable.UpdateItems(context, diff.UpdateDocument.Readings.Values);
        SenseTable.UpdateItems(context, diff.UpdateDocument.Senses.Values);
        KanjiFormInfoTable.UpdateItems(context, diff.UpdateDocument.KanjiFormInfos.Values);
        KanjiFormPriorityTable.UpdateItems(context, diff.UpdateDocument.KanjiFormPriorities.Values);
        ReadingInfoTable.UpdateItems(context, diff.UpdateDocument.ReadingInfos.Values);
        ReadingPriorityTable.UpdateItems(context, diff.UpdateDocument.ReadingPriorities.Values);
        RestrictionTable.UpdateItems(context, diff.UpdateDocument.Restrictions.Values);
        CrossReferenceTable.UpdateItems(context, diff.UpdateDocument.CrossReferences.Values);
        DialectTable.UpdateItems(context, diff.UpdateDocument.Dialects.Values);
        FieldTable.UpdateItems(context, diff.UpdateDocument.Fields.Values);
        GlossTable.UpdateItems(context, diff.UpdateDocument.Glosses.Values);
        KanjiFormRestrictionTable.UpdateItems(context, diff.UpdateDocument.KanjiFormRestrictions.Values);
        LanguageSourceTable.UpdateItems(context, diff.UpdateDocument.LanguageSources.Values);
        MiscTable.UpdateItems(context, diff.UpdateDocument.Miscs.Values);
        NoteTable.UpdateItems(context, diff.UpdateDocument.Notes.Values);
        PartOfSpeechTable.UpdateItems(context, diff.UpdateDocument.PartsOfSpeech.Values);
        ReadingRestrictionTable.UpdateItems(context, diff.UpdateDocument.ReadingRestrictions.Values);

        ReadingRestrictionTable.DeleteItems(context, diff.DeleteDocument.ReadingRestrictions.Values);
        PartOfSpeechTable.DeleteItems(context, diff.DeleteDocument.PartsOfSpeech.Values);
        NoteTable.DeleteItems(context, diff.DeleteDocument.Notes.Values);
        MiscTable.DeleteItems(context, diff.DeleteDocument.Miscs.Values);
        LanguageSourceTable.DeleteItems(context, diff.DeleteDocument.LanguageSources.Values);
        KanjiFormRestrictionTable.DeleteItems(context, diff.DeleteDocument.KanjiFormRestrictions.Values);
        GlossTable.DeleteItems(context, diff.DeleteDocument.Glosses.Values);
        FieldTable.DeleteItems(context, diff.DeleteDocument.Fields.Values);
        DialectTable.DeleteItems(context, diff.DeleteDocument.Dialects.Values);
        CrossReferenceTable.DeleteItems(context, diff.DeleteDocument.CrossReferences.Values);
        RestrictionTable.DeleteItems(context, diff.DeleteDocument.Restrictions.Values);
        ReadingPriorityTable.DeleteItems(context, diff.DeleteDocument.ReadingPriorities.Values);
        ReadingInfoTable.DeleteItems(context, diff.DeleteDocument.ReadingInfos.Values);
        KanjiFormPriorityTable.DeleteItems(context, diff.DeleteDocument.KanjiFormPriorities.Values);
        KanjiFormInfoTable.DeleteItems(context, diff.DeleteDocument.KanjiFormInfos.Values);
        SenseTable.DeleteItems(context, diff.DeleteDocument.Senses.Values);
        ReadingTable.DeleteItems(context, diff.DeleteDocument.Readings.Values);
        KanjiFormTable.DeleteItems(context, diff.DeleteDocument.KanjiForms.Values);
        EntryTable.DeleteItems(context, diff.DeleteDocument.Entries.Values);

        FileHeaderTable.InsertItem(context, diff.InsertDocument.Header);
        var fileHeaderId = (int)context.GetLastInsertRowId();

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
