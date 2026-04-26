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

using Jitendex.Import.JMnedict.TableRows;

namespace Jitendex.Import.JMnedict;

internal sealed class Document : IDocument<DateOnly>
{
    public required DateOnly ArchiveKey { get; init; }
    public Dictionary<int, EntryElement> Entries { get; init; }

    #region Entry Elements
    public Dictionary<(int, int), KanjiFormRow> KanjiForms { get; init; }
    public Dictionary<(int, int), ReadingRow> Readings { get; init; }
    public Dictionary<(int, int), TranslationRow> Translations { get; init; }
    #endregion

    #region Kanji Form Elements
    public Dictionary<(int, int, int), KanjiFormInfoRow> KanjiFormInfos { get; init; }
    public Dictionary<(int, int, int), KanjiFormPriorityRow> KanjiFormPriorities { get; init; }
    #endregion

    #region Reading Elements
    public Dictionary<(int, int, int), ReadingInfoRow> ReadingInfos { get; init; }
    public Dictionary<(int, int, int), ReadingPriorityRow> ReadingPriorities { get; init; }
    public Dictionary<(int, int, int), RestrictionRow> Restrictions { get; init; }
    #endregion

    #region Translation Elements
    public Dictionary<(int, int, int), NameTypeRow> NameTypes { get; init; }
    public Dictionary<(int, int, int), DetailRow> Details { get; init; }
    public Dictionary<(int, int, int), CrossReferenceRow> CrossReferences { get; init; }
    #endregion

    #region Keywords
    public HashSet<string> PriorityTags { get; init; } = [];
    public HashSet<string> ReadingInfoTags { get; init; } = [];
    public HashSet<string> KanjiFormInfoTags { get; init; } = [];
    public HashSet<string> NameTypeTags { get; init; } = [];
    public HashSet<string> DetailLanguages { get; init; } = [];
    #endregion

    public Dictionary<string, string> KeywordDescriptionToName { get; init; } = [];

    public Document(int expectedEntryCount = 800_000)
    {
        Entries = new(expectedEntryCount);

        KanjiForms = new(expectedEntryCount);
        Readings = new(expectedEntryCount);
        Translations = new(expectedEntryCount);

        KanjiFormInfos = [];
        KanjiFormPriorities = [];

        ReadingInfos = [];
        ReadingPriorities = new(expectedEntryCount / 80);
        Restrictions = [];

        CrossReferences = [];
        Details = new(expectedEntryCount);
        NameTypes = new(expectedEntryCount);
    }

    public IEnumerable<DocumentSequence> GetSequences(int fileHeaderId)
        => Entries.Select(e => new DocumentSequence(e.Key, fileHeaderId));

    public IEnumerable<PriorityTagRow> GetPriorityTags(int fileHeaderId)
        => PriorityTags.Select(e => new PriorityTagRow(e, fileHeaderId));

    public IEnumerable<ReadingInfoTagRow> GetReadingInfoTags(int fileHeaderId)
        => ReadingInfoTags.Select(e => new ReadingInfoTagRow(e, fileHeaderId));

    public IEnumerable<KanjiFormInfoTagRow> GetKanjiFormInfoTags(int fileHeaderId)
        => KanjiFormInfoTags.Select(e => new KanjiFormInfoTagRow(e, fileHeaderId));

    public IEnumerable<NameTypeTagRow> GetNameTypeTags(int fileHeaderId)
        => NameTypeTags.Select(e => new NameTypeTagRow(e, fileHeaderId));

    public IEnumerable<DetailLanguageRow> GetDetailLanguages(int fileHeaderId)
        => DetailLanguages.Select(e => new DetailLanguageRow(e, fileHeaderId));

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
