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

global using SolvableData = System.Collections.Generic.IEnumerable<(string KanjiFormText, string ReadingText, string ExpectedSolutionText)>;
global using UnsolvableData = System.Collections.Generic.IEnumerable<(string KanjiFormText, string ReadingText)>;
using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana.Test;

public class ServiceTest
{
    protected IFuriganaService Service { get; } = FuriganaServiceProvider.GetFuriganaService();

    protected void AddCharacters(Dictionary<string, string[]> characters)
    {
        foreach (var (character, readings) in characters)
        {
            var rune = character.EnumerateRunes().First();
            foreach (var reading in readings)
            {
                Service.AddCharacterReading(rune, reading);
            }
        }
    }

    protected void AddNameKanji(Dictionary<string, string[]> nameKanji)
    {
        foreach (var (character, readings) in nameKanji)
        {
            var rune = character.EnumerateRunes().First();
            foreach (var reading in readings)
            {
                Service.AddNameReading(rune, reading);
            }
        }
    }

    protected void AddCompounds(Dictionary<string, string[]> nameKanji)
    {
        foreach (var (compound, readings) in nameKanji)
        {
            foreach (var reading in readings)
            {
                Service.AddCompoundReading(compound, reading);
            }
        }
    }

    protected void TestSolvable(SolvableData data)
    {
        foreach (var (kanjiFormText, readingText, expectedSolutionText) in data)
        {
            TestSingleSolvable(kanjiFormText, readingText, expectedSolutionText);
        }
    }

    protected void TestUnsolvable(UnsolvableData data)
    {
        foreach (var (kanjiFormText, readingText) in data)
        {
            TestSingleUnsolvable(kanjiFormText, readingText);
        }
    }

    private void TestSingleSolvable(string kanjiForm, string reading, string expectedSolutionText)
    {
        var solution = Service.Solve(kanjiForm, reading);
        Assert.IsNotNull(solution, $"\n\n{kanjiForm}【{reading}】\n");

        var entry = new Entry(kanjiForm, reading);
        var expectedSolution = TextSolution.Parse(expectedSolutionText, entry);
        Assert.AreEqual(expectedSolution, solution);
    }

    private void TestSingleUnsolvable(string kanjiForm, string reading)
    {
        var solution = Service.Solve(kanjiForm, reading);
        Assert.IsNull(solution, $"\n\n{kanjiForm}【{reading}】\n");
    }
}
