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
using Jitendex.Furigana.Internal.SolutionGenerators;

namespace Jitendex.Furigana.Internal;

internal sealed class IterationSolver(ImmutableArray<ISolutionPartsGenerator> solutionPartsGenerators)
{
    public List<Solution> Solve(Entry entry)
    {
        var possibleSolutions = FindPossibleSolutions(entry);
        var validSolutions = new List<Solution>(possibleSolutions.Count);

        foreach (var possibleSolution in possibleSolutions)
        {
            var solution = possibleSolution.ToSolution(entry);
            if (solution is not null)
            {
                validSolutions.Add(solution);
            }
        }

        return validSolutions;
    }

    private List<SolutionBuilder> FindPossibleSolutions(Entry entry)
    {
        var solutions = new List<SolutionBuilder>() { new() };

        for (int sliceStart = 0; sliceStart < entry.KanjiFormRunes.Length; sliceStart++)
        {
        BeginGeneratorLoop:
            foreach (var solutionPartsGenerator in solutionPartsGenerators)
            {
                for (int sliceEnd = entry.KanjiFormRunes.Length; sliceStart < sliceEnd; sliceEnd--)
                {
                    var kanjiFormSlice = new KanjiFormSlice(entry, sliceStart, sliceEnd);
                    var newSolutions = IterateSolutions(solutionPartsGenerator, entry, kanjiFormSlice, solutions);
                    if (newSolutions.Count > 0)
                    {
                        sliceStart += sliceEnd - sliceStart;
                        solutions = newSolutions;
                        goto BeginGeneratorLoop;
                    }
                }
            }
        }

        return solutions;
    }

    private static List<SolutionBuilder> IterateSolutions
    (
        ISolutionPartsGenerator solutionPartsGenerator,
        Entry entry,
        in KanjiFormSlice kanjiFormSlice,
        List<SolutionBuilder> solutions
    )
    {
        var newSolutions = new List<SolutionBuilder>();

        foreach (var solution in solutions)
        {
            var readingState = new ReadingState(entry, solution.ReadingTextLength());

            foreach (var newParts in solutionPartsGenerator.Enumerate(entry, kanjiFormSlice, readingState))
            {
                var newSolution = new SolutionBuilder(solution.Parts.AddRange(newParts));
                newSolutions.Add(newSolution);
            }
        }

        return newSolutions;
    }
}
