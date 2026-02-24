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

namespace Jitendex.JMnedict.Import.Models;

internal sealed class Document
{
    public required DocumentHeader Header { get; init; }
    public Dictionary<int, EntryElement> Entries { get; init; }

    #region Entry Elements
    public Dictionary<(int, int), KanjiFormElement> KanjiForms { get; init; }
    public Dictionary<(int, int), ReadingElement> Readings { get; init; }
    public Dictionary<(int, int), TranslationElement> Translations { get; init; }
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

    #region Translation Elements
    public Dictionary<(int, int, int), NameTypeElement> NameTypes { get; init; }
    public Dictionary<(int, int, int), DetailElement> Details { get; init; }
    public Dictionary<(int, int, int), CrossReferenceElement> CrossReferences { get; init; }
    #endregion

    #region Keywords
    public Dictionary<string, PriorityTagElement> PriorityTags { get; init; } = [];
    public Dictionary<string, ReadingInfoTagElement> ReadingInfoTags { get; init; } = [];
    public Dictionary<string, KanjiFormInfoTagElement> KanjiFormInfoTags { get; init; } = [];
    public Dictionary<string, NameTypeTagElement> NameTypeTags { get; init; } = [];
    public Dictionary<string, DetailLanguageElement> DetailLanguages { get; init; } = [];
    #endregion

    public Dictionary<string, string> KeywordDescriptionToName { get; init; } = [];

    public Document(int expectedEntryCount = 250_000)
    {
        Entries = new(expectedEntryCount);

        KanjiForms = new(expectedEntryCount);
        Readings = new(expectedEntryCount);
        Translations = new(expectedEntryCount);

        KanjiFormInfos = new(expectedEntryCount / 20);
        KanjiFormPriorities = new(expectedEntryCount / 4);

        ReadingInfos = new(expectedEntryCount / 30);
        ReadingPriorities = new(expectedEntryCount / 4);
        Restrictions = new(expectedEntryCount / 25);

        CrossReferences = [];
        Details = new(expectedEntryCount * 2);
        NameTypes = new(expectedEntryCount / 5);
    }

    public IEnumerable<DocumentSequence> GetSequences(int fileHeaderId)
        => Entries.Select(e => new DocumentSequence(e.Key, fileHeaderId));

    public IEnumerable<int> ConcatAllEntryIds()
        => Entries.Keys
            .Concat(KanjiForms.EntryIds())
            .Concat(Readings.EntryIds())
            .Concat(Translations.EntryIds())
            .Concat(KanjiFormInfos.EntryIds())
            .Concat(KanjiFormPriorities.EntryIds())
            .Concat(ReadingInfos.EntryIds())
            .Concat(ReadingPriorities.EntryIds())
            .Concat(Restrictions.EntryIds())
            .Concat(CrossReferences.EntryIds())
            .Concat(Details.EntryIds())
            .Concat(NameTypes.EntryIds());
}
