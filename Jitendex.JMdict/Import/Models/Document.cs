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

namespace Jitendex.JMdict.Import.Models;

internal sealed class Document
{
    public required DocumentHeader Header { get; init; }
    public Dictionary<int, EntryElement> Entries { get; init; }

    #region Entry Elements
    public Dictionary<(int, int), KanjiFormElement> KanjiForms { get; init; }
    public Dictionary<(int, int), ReadingElement> Readings { get; init; }
    public Dictionary<(int, int), SenseElement> Senses { get; init; }
    #endregion

    #region Kanji Form Elements
    public Dictionary<(int, int, int), KanjiFormInfoElement> KanjiFormInfos { get; init; }
    public Dictionary<(int, int, int), KanjiFormPriorityElement> KanjiFormPriorities { get; init; }
    #endregion

    #region Reading Elements
    public Dictionary<(int, int, int), ReadingInfoElement> ReadingInfos { get; init; }
    public Dictionary<(int, int, int), ReadingPriorityElement> ReadingPriorities { get; init; }
    public Dictionary<(int, int, int), RestrictionElement> Restrictions { get; init; }
    #endregion

    #region Sense Elements
    public Dictionary<(int, int, int), CrossReferenceElement> CrossReferences { get; init; }
    public Dictionary<(int, int, int), DialectElement> Dialects { get; init; }
    public Dictionary<(int, int, int), FieldElement> Fields { get; init; }
    public Dictionary<(int, int, int), GlossElement> Glosses { get; init; }
    public Dictionary<(int, int, int), KanjiFormRestrictionElement> KanjiFormRestrictions { get; init; }
    public Dictionary<(int, int, int), LanguageSourceElement> LanguageSources { get; init; }
    public Dictionary<(int, int, int), MiscElement> Miscs { get; init; }
    public Dictionary<(int, int, int), NoteElement> Notes { get; init; }
    public Dictionary<(int, int, int), PartOfSpeechElement> PartsOfSpeech { get; init; }
    public Dictionary<(int, int, int), ReadingRestrictionElement> ReadingRestrictions { get; init; }
    #endregion

    #region Keywords
    public Dictionary<string, PriorityTagElement> PriorityTags { get; init; } = [];
    public Dictionary<string, ReadingInfoTagElement> ReadingInfoTags { get; init; } = [];
    public Dictionary<string, KanjiFormInfoTagElement> KanjiFormInfoTags { get; init; } = [];
    public Dictionary<string, PartOfSpeechTagElement> PartOfSpeechTags { get; init; } = [];
    public Dictionary<string, FieldTagElement> FieldTags { get; init; } = [];
    public Dictionary<string, MiscTagElement> MiscTags { get; init; } = [];
    public Dictionary<string, DialectTagElement> DialectTags { get; init; } = [];
    public Dictionary<string, GlossTypeElement> GlossTypes { get; init; } = [];
    public Dictionary<string, CrossReferenceTypeElement> CrossReferenceTypes { get; init; } = [];
    public Dictionary<string, LanguageSourceTypeElement> LanguageSourceTypes { get; init; } = [];
    public Dictionary<string, LanguageElement> Languages { get; init; } = [];
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
        KanjiFormRestrictions = new(expectedEntryCount / 100);
        LanguageSources = new(expectedEntryCount / 30);
        Miscs = new(expectedEntryCount / 5);
        Notes = new(expectedEntryCount / 20);
        PartsOfSpeech = new(expectedEntryCount * 2);
        ReadingRestrictions = new(expectedEntryCount / 100);
    }

    public IEnumerable<DocumentSequence> GetSequences()
        => Entries.Select(e => new DocumentSequence(e.Key, Header.Date));

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
            .Concat(KanjiFormRestrictions.EntryIds())
            .Concat(LanguageSources.EntryIds())
            .Concat(Miscs.EntryIds())
            .Concat(Notes.EntryIds())
            .Concat(PartsOfSpeech.EntryIds())
            .Concat(ReadingRestrictions.EntryIds());
}

internal sealed record DocumentHeader(DateOnly Date);
internal sealed record DocumentSequence(int Id, DateOnly CreatedDate);
internal sealed record DocumentRevision(int SequenceId, int Number, DateOnly CreatedDate, string DiffJson);
