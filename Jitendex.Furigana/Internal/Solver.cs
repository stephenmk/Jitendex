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

using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana.Internal;

internal sealed class Solver(IterationSolver iterationSolver, ResourceCache cache) : IFuriganaSolver
{
    public Solution? SolveVocab(string kanjiFormText, string readingText)
        => Solve(new VocabEntry(kanjiFormText, readingText));

    public Solution? SolveName(string kanjiFormText, string readingText)
        => Solve(new NameEntry(kanjiFormText, readingText));

    private Solution? Solve(Entry entry)
    {
        var solutions = iterationSolver.Solve(entry).ToArray();

        if (solutions.Length == 1)
        {
            return solutions[0];
        }
        else
        {
            return null;
        }
    }

    public void AddCompounds(IEnumerable<JapaneseCompound> compounds)
    {
        foreach (var compound in compounds)
        {
            cache.Compounds.Add(compound.Text, compound);
        }
    }

    public void AddCharacters(IEnumerable<JapaneseCharacter> characters)
    {
        foreach (var character in characters)
        {
            cache.Characters.Add(character.Rune.Value, character);
        }
    }
}
