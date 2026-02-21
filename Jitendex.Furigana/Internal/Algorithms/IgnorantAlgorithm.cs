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
using Jitendex.Furigana.Internal.Models;
using Jitendex.Furigana.Internal.Algorithms.Ignorance;

namespace Jitendex.Furigana.Internal.Algorithms;

internal sealed class IgnorantAlgorithm(SingleCharacterAlgorithm single, RepeatedCharacterAlgorithm repeated) : IAlgorithm
{
    public ImmutableArray<List<Solution.Part>> Solve(Entry _, in TextSlice textSlice, in ReadingState readingState)
        => textSlice.Runes switch
        {
            { Length: 1 } => single.Solve(textSlice, readingState),
            { Length: 2 } => repeated.Solve(textSlice, readingState),
            _ => []
        };
}
