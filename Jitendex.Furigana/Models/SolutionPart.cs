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
using Jitendex.Furigana.Models.TextUnits.Readings;

namespace Jitendex.Furigana.Models;

public sealed class SolutionPart
{
    public required string BaseText { get; init; }
    public string? Furigana { get; init; }
    public ImmutableArray<IReading> Readings { get; init; } = [];

    public override bool Equals(object? obj)
        => obj is SolutionPart part
        && string.Equals(BaseText, part.BaseText, StringComparison.Ordinal)
        && string.Equals(Furigana, part.Furigana, StringComparison.Ordinal);

    public override int GetHashCode()
        => HashCode.Combine(BaseText, Furigana);
}
