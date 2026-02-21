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

namespace Jitendex.Furigana.Test.ServiceTests;

[TestClass]
public class NameKanji : ServiceTest
{
    private const string _kanjiFormText = "佐藤";
    private const string _readingText = "さとう";
    private const string _expectedSolutionText = "[佐|さ][藤|とう]";

    private static readonly Dictionary<string, string[]> _kanji = new()
    {
        ["佐"] = ["あ"],
        ["藤"] = ["あ"],
    };

    private static readonly Dictionary<string, string[]> _nameKanji = new()
    {
        ["佐"] = ["さ"],
        ["藤"] = ["とう"],
    };

    [TestMethod]
    public void TestSolvable()
    {
        AddCharacters(_kanji);
        AddNameKanji(_nameKanji);

        var solution = Service.SolveName(_kanjiFormText, _readingText);
        Assert.IsNotNull(solution);

        var nameEntry = new NameEntry(_kanjiFormText, _readingText);
        var expectedSolution = TextSolution.Parse(_expectedSolutionText, nameEntry);
        Assert.AreEqual(expectedSolution, solution);
    }

    [TestMethod]
    public void TestUnsolvable()
    {
        var solution = Service.Solve(_kanjiFormText, _readingText);
        Assert.IsNull(solution);
    }

    [TestMethod]
    public void TestUnsolvableWithBogusKanji()
    {
        AddCharacters(_kanji);
        var solution = Service.Solve(_kanjiFormText, _readingText);
        Assert.IsNull(solution);
    }
}
