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

using Jitendex.Furigana.Models;
using Jitendex.Furigana.Solver.SolutionGenerators;

namespace Jitendex.Furigana.Solver;

internal sealed class IterationSolver
{
    private readonly List<ISolutionPartsGenerator> _solutionPartsGenerators;

    public IterationSolver(List<ISolutionPartsGenerator> solutionPartsGenerators)
    {
        _solutionPartsGenerators = solutionPartsGenerators;
    }

    public IEnumerable<Solution> Solve(Entry entry)
    {
        var solutions = new List<SolutionBuilder>() { new() };

        for (int sliceStart = 0; sliceStart < entry.KanjiFormRunes.Length; sliceStart++)
        {
        BeginGeneratorLoop:
            foreach (var solutionPartsGenerator in _solutionPartsGenerators)
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

        foreach (var solutionBuilder in solutions)
        {
            var solution = solutionBuilder.ToSolution(entry);
            if (solution is not null)
            {
                yield return solution;
            }
        }
    }

    private static List<SolutionBuilder> IterateSolutions(
        in ISolutionPartsGenerator solutionPartsGenerator,
        in Entry entry,
        in KanjiFormSlice kanjiFormSlice,
        in List<SolutionBuilder> solutions)
    {
        var newSolutions = new List<SolutionBuilder>();

        foreach (var solution in solutions)
        {
            var solutionParts = solution.ToParts();
            var readingState = new ReadingState(entry, solution.ReadingTextLength());

            foreach (var newParts in solutionPartsGenerator.Enumerate(entry, kanjiFormSlice, readingState))
            {
                newSolutions.Add
                (
                    new(solutionParts.AddRange(newParts))
                );
            }
        }

        return newSolutions;
    }
}
