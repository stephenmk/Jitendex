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

using System.Collections.Immutable;

namespace Jitendex.Furigana.Models;

public sealed class Solution
{
    public required Entry Entry { get; init; }
    public required ImmutableArray<SolutionPart> Parts { get; init; }

    public override bool Equals(object? obj)
        => obj is Solution other
        && Entry.Equals(other.Entry)
        && Parts.SequenceEqual(other.Parts);

    public override int GetHashCode() => Parts.Aggregate
    (
        seed: Entry.GetHashCode(),
        func: static (hashcode, part) => HashCode.Combine(hashcode, part.GetHashCode())
    );
}
