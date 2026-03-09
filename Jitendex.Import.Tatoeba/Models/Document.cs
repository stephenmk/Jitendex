/*
Copyright (c) 2025-2026 Stephen Kraus
SPDX-License-Identifier: AGPL-3.0-or-later

This file is part of Jitendex.

Jitendex is free software: you can redistribute it and/or modify it under the terms
of the GNU Affero General Public License as published by the Free Software Foundation,
either version 3 of the License or (at your option) any later version.

Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with Jitendex.
If not, see <https://www.gnu.org/licenses/>.
*/

using Jitendex.Import;

namespace Jitendex.Tatoeba.Import.Models;

internal sealed class Document : IDocument<DateOnly>
{
    public required DateOnly ArchiveKey { get; init; }
    public Dictionary<int, ExampleElement> Examples { get; init; }
    public Dictionary<int, TranslationElement> Translations { get; init; }
    public Dictionary<(int, int), SegmentationElement> Segmentations { get; init; }
    public Dictionary<(int, int, int), TokenElement> Tokens { get; init; }

    public Document(int expectedExampleCount = 150_000)
    {
        Examples = new(expectedExampleCount);
        Translations = new(expectedExampleCount);
        Segmentations = new(expectedExampleCount);
        Tokens = new(expectedExampleCount * 8);
    }

    public IEnumerable<SequenceElement> GetSequences(int fileHeaderId)
        => Examples.Select(e => new SequenceElement(e.Key, fileHeaderId));

    public IEnumerable<int> ConcatAllExampleIds()
        => Examples.Keys
            .Concat(Segmentations.EntryIds())
            .Concat(Tokens.EntryIds());

    public IEnumerable<int> PriorityEntryIds()
        => Tokens.Values
            .Where(static token => token.IsPriority)
            .Select(static token => token.ExampleId);
}
