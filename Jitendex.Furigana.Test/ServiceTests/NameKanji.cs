/*
Copyright (c) 2025 Stephen Kraus
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
    private readonly IFuriganaSolver _solver;

    private const string _kanjiFormText = "佐藤";
    private const string _readingText = "さとう";
    private const string _expectedSolutionText = "[佐|さ][藤|とう]";

    public NameKanji()
    {
        var characters = ResourceMethods.NameKanji(new()
        {
            ["佐"] = (["あ"], ["さ"]),
            ["藤"] = (["あ"], ["とう"]),
        });
        _solver = FuriganaSolverProvider.GetFuriganaSolver();
        _solver.AddCharacters(characters);
    }

    [TestMethod]
    public void TestSolvable()
    {
        var nameSolution = _solver.SolveName(_kanjiFormText, _readingText);
        Assert.IsNotNull(nameSolution);

        var nameEntry = new NameEntry(_kanjiFormText, _readingText);
        var expectedSolution = TextSolution.Parse(_expectedSolutionText, nameEntry);
        Assert.AreEqual(expectedSolution, nameSolution);
    }

    [TestMethod]
    public void TestUnsolvable()
    {
        var vocabSolution = _solver.SolveVocab(_kanjiFormText, _readingText);
        Assert.IsNull(vocabSolution);
    }
}
