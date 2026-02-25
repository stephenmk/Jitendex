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

using Jitendex.Import;

namespace Jitendex.Kanjidic2.Import.Models;

internal sealed record DocumentSequence(int Id, int FileHeaderId);
internal sealed record DocumentRevision(int SequenceId, int Number, int FileHeaderId, string DiffJson);

internal sealed class Document
{
    public required DocumentHeader Header { get; init; }
    public Dictionary<int, EntryElement> Entries { get; init; }

    #region Keywords
    public Dictionary<string, CodepointTypeElement> CodepointTypes { get; init; } = [];
    public Dictionary<string, DictionaryTypeElement> DictionaryTypes { get; init; } = [];
    public Dictionary<string, QueryCodeTypeElement> QueryCodeTypes { get; init; } = [];
    public Dictionary<string, MisclassificationTypeElement> MisclassificationTypes { get; init; } = [];
    public Dictionary<string, RadicalTypeElement> RadicalTypes { get; init; } = [];
    public Dictionary<string, ReadingTypeElement> ReadingTypes { get; init; } = [];
    public Dictionary<string, VariantTypeElement> VariantTypes { get; init; } = [];
    #endregion

    #region Groups
    public Dictionary<(int, int), CodepointGroupElement> CodepointGroups { get; init; }
    public Dictionary<(int, int), DictionaryGroupElement> DictionaryGroups { get; init; }
    public Dictionary<(int, int), MiscGroupElement> MiscGroups { get; init; }
    public Dictionary<(int, int), QueryCodeGroupElement> QueryCodeGroups { get; init; }
    public Dictionary<(int, int), RadicalGroupElement> RadicalGroups { get; init; }
    public Dictionary<(int, int), ReadingMeaningGroupElement> ReadingMeaningGroups { get; init; }
    #endregion

    #region Group Elements
    public Dictionary<(int, int, int), CodepointElement> Codepoints { get; init; }
    public Dictionary<(int, int, int), DictionaryElement> Dictionaries { get; init; }
    public Dictionary<(int, int, int), NanoriElement> Nanoris { get; init; }
    public Dictionary<(int, int, int), QueryCodeElement> QueryCodes { get; init; }
    public Dictionary<(int, int, int), RadicalElement> Radicals { get; init; }
    public Dictionary<(int, int, int), RadicalNameElement> RadicalNames { get; init; }
    public Dictionary<(int, int, int), ReadingMeaningElement> ReadingMeanings { get; init; }
    public Dictionary<(int, int, int), StrokeCountElement> StrokeCounts { get; init; }
    public Dictionary<(int, int, int), VariantElement> Variants { get; init; }
    #endregion

    #region Subgroup Elements
    public Dictionary<(int, int, int, int), MeaningElement> Meanings { get; init; }
    public Dictionary<(int, int, int, int), ReadingElement> Readings { get; init; }
    #endregion

    public Document(int expectedEntryCount = 13_200)
    {
        Entries = new(expectedEntryCount);

        CodepointGroups = new(expectedEntryCount);
        DictionaryGroups = new(expectedEntryCount);
        MiscGroups = new(expectedEntryCount);
        QueryCodeGroups = new(expectedEntryCount);
        RadicalGroups = new(expectedEntryCount);
        ReadingMeaningGroups = new(expectedEntryCount);

        Codepoints = new(expectedEntryCount * 3);
        Dictionaries = new(expectedEntryCount * 6);
        Nanoris = new(expectedEntryCount / 2);
        QueryCodes = new(expectedEntryCount * 3);
        Radicals = new(expectedEntryCount * 2);
        RadicalNames = new(expectedEntryCount / 66);
        ReadingMeanings = new(expectedEntryCount);
        StrokeCounts = new(expectedEntryCount * 2);
        Variants = new(expectedEntryCount / 2);

        Meanings = new(expectedEntryCount * 2);
        Readings = new(expectedEntryCount * 7);
    }

    public IEnumerable<DocumentSequence> GetSequences(int fileHeaderId)
        => Entries.Select(e => new DocumentSequence(e.Key, fileHeaderId));

    public IEnumerable<int> ConcatAllEntryIds()
        => Entries.Keys
            .Concat(CodepointGroups.EntryIds())
            .Concat(DictionaryGroups.EntryIds())
            .Concat(MiscGroups.EntryIds())
            .Concat(QueryCodeGroups.EntryIds())
            .Concat(RadicalGroups.EntryIds())
            .Concat(ReadingMeaningGroups.EntryIds())

            .Concat(Codepoints.EntryIds())
            .Concat(Dictionaries.EntryIds())
            .Concat(Nanoris.EntryIds())
            .Concat(QueryCodes.EntryIds())
            .Concat(Radicals.EntryIds())
            .Concat(RadicalNames.EntryIds())
            .Concat(ReadingMeanings.EntryIds())
            .Concat(StrokeCounts.EntryIds())
            .Concat(Variants.EntryIds())

            .Concat(Meanings.EntryIds())
            .Concat(Readings.EntryIds());
}
