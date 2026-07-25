// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Document.cs, is part of Jitendex.
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

using Jitendex.Import.JMdict.TableRows;

namespace Jitendex.Import.JMdict;

internal sealed class Document : IDocument<DateOnly>
{
    public required DateOnly ArchiveKey { get; init; }
    public required string Version { get; set; }

    public Dictionary<int, EntryRow> Entries { get; init; }

    #region Entry Children
    public Dictionary<(int, int), KanjiFormRow> KanjiForms { get; init; }
    public Dictionary<(int, int), ReadingRow> Readings { get; init; }
    public Dictionary<(int, int), SenseRow> Senses { get; init; }
    #endregion

    #region Kanji Form Children
    public Dictionary<(int, int, int), KanjiFormInfoRow> KanjiFormInfos { get; init; }
    public Dictionary<(int, int, int), KanjiFormPriorityRow> KanjiFormPriorities { get; init; }
    #endregion

    #region Reading Children
    public Dictionary<(int, int, int), ReadingInfoRow> ReadingInfos { get; init; }
    public Dictionary<(int, int, int), ReadingPriorityRow> ReadingPriorities { get; init; }
    public Dictionary<(int, int, int), RestrictionRow> Restrictions { get; init; }
    #endregion

    #region Sense Children
    public Dictionary<(int, int, int), CrossReferenceRow> CrossReferences { get; init; }
    public Dictionary<(int, int, int), DialectRow> Dialects { get; init; }
    public Dictionary<(int, int, int), FieldRow> Fields { get; init; }
    public Dictionary<(int, int, int), GlossRow> Glosses { get; init; }
    public Dictionary<(int, int, int), GlossTypeRow> GlossTypes { get; init; }
    public Dictionary<(int, int, int), KanjiFormRestrictionRow> KanjiFormRestrictions { get; init; }
    public Dictionary<(int, int, int), LanguageSourceRow> LanguageSources { get; init; }
    public Dictionary<(int, int, int), MiscRow> Miscs { get; init; }
    public Dictionary<(int, int, int), NoteRow> Notes { get; init; }
    public Dictionary<(int, int, int), PartOfSpeechRow> PartsOfSpeech { get; init; }
    public Dictionary<(int, int, int), ReadingRestrictionRow> ReadingRestrictions { get; init; }
    #endregion

    #region Keywords
    public HashSet<string> PriorityTags { get; init; } = [];
    public HashSet<string> ReadingInfoTags { get; init; } = [];
    public HashSet<string> KanjiFormInfoTags { get; init; } = [];
    public HashSet<string> PartOfSpeechTags { get; init; } = [];
    public HashSet<string> FieldTags { get; init; } = [];
    public HashSet<string> MiscTags { get; init; } = [];
    public HashSet<string> DialectTags { get; init; } = [];
    public HashSet<string> GlossTypeTags { get; init; } = [];
    public HashSet<string> CrossReferenceTypes { get; init; } = [];
    public HashSet<string> LanguageSourceTypes { get; init; } = [];
    public HashSet<string> Languages { get; init; } = [];
    #endregion

    public Dictionary<string, string> KeywordDescriptionToName { get; init; } = [];

    public Document(int expectedEntryCount = 250_000)
    {
        Entries = new(expectedEntryCount);

        KanjiForms = new(expectedEntryCount);
        Readings = new(expectedEntryCount);
        Senses = new(expectedEntryCount);

        KanjiFormInfos = new(expectedEntryCount / 20);
        KanjiFormPriorities = new(expectedEntryCount / 4);

        ReadingInfos = new(expectedEntryCount / 30);
        ReadingPriorities = new(expectedEntryCount / 4);
        Restrictions = new(expectedEntryCount / 25);

        CrossReferences = new(expectedEntryCount / 5);
        Dialects = new(expectedEntryCount / 100);
        Fields = new(expectedEntryCount / 5);
        Glosses = new(expectedEntryCount * 2);
        GlossTypes = new(expectedEntryCount / 30);
        KanjiFormRestrictions = new(expectedEntryCount / 100);
        LanguageSources = new(expectedEntryCount / 30);
        Miscs = new(expectedEntryCount / 5);
        Notes = new(expectedEntryCount / 20);
        PartsOfSpeech = new(expectedEntryCount * 2);
        ReadingRestrictions = new(expectedEntryCount / 100);
    }

    public IEnumerable<SequenceRow> GetSequences(int fileHeaderId)
        => Entries.Select(e => new SequenceRow(e.Key, fileHeaderId));

    public IEnumerable<PriorityTagRow> GetPriorityTags(int fileHeaderId)
        => PriorityTags.Select(t => new PriorityTagRow(t, fileHeaderId));

    public IEnumerable<ReadingInfoTagRow> GetReadingInfoTags(int fileHeaderId)
        => ReadingInfoTags.Select(t => new ReadingInfoTagRow(t, fileHeaderId));

    public IEnumerable<KanjiFormInfoTagRow> GetKanjiFormInfoTags(int fileHeaderId)
        => KanjiFormInfoTags.Select(t => new KanjiFormInfoTagRow(t, fileHeaderId));

    public IEnumerable<PartOfSpeechTagRow> GetPartOfSpeechTags(int fileHeaderId)
        => PartOfSpeechTags.Select(t => new PartOfSpeechTagRow(t, fileHeaderId));

    public IEnumerable<FieldTagRow> GetFieldTags(int fileHeaderId)
        => FieldTags.Select(t => new FieldTagRow(t, fileHeaderId));

    public IEnumerable<MiscTagRow> GetMiscTags(int fileHeaderId)
        => MiscTags.Select(t => new MiscTagRow(t, fileHeaderId));

    public IEnumerable<DialectTagRow> GetDialectTags(int fileHeaderId)
        => DialectTags.Select(t => new DialectTagRow(t, fileHeaderId));

    public IEnumerable<GlossTypeTagRow> GetGlossTypeTags(int fileHeaderId)
        => GlossTypeTags.Select(t => new GlossTypeTagRow(t, fileHeaderId));

    public IEnumerable<CrossReferenceTypeRow> GetCrossReferenceTypes(int fileHeaderId)
        => CrossReferenceTypes.Select(t => new CrossReferenceTypeRow(t, fileHeaderId));

    public IEnumerable<LanguageSourceTypeRow> GetLanguageSourceTypes(int fileHeaderId)
        => LanguageSourceTypes.Select(t => new LanguageSourceTypeRow(t, fileHeaderId));

    public IEnumerable<LanguageRow> GetLanguages(int fileHeaderId)
        => Languages.Select(t => new LanguageRow(t, fileHeaderId));

    public IEnumerable<int> ConcatAllEntryIds()
        => Entries.Keys
            .Concat(KanjiForms.EntryIds())
            .Concat(Readings.EntryIds())
            .Concat(Senses.EntryIds())
            .Concat(KanjiFormInfos.EntryIds())
            .Concat(KanjiFormPriorities.EntryIds())
            .Concat(ReadingInfos.EntryIds())
            .Concat(ReadingPriorities.EntryIds())
            .Concat(Restrictions.EntryIds())
            .Concat(CrossReferences.EntryIds())
            .Concat(Dialects.EntryIds())
            .Concat(Fields.EntryIds())
            .Concat(Glosses.EntryIds())
            .Concat(GlossTypes.EntryIds())
            .Concat(KanjiFormRestrictions.EntryIds())
            .Concat(LanguageSources.EntryIds())
            .Concat(Miscs.EntryIds())
            .Concat(Notes.EntryIds())
            .Concat(PartsOfSpeech.EntryIds())
            .Concat(ReadingRestrictions.EntryIds());
}
