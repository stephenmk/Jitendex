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

namespace Jitendex.Furigana.Test.ServiceTests;

[TestClass]
public class RequiresSpecialExpressionReadings : ServiceTest
{
    private static readonly IEnumerable<JapaneseCompound> _compounds = ResourceMethods.Compounds(new()
    {
        ["発条"] = ["ぜんまい", "ばね"],
        ["芝生"] = ["しばふ"],
        ["草履"] = ["ぞうり"],
        ["竹刀"] = ["しない"],
        ["大人"] = ["おとな"],
        ["狗母魚"] = ["えそ"],
    });

    private static readonly SolvableData _data =
    [
        ("芝生", "しばふ", "[芝生|しばふ]"),
        ("草履", "ぞうり", "[草履|ぞうり]"),
        ("竹刀", "しない", "[竹刀|しない]"),
        ("大人の人", "おとなのひと", "[大人|おとな]の[人|ひと]"),

        // Three kanji, two furigana characters
        ("鯛なくば狗母魚", "たいなくばえそ", "[鯛|たい]なくば[狗母魚|えそ]"),

        // 発条 has two different special readings
        ("発条仕掛け", "ぜんまいじかけ", "[発条|ぜんまい][仕|じ][掛|か]け"),
        ("発条仕掛け", "ばねじかけ", "[発条|ばね][仕|じ][掛|か]け"),

        // This is bogus data but it will solve because it's the correct length.
        ("発条仕掛け", "ああああけ", "[発|あ][条|あ][仕|あ][掛|あ]け"),
    ];

    private static readonly UnsolvableData _unsolvableData =
    [
        ("発条仕掛け", "はつじょうじかけ"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        var solver = FuriganaSolverProvider.GetFuriganaSolver();
        solver.AddCompounds(_compounds);
        TestSolvable(solver, _data);
    }

    [TestMethod]
    public void TestUnsolvable()
    {
        var solver = FuriganaSolverProvider.GetFuriganaSolver();
        solver.AddCompounds(_compounds);
        TestUnsolvable(solver, _unsolvableData);
    }
}
