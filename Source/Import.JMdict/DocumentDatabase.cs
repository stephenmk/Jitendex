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

using Jitendex.Data.JMdict;
using Jitendex.Data.JMdict.Mappers;
using Jitendex.Import.JMdict.TableRows;
using Jitendex.Import.JMdict.Tables;
using Jitendex.Import.JMdict.Tables.EntryChildren;
using Jitendex.Import.JMdict.Tables.EntryChildren.KanjiFormChildren;
using Jitendex.Import.JMdict.Tables.EntryChildren.ReadingChildren;
using Jitendex.Import.JMdict.Tables.EntryChildren.SenseChildren;
using Jitendex.MinimalJsonDiff;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMdict;

internal sealed class DocumentDatabase(ILogger<DocumentDatabase> logger, JMdictContext context)
    : IDocumentDatabase<DateOnly, Document, DocumentDiff>
{
    private static readonly FileHeaderTable FileHeaderTable = new();
    private static readonly RevisionTable RevisionTable = new();
    private static readonly SequenceTable SequenceTable = new();
    private static readonly EntryTable EntryTable = new();

    #region Entry Children Tables
    private static readonly KanjiFormTable KanjiFormTable = new();
    private static readonly ReadingTable ReadingTable = new();
    private static readonly SenseTable SenseTable = new();
    private static readonly LanguageSourceTable LanguageSourceTable = new();
    private static readonly EntryNoteTable EntryNoteTable = new();
    #endregion

    #region Kanji Form Children Tables
    private static readonly KanjiFormInfoTable KanjiFormInfoTable = new();
    private static readonly KanjiFormPriorityTable KanjiFormPriorityTable = new();
    #endregion

    #region Reading Children Tables
    private static readonly ReadingInfoTable ReadingInfoTable = new();
    private static readonly ReadingPriorityTable ReadingPriorityTable = new();
    private static readonly RestrictionTable RestrictionTable = new();
    #endregion

    #region Sense Children Tables
    private static readonly CrossReferenceTable CrossReferenceTable = new();
    private static readonly DialectTable DialectTable = new();
    private static readonly FieldTable FieldTable = new();
    private static readonly GlossTable GlossTable = new();
    private static readonly GlossTypeTable GlossTypeTable = new();
    private static readonly KanjiFormRestrictionTable KanjiFormRestrictionTable = new();
    private static readonly MiscTable MiscTable = new();
    private static readonly SenseNoteTable SenseNoteTable = new();
    private static readonly PartOfSpeechTable PartOfSpeechTable = new();
    private static readonly ReadingRestrictionTable ReadingRestrictionTable = new();
    #endregion

    #region Keyword Tables
    private static readonly KeywordTable<ReadingInfoTagRow> ReadingInfoTagTable = new();
    private static readonly KeywordTable<KanjiFormInfoTagRow> KanjiFormInfoTagTable = new();
    private static readonly KeywordTable<PartOfSpeechTagRow> PartOfSpeechTagTable = new();
    private static readonly KeywordTable<FieldTagRow> FieldTagTable = new();
    private static readonly KeywordTable<MiscTagRow> MiscTagTable = new();
    private static readonly KeywordTable<DialectTagRow> DialectTagTable = new();
    private static readonly KeywordTable<GlossTypeTagRow> GlossTypeTagTable = new();
    private static readonly KeywordTable<CrossReferenceTypeRow> CrossReferenceTypeTable = new();
    private static readonly KeywordTable<LanguageSourceTypeRow> LanguageSourceTypeTable = new();
    private static readonly KeywordTable<PriorityTagRow> PriorityTagTable = new();
    private static readonly KeywordTable<LanguageRow> LanguageTable = new();
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
        var header = new HeaderRow(document.ArchiveKey, document.Version);

        FileHeaderTable.InsertItem(context, header);
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertItems(context, document.GetSequences(fileHeaderId));

        #pragma warning disable format

        ReadingInfoTagTable      .InsertItems(context, document.GetReadingInfoTags(fileHeaderId));
        KanjiFormInfoTagTable    .InsertItems(context, document.GetKanjiFormInfoTags(fileHeaderId));
        PartOfSpeechTagTable     .InsertItems(context, document.GetPartOfSpeechTags(fileHeaderId));
        FieldTagTable            .InsertItems(context, document.GetFieldTags(fileHeaderId));
        MiscTagTable             .InsertItems(context, document.GetMiscTags(fileHeaderId));
        DialectTagTable          .InsertItems(context, document.GetDialectTags(fileHeaderId));
        GlossTypeTagTable        .InsertItems(context, document.GetGlossTypeTags(fileHeaderId));
        CrossReferenceTypeTable  .InsertItems(context, document.GetCrossReferenceTypes(fileHeaderId));
        LanguageSourceTypeTable  .InsertItems(context, document.GetLanguageSourceTypes(fileHeaderId));
        PriorityTagTable         .InsertItems(context, document.GetPriorityTags(fileHeaderId));
        LanguageTable            .InsertItems(context, document.GetLanguages(fileHeaderId));

        EntryTable               .InsertItems(context, document.Entries.Values);
        KanjiFormTable           .InsertItems(context, document.KanjiForms.Values);
        ReadingTable             .InsertItems(context, document.Readings.Values);
        SenseTable               .InsertItems(context, document.Senses.Values);
        LanguageSourceTable      .InsertItems(context, document.LanguageSources.Values);
        EntryNoteTable           .InsertItems(context, document.EntryNotes.Values);
        KanjiFormInfoTable       .InsertItems(context, document.KanjiFormInfos.Values);
        KanjiFormPriorityTable   .InsertItems(context, document.KanjiFormPriorities.Values);
        ReadingInfoTable         .InsertItems(context, document.ReadingInfos.Values);
        ReadingPriorityTable     .InsertItems(context, document.ReadingPriorities.Values);
        RestrictionTable         .InsertItems(context, document.Restrictions.Values);
        CrossReferenceTable      .InsertItems(context, document.CrossReferences.Values);
        DialectTable             .InsertItems(context, document.Dialects.Values);
        FieldTable               .InsertItems(context, document.Fields.Values);
        GlossTable               .InsertItems(context, document.Glosses.Values);
        GlossTypeTable           .InsertItems(context, document.GlossTypes.Values);
        KanjiFormRestrictionTable.InsertItems(context, document.KanjiFormRestrictions.Values);
        MiscTable                .InsertItems(context, document.Miscs.Values);
        SenseNoteTable           .InsertItems(context, document.SenseNotes.Values);
        PartOfSpeechTable        .InsertItems(context, document.PartsOfSpeech.Values);
        ReadingRestrictionTable  .InsertItems(context, document.ReadingRestrictions.Values);

        #pragma warning restore format

        transaction.Commit();
        context.ExecuteVacuum();
    }

    public void Update(DocumentDiff diff)
    {
        var sequenceIds = diff.SequenceIds();

        logger.LogInformation("Updating {Count} entries with data from {Date:yyyy-MM-dd}", sequenceIds.Length, diff.ArchiveKey);

        using var transaction = context.Database.BeginTransaction();

        var aSequences = SequenceDictionaryLoader.Load(context, sequenceIds);
        var header = new HeaderRow(diff.Upserts.ArchiveKey, diff.Upserts.Version);

        FileHeaderTable.InsertItem(context, header);
        var fileHeaderId = (int)context.GetLastInsertRowId();

        #pragma warning disable format

        SequenceTable            .InsertOrIgnoreItems(context, diff.Upserts.GetSequences(fileHeaderId));
        ReadingInfoTagTable      .InsertOrIgnoreItems(context, diff.Upserts.GetReadingInfoTags(fileHeaderId));
        KanjiFormInfoTagTable    .InsertOrIgnoreItems(context, diff.Upserts.GetKanjiFormInfoTags(fileHeaderId));
        PartOfSpeechTagTable     .InsertOrIgnoreItems(context, diff.Upserts.GetPartOfSpeechTags(fileHeaderId));
        FieldTagTable            .InsertOrIgnoreItems(context, diff.Upserts.GetFieldTags(fileHeaderId));
        MiscTagTable             .InsertOrIgnoreItems(context, diff.Upserts.GetMiscTags(fileHeaderId));
        DialectTagTable          .InsertOrIgnoreItems(context, diff.Upserts.GetDialectTags(fileHeaderId));
        GlossTypeTagTable        .InsertOrIgnoreItems(context, diff.Upserts.GetGlossTypeTags(fileHeaderId));
        CrossReferenceTypeTable  .InsertOrIgnoreItems(context, diff.Upserts.GetCrossReferenceTypes(fileHeaderId));
        LanguageSourceTypeTable  .InsertOrIgnoreItems(context, diff.Upserts.GetLanguageSourceTypes(fileHeaderId));
        PriorityTagTable         .InsertOrIgnoreItems(context, diff.Upserts.GetPriorityTags(fileHeaderId));
        LanguageTable            .InsertOrIgnoreItems(context, diff.Upserts.GetLanguages(fileHeaderId));

        EntryTable               .UpsertItems(context, diff.Upserts.Entries.Values);
        KanjiFormTable           .UpsertItems(context, diff.Upserts.KanjiForms.Values);
        ReadingTable             .UpsertItems(context, diff.Upserts.Readings.Values);
        SenseTable               .UpsertItems(context, diff.Upserts.Senses.Values);
        LanguageSourceTable      .UpsertItems(context, diff.Upserts.LanguageSources.Values);
        KanjiFormInfoTable       .UpsertItems(context, diff.Upserts.KanjiFormInfos.Values);
        KanjiFormPriorityTable   .UpsertItems(context, diff.Upserts.KanjiFormPriorities.Values);
        ReadingInfoTable         .UpsertItems(context, diff.Upserts.ReadingInfos.Values);
        ReadingPriorityTable     .UpsertItems(context, diff.Upserts.ReadingPriorities.Values);
        RestrictionTable         .UpsertItems(context, diff.Upserts.Restrictions.Values);
        CrossReferenceTable      .UpsertItems(context, diff.Upserts.CrossReferences.Values);
        DialectTable             .UpsertItems(context, diff.Upserts.Dialects.Values);
        FieldTable               .UpsertItems(context, diff.Upserts.Fields.Values);
        GlossTable               .UpsertItems(context, diff.Upserts.Glosses.Values);
        GlossTypeTable           .UpsertItems(context, diff.Upserts.GlossTypes.Values);
        KanjiFormRestrictionTable.UpsertItems(context, diff.Upserts.KanjiFormRestrictions.Values);
        MiscTable                .UpsertItems(context, diff.Upserts.Miscs.Values);
        SenseNoteTable           .UpsertItems(context, diff.Upserts.SenseNotes.Values);
        PartOfSpeechTable        .UpsertItems(context, diff.Upserts.PartsOfSpeech.Values);
        ReadingRestrictionTable  .UpsertItems(context, diff.Upserts.ReadingRestrictions.Values);

        ReadingRestrictionTable  .DeleteItems(context, diff.Deletes.ReadingRestrictions.Values);
        PartOfSpeechTable        .DeleteItems(context, diff.Deletes.PartsOfSpeech.Values);
        SenseNoteTable           .DeleteItems(context, diff.Deletes.SenseNotes.Values);
        MiscTable                .DeleteItems(context, diff.Deletes.Miscs.Values);
        KanjiFormRestrictionTable.DeleteItems(context, diff.Deletes.KanjiFormRestrictions.Values);
        GlossTypeTable           .DeleteItems(context, diff.Deletes.GlossTypes.Values);
        GlossTable               .DeleteItems(context, diff.Deletes.Glosses.Values);
        FieldTable               .DeleteItems(context, diff.Deletes.Fields.Values);
        DialectTable             .DeleteItems(context, diff.Deletes.Dialects.Values);
        CrossReferenceTable      .DeleteItems(context, diff.Deletes.CrossReferences.Values);
        RestrictionTable         .DeleteItems(context, diff.Deletes.Restrictions.Values);
        ReadingPriorityTable     .DeleteItems(context, diff.Deletes.ReadingPriorities.Values);
        ReadingInfoTable         .DeleteItems(context, diff.Deletes.ReadingInfos.Values);
        KanjiFormPriorityTable   .DeleteItems(context, diff.Deletes.KanjiFormPriorities.Values);
        KanjiFormInfoTable       .DeleteItems(context, diff.Deletes.KanjiFormInfos.Values);
        LanguageSourceTable      .DeleteItems(context, diff.Deletes.LanguageSources.Values);
        SenseTable               .DeleteItems(context, diff.Deletes.Senses.Values);
        ReadingTable             .DeleteItems(context, diff.Deletes.Readings.Values);
        KanjiFormTable           .DeleteItems(context, diff.Deletes.KanjiForms.Values);
        EntryTable               .DeleteItems(context, diff.Deletes.Entries.Values);

        #pragma warning restore format

        var bSequences = SequenceDictionaryLoader.Load(context, sequenceIds);

        var sequences = context.Sequences
            .Where(seq => sequenceIds.Contains(seq.Id))
            .Select(seq => new
            {
                seq.Id,
                RevisionCount = seq.Revisions.Count,
            });

        var revisions = new List<RevisionRow>(aSequences.Count);

        foreach (var seq in sequences)
        {
            if (aSequences.TryGetValue(seq.Id, out var aSeq))
            {
                var bSeq = bSequences[seq.Id];
                var baDiff = JsonDiffer.DiffToUtf8Bytes(a: bSeq, b: aSeq);
                revisions.Add(new
                (
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
