// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, AlphanumericForms.cs, is part of Jitendex.
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

namespace Jitendex.Furigana.Test.ServiceTests;

[TestClass]
public class AlphanumericForms : ServiceTest
{
    private static readonly Dictionary<string, string[]> _kanji = new()
    {
        ["Ｄ"] = ["ディー"],
        ["Ｆ"] = ["エフ"],
        ["Ｍ"] = ["エム"],
        ["Ｎ"] = ["エヌ", "エン"],
        ["３"] = ["スリー"],
    };

    private static readonly SolvableData _solvable =
    [
        ("ＡＤＳＬ", "エー・ディー・エス・エル", "[ＡＤＳＬ|エー・ディー・エス・エル]"),
    ];

    private static readonly SolvableData _solvableWithKanji =
    [
        ("ＡＤＳＬ", "エー・ディー・エス・エル", "[ＡＤＳＬ|エー・ディー・エス・エル]"),
        ("３Ｄレンダリング", "スリーディーレンダリング", "[３|スリー][Ｄ|ディー]レンダリング"),
    ];

    private static readonly UnsolvableData _unsolvable =
    [
        ("３Ｄレンダリング", "スリーディーレンダリング"),
        ("ＭＦＮ", "エム・エフ・エン"),
    ];

    private static readonly UnsolvableData _unsolvableWithKanji =
    [
        ("ＭＦＮ", "エム・エフ・エン"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        TestSolvable(_solvable);
    }

    [TestMethod]
    public void TestSolvableWithKanji()
    {
        AddCharacters(_kanji);
        TestSolvable(_solvableWithKanji);
    }

    [TestMethod]
    public void TestUnsolvable()
    {
        TestUnsolvable(_unsolvable);
    }

    [TestMethod]
    public void TestUnsolvableWithKanji()
    {
        AddCharacters(_kanji);
        TestUnsolvable(_unsolvableWithKanji);
    }
}
