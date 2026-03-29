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

using System.Text;
using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana.Internal;

internal sealed class Service(Knowledge knowledge, ImmutableArray<IterationSolver> solvers) : IFuriganaService
{
    public int AddCharacterReading(Rune character, string reading, bool isPrefix = false, bool isSuffix = false)
        => knowledge.AddCharacterReading(character, reading, isPrefix, isSuffix);

    public int AddNameReading(Rune kanji, string reading, bool isPrefix = false, bool isSuffix = false)
        => knowledge.AddNameReading(kanji, reading, isPrefix, isSuffix);

    public int AddHanziReading(Rune hanzi, string reading, bool isPrefix = false, bool isSuffix = false)
        => knowledge.AddHanziReading(hanzi, reading, isPrefix, isSuffix);

    public int AddHanjaReading(Rune hanja, string reading, bool isPrefix = false, bool isSuffix = false)
        => knowledge.AddHanjaReading(hanja, reading, isPrefix, isSuffix);

    public int AddCompoundReading(string compound, string reading, bool isPrefix = false, bool isSuffix = false)
        => knowledge.AddCompoundReading(compound, reading, isPrefix, isSuffix);

    public Solution? Solve(string text, string reading)
        => Solve(new Entry(text, reading, EntryType.Regular));

    public Solution? SolveName(string text, string reading)
        => Solve(new Entry(text, reading, EntryType.Name));

    public Solution? SolveChineseLoanword(string text, string reading)
        => Solve(new Entry(text, reading, EntryType.Chinese));

    public Solution? SolveKoreanLoanword(string text, string reading)
        => Solve(new Entry(text, reading, EntryType.Korean));

    private Solution? Solve(Entry entry)
    {
        foreach (var solver in solvers)
        {
            var solutions = solver.Solve(entry);
            if (solutions.Length == 1)
            {
                return solutions[0];
            }
        }
        return null;
    }
}
