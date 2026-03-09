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
using Jitendex.Import;
using Jitendex.MinimalJsonDiff;
using Jitendex.Data.JMdict;
using Jitendex.JMdict.Import.Models;
using Jitendex.JMdict.Import.Tables;
using Jitendex.JMdict.Import.Tables.EntryElements;
using Jitendex.JMdict.Import.Tables.EntryElements.KanjiFormElements;
using Jitendex.JMdict.Import.Tables.EntryElements.ReadingElements;
using Jitendex.JMdict.Import.Tables.EntryElements.SenseElements;

namespace Jitendex.JMdict.Import;

internal sealed class DocumentDatabase(ILogger<DocumentDatabase> logger, JmdictContext context)
    : IDocumentDatabase<DateOnly, Document, DocumentDiff>
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

        ReadingInfoTagTable.InsertItems(context, document.GetReadingInfoTags(fileHeaderId));
        KanjiFormInfoTagTable.InsertItems(context, document.GetKanjiFormInfoTags(fileHeaderId));
        PartOfSpeechTagTable.InsertItems(context, document.GetPartOfSpeechTags(fileHeaderId));
        FieldTagTable.InsertItems(context, document.GetFieldTags(fileHeaderId));
        MiscTagTable.InsertItems(context, document.GetMiscTags(fileHeaderId));
        DialectTagTable.InsertItems(context, document.GetDialectTags(fileHeaderId));
        GlossTypeTable.InsertItems(context, document.GetGlossTypes(fileHeaderId));
        CrossReferenceTypeTable.InsertItems(context, document.GetCrossReferenceTypes(fileHeaderId));
        LanguageSourceTypeTable.InsertItems(context, document.GetLanguageSourceTypes(fileHeaderId));
        PriorityTagTable.InsertItems(context, document.GetPriorityTags(fileHeaderId));
        LanguageTable.InsertItems(context, document.GetLanguages(fileHeaderId));

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
        context.ExecuteVacuum();
    }

    public void Update(DocumentDiff diff)
    {
        var sequenceIds = diff.SequenceIds();

        logger.LogInformation("Updating {Count} entries with data from {Date:yyyy-MM-dd}", sequenceIds.Count, diff.ArchiveKey);

        using var transaction = context.Database.BeginTransaction();

        var aSequences = DtoMapper.LoadSequencesWithoutRevisions(context, sequenceIds);

        FileHeaderTable.InsertItem(context, new(diff.Inserts.ArchiveKey));
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertOrIgnoreItems(context, diff.Inserts.GetSequences(fileHeaderId));

        ReadingInfoTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetReadingInfoTags(fileHeaderId));
        KanjiFormInfoTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetKanjiFormInfoTags(fileHeaderId));
        PartOfSpeechTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetPartOfSpeechTags(fileHeaderId));
        FieldTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetFieldTags(fileHeaderId));
        MiscTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetMiscTags(fileHeaderId));
        DialectTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetDialectTags(fileHeaderId));
        GlossTypeTable.InsertOrIgnoreItems(context, diff.Inserts.GetGlossTypes(fileHeaderId));
        CrossReferenceTypeTable.InsertOrIgnoreItems(context, diff.Inserts.GetCrossReferenceTypes(fileHeaderId));
        LanguageSourceTypeTable.InsertOrIgnoreItems(context, diff.Inserts.GetLanguageSourceTypes(fileHeaderId));
        PriorityTagTable.InsertOrIgnoreItems(context, diff.Inserts.GetPriorityTags(fileHeaderId));
        LanguageTable.InsertOrIgnoreItems(context, diff.Inserts.GetLanguages(fileHeaderId));

        EntryTable.InsertItems(context, diff.Inserts.Entries.Values);
        KanjiFormTable.InsertItems(context, diff.Inserts.KanjiForms.Values);
        ReadingTable.InsertItems(context, diff.Inserts.Readings.Values);
        SenseTable.InsertItems(context, diff.Inserts.Senses.Values);
        KanjiFormInfoTable.InsertItems(context, diff.Inserts.KanjiFormInfos.Values);
        KanjiFormPriorityTable.InsertItems(context, diff.Inserts.KanjiFormPriorities.Values);
        ReadingInfoTable.InsertItems(context, diff.Inserts.ReadingInfos.Values);
        ReadingPriorityTable.InsertItems(context, diff.Inserts.ReadingPriorities.Values);
        RestrictionTable.InsertItems(context, diff.Inserts.Restrictions.Values);
        CrossReferenceTable.InsertItems(context, diff.Inserts.CrossReferences.Values);
        DialectTable.InsertItems(context, diff.Inserts.Dialects.Values);
        FieldTable.InsertItems(context, diff.Inserts.Fields.Values);
        GlossTable.InsertItems(context, diff.Inserts.Glosses.Values);
        KanjiFormRestrictionTable.InsertItems(context, diff.Inserts.KanjiFormRestrictions.Values);
        LanguageSourceTable.InsertItems(context, diff.Inserts.LanguageSources.Values);
        MiscTable.InsertItems(context, diff.Inserts.Miscs.Values);
        NoteTable.InsertItems(context, diff.Inserts.Notes.Values);
        PartOfSpeechTable.InsertItems(context, diff.Inserts.PartsOfSpeech.Values);
        ReadingRestrictionTable.InsertItems(context, diff.Inserts.ReadingRestrictions.Values);

        EntryTable.UpdateItems(context, diff.Updates.Entries.Values);
        KanjiFormTable.UpdateItems(context, diff.Updates.KanjiForms.Values);
        ReadingTable.UpdateItems(context, diff.Updates.Readings.Values);
        SenseTable.UpdateItems(context, diff.Updates.Senses.Values);
        KanjiFormInfoTable.UpdateItems(context, diff.Updates.KanjiFormInfos.Values);
        KanjiFormPriorityTable.UpdateItems(context, diff.Updates.KanjiFormPriorities.Values);
        ReadingInfoTable.UpdateItems(context, diff.Updates.ReadingInfos.Values);
        ReadingPriorityTable.UpdateItems(context, diff.Updates.ReadingPriorities.Values);
        RestrictionTable.UpdateItems(context, diff.Updates.Restrictions.Values);
        CrossReferenceTable.UpdateItems(context, diff.Updates.CrossReferences.Values);
        DialectTable.UpdateItems(context, diff.Updates.Dialects.Values);
        FieldTable.UpdateItems(context, diff.Updates.Fields.Values);
        GlossTable.UpdateItems(context, diff.Updates.Glosses.Values);
        KanjiFormRestrictionTable.UpdateItems(context, diff.Updates.KanjiFormRestrictions.Values);
        LanguageSourceTable.UpdateItems(context, diff.Updates.LanguageSources.Values);
        MiscTable.UpdateItems(context, diff.Updates.Miscs.Values);
        NoteTable.UpdateItems(context, diff.Updates.Notes.Values);
        PartOfSpeechTable.UpdateItems(context, diff.Updates.PartsOfSpeech.Values);
        ReadingRestrictionTable.UpdateItems(context, diff.Updates.ReadingRestrictions.Values);

        ReadingRestrictionTable.DeleteItems(context, diff.Deletes.ReadingRestrictions.Values);
        PartOfSpeechTable.DeleteItems(context, diff.Deletes.PartsOfSpeech.Values);
        NoteTable.DeleteItems(context, diff.Deletes.Notes.Values);
        MiscTable.DeleteItems(context, diff.Deletes.Miscs.Values);
        LanguageSourceTable.DeleteItems(context, diff.Deletes.LanguageSources.Values);
        KanjiFormRestrictionTable.DeleteItems(context, diff.Deletes.KanjiFormRestrictions.Values);
        GlossTable.DeleteItems(context, diff.Deletes.Glosses.Values);
        FieldTable.DeleteItems(context, diff.Deletes.Fields.Values);
        DialectTable.DeleteItems(context, diff.Deletes.Dialects.Values);
        CrossReferenceTable.DeleteItems(context, diff.Deletes.CrossReferences.Values);
        RestrictionTable.DeleteItems(context, diff.Deletes.Restrictions.Values);
        ReadingPriorityTable.DeleteItems(context, diff.Deletes.ReadingPriorities.Values);
        ReadingInfoTable.DeleteItems(context, diff.Deletes.ReadingInfos.Values);
        KanjiFormPriorityTable.DeleteItems(context, diff.Deletes.KanjiFormPriorities.Values);
        KanjiFormInfoTable.DeleteItems(context, diff.Deletes.KanjiFormInfos.Values);
        SenseTable.DeleteItems(context, diff.Deletes.Senses.Values);
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
                    FileHeaderId: fileHeaderId,
                    DiffJson: baDiff
                ));
            }
        }

        RevisionTable.InsertItems(context, revisions);
        transaction.Commit();
    }
}
