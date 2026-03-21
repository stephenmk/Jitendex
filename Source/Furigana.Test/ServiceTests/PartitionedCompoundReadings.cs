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

namespace Jitendex.Furigana.Test.ServiceTests;

[TestClass]
public class PartitionedCompoundReadings : ServiceTest
{
    private static readonly Dictionary<string, string[]> _kanji = new()
    {
        ["純"] = ["ジュン"],
        ["日"] = ["ニチ", "ジツ", "ひ", "び", "か"],
        ["本"] = ["ホン", "もと"],
        ["風"] = ["フウ", "フ", "かぜ", "かざ"],
        ["十"] = ["じゅう"],
    };

    private static readonly Dictionary<string, string[]> _compounds = new()
    {
        ["日本"] = ["に|ほん"],
        ["釈迦"] = ["しゃ|か"],
        ["対手"] = ["あい|て"],
    };

    private static readonly SolvableData _data =
    [
        ("純日本風", "じゅんにほんふう", "[純|じゅん][日|に][本|ほん][風|ふう]"),
        ("日本製", "にほんせい", "[日|に][本|ほん][製|せい]"),
        ("日本側", "にほんがわ", "[日|に][本|ほん][側|がわ]"),
        ("日本刀", "にほんとう", "[日|に][本|ほん][刀|とう]"),
        ("日本風", "にほんふう", "[日|に][本|ほん][風|ふう]"),
        ("釈迦十", "しゃかじゅう", "[釈|しゃ][迦|か][十|じゅう]"),
    ];

    private static readonly SolvableData _defaultData =
    [
        ("純日本風", "じゅんにほんふう", "[純日本風|じゅんにほんふう]"),
        ("日本製", "にほんせい", "[日本製|にほんせい]"),
        ("日本側", "にほんがわ", "[日本側|にほんがわ]"),
        ("日本刀", "にほんとう", "[日本刀|にほんとう]"),
        ("日本風", "にほんふう", "[日本風|にほんふう]"),
        ("釈迦十", "しゃかじゅう", "[釈迦十|しゃかじゅう]"),
    ];

    [TestMethod]
    public void TestCompoundSolvable()
    {
        AddCompounds(_compounds);
        TestSolvable(_data);
    }

    [TestMethod]
    public void TestKanjiSolvable()
    {
        AddCharacters(_kanji);
        TestSolvable(_data);
    }

    [TestMethod]
    public void TestDefaultSolvable()
    {
        TestSolvable(_defaultData);
    }
}
