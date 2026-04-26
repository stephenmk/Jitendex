// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, ServiceTest.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

global using SolvableData = System.Collections.Generic.IEnumerable<(string Text, string Reading, string Solution)>;
global using UnsolvableData = System.Collections.Generic.IEnumerable<(string Text, string Reading)>;
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
        foreach (var (text, reading, expectedSolutionText) in data)
        {
            TestSingleSolvable(text, reading, expectedSolutionText);
        }
    }

    protected void TestUnsolvable(UnsolvableData data)
    {
        foreach (var (text, reading) in data)
        {
            TestSingleUnsolvable(text, reading);
        }
    }

    private void TestSingleSolvable(string text, string reading, string expectedSolutionText)
    {
        var solution = Service.Solve(text, reading);
        Assert.IsNotNull(solution, $"\n\n{text}【{reading}】\n");

        var entry = new Entry(text, reading, EntryType.Regular);
        var expectedSolution = TextSolution.Parse(expectedSolutionText, entry);

        AssertSolutionsAreEqual(expectedSolution, solution);
    }

    private void TestSingleUnsolvable(string text, string reading)
    {
        var solution = Service.Solve(text, reading);
        Assert.IsNull(solution, $"\n\n{text}【{reading}】\n");
    }

    protected static void AssertSolutionsAreEqual(Solution expected, Solution actual)
    {
        Assert.AreEqual(expected.Text, actual.Text);
        Assert.AreEqual(expected.Reading, actual.Reading);
        CollectionAssert.AreEqual
        (
            expected.Parts.Select(static p => p.BaseText).ToArray(),
            actual.Parts.Select(static p => p.BaseText).ToArray()
        );
        CollectionAssert.AreEqual
        (
            expected.Parts.Select(static p => p.RubyText).ToArray(),
            actual.Parts.Select(static p => p.RubyText).ToArray()
        );
    }
}
