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

using Jitendex.Furigana.Internal.Algorithms.Ignorance;
using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana.Internal.Algorithms;

internal sealed class IgnorantAlgorithm
(
    SingleCharacterAlgorithm singleSolver,
    RepeatedKanjiAlgorithm repeatedSolver,
    IdentityAlgorithm identityAlgorithm,
    ConsecutiveKanjiAlgorithm? consecutiveSolver = null
) : IAlgorithm
{
    #pragma warning disable format
    public ImmutableArray<ImmutableArray<SolutionPart>> Solve(EntryType _, in TextSlice textSlice, in ReadingState readingState)
        => textSlice.Runes switch
        {
            { Length: 1 } => SolveOneRuneLengthText(textSlice, readingState),
            { Length: 2 } => SolveTwoRuneLengthText(textSlice, readingState),
            _             => SolveAnyRuneLengthText(textSlice, readingState),
        };
    #pragma warning restore format

    private ImmutableArray<ImmutableArray<SolutionPart>> SolveOneRuneLengthText(in TextSlice textSlice, in ReadingState readingState)
        => singleSolver.Solve(textSlice, readingState);

    private ImmutableArray<ImmutableArray<SolutionPart>> SolveTwoRuneLengthText(in TextSlice textSlice, in ReadingState readingState)
        => repeatedSolver.Solve(textSlice, readingState) is var parts and not []
            ? parts
            : SolveAnyRuneLengthText(textSlice, readingState);

    private ImmutableArray<ImmutableArray<SolutionPart>> SolveAnyRuneLengthText(in TextSlice textSlice, in ReadingState readingState)
        => identityAlgorithm.Solve(textSlice, readingState) is var parts and not []
            ? parts
            : consecutiveSolver?.Solve(textSlice, readingState) ?? [];
}
