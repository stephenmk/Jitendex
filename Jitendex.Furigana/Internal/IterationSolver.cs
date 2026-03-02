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
using Jitendex.Furigana.Internal.Algorithms;

namespace Jitendex.Furigana.Internal;

internal sealed class IterationSolver(ImmutableArray<IAlgorithm> algorithms)
{
    public ReadOnlySpan<Solution> Solve(in Entry entry)
    {
        var possibleSolutions = FindPossibleSolutions(entry);
        var validSolutions = new Solution[possibleSolutions.Count];
        int i = 0;

        foreach (var possibleSolution in possibleSolutions)
        {
            if (possibleSolution.ToSolution(entry) is Solution solution)
            {
                validSolutions[i++] = solution;
            }
        }

        return validSolutions.AsSpan(0, i);
    }

    private List<SolutionBuilder> FindPossibleSolutions(in Entry entry)
    {
        var emptySolution = new SolutionBuilder([]); // Need an initial, empty solution to iterate upon.
        var solutions = new List<SolutionBuilder>() { emptySolution };

        for (int sliceStart = 0; sliceStart < entry.TextRunes.Length; sliceStart++)
        {
        BeginAlgorithmLoop:
            foreach (var algorithm in algorithms)
            {
                for (int sliceEnd = entry.TextRunes.Length; sliceStart < sliceEnd; sliceEnd--)
                {
                    var textSlice = new TextSlice(entry, sliceStart, sliceEnd);
                    var newSolutions = IterateSolutions(entry, textSlice, algorithm, solutions);
                    if (newSolutions.Count > 0)
                    {
                        sliceStart += sliceEnd - sliceStart;
                        solutions = newSolutions;
                        goto BeginAlgorithmLoop;
                    }
                }
            }
        }

        return solutions;
    }

    private static List<SolutionBuilder> IterateSolutions
    (
        in Entry entry,
        in TextSlice textSlice,
        IAlgorithm algorithm,
        List<SolutionBuilder> solutions
    )
    {
        var newSolutions = new List<SolutionBuilder>();

        foreach (var solution in solutions)
        {
            var readingState = new ReadingState(entry, solution.ReadingLength());

            foreach (var nextParts in algorithm.Solve(entry.Type, textSlice, readingState))
            {
                var newParts = solution.Parts.AddRange(nextParts);
                var newSolution = new SolutionBuilder(newParts);
                newSolutions.Add(newSolution);
            }
        }

        return newSolutions;
    }
}
