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

using Jitendex.Import.Tatoeba.RowModels;

namespace Jitendex.Import.Tatoeba;

internal sealed class Document : IDocument<DateOnly>
{
    public required DateOnly ArchiveKey { get; init; }
    public Dictionary<int, ExampleRow> Examples { get; init; }
    public Dictionary<(int, int), SegmentationRow> Segmentations { get; init; }
    public Dictionary<(int, int, int), TokenRow> Tokens { get; init; }

    public Document(int expectedExampleCount = 300_000)
    {
        Examples = new(expectedExampleCount);
        Segmentations = new(expectedExampleCount / 2);
        Tokens = new(expectedExampleCount * 4);
    }

    public IEnumerable<SequenceRow> GetSequences(int fileHeaderId)
        => Examples.Select(e => new SequenceRow(e.Key, fileHeaderId));

    public IEnumerable<int> ConcatAllExampleIds()
        => Examples.Keys
            .Concat(Segmentations.EntryIds())
            .Concat(Tokens.EntryIds());

    public IEnumerable<int> PriorityEntryIds()
        => Tokens.Values
            .Where(static token => token.IsPriority)
            .Select(static token => token.ExampleId);
}
