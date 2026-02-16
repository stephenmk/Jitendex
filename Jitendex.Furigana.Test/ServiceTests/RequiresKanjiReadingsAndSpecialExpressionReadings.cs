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
public class RequiresKanjiReadingsAndSpecialExpressions : ServiceTest
{
    private static readonly IEnumerable<JapaneseCharacter> _kanji = ResourceMethods.VocabKanji(new()
    {
        ["大"] = ["ダイ", "タイ", "おお"],
        ["和"] = ["ワ", "オ", "カ", "やわ", "なご", "あ"],
        ["魂"] = ["コン", "たましい", "たま"],
        ["風"] = ["フウ", "フ", "かぜ", "かざ"],
        ["邪"] = ["ジャ", "よこし"],
        ["薬"] = ["ヤク", "くすり"],
        ["純"] = ["ジュン"],
        ["日"] = ["ニチ", "ジツ", "ひ", "び", "か"],
        ["本"] = ["ホン", "もと"],
        ["学"] = ["ガク", "まな"],
        ["者"] = ["シャ", "もの"],
        ["製"] = ["セイ"],
        ["側"] = ["ソク", "かわ", "がわ", "そば"],
        ["刀"] = ["トウ", "かたな", "そり"],
        ["発"] = ["ハツ", "ホツ", "た", "あば", "おこ", "つか", "はな"],
        ["条"] = ["ジョウ", "チョウ", "デキ", "えだ", "すじ"],
        ["仕"] = ["シ", "ジ", "つか"],
        ["掛"] = ["カイ", "ケイ", "か", "が", "かかり", "がかり"],
    });

    private static readonly IEnumerable<JapaneseCompound> _compounds = ResourceMethods.Compounds(new()
    {
        ["日本"] = ["にほん"],
        ["大和"] = ["やまと"],
        ["風邪"] = ["かぜ"],
        ["発条"] = ["ぜんまい", "ばね"],
    });

    private static readonly SolvableData _data =
    [
        ("大和魂", "やまとだましい", "[大和|やまと][魂|だましい]"),
        ("風邪薬", "かぜぐすり", "[風邪|かぜ][薬|ぐすり]"),
        ("純日本風", "じゅんにほんふう", "[純|じゅん][日本|にほん][風|ふう]"),
        ("日本学者", "にほんがくしゃ", "[日本|にほん][学|がく][者|しゃ]"),
        ("日本製", "にほんせい", "[日本|にほん][製|せい]"),
        ("日本側", "にほんがわ", "[日本|にほん][側|がわ]"),
        ("日本刀", "にほんとう", "[日本|にほん][刀|とう]"),
        ("日本風", "にほんふう", "[日本|にほん][風|ふう]"),

        // 発条 has two different special readings
        ("発条仕掛け", "ぜんまいじかけ", "[発条|ぜんまい][仕|じ][掛|か]け"),
        ("発条仕掛け", "ばねじかけ", "[発条|ばね][仕|じ][掛|か]け"),

        // Here 発条 uses regular, kanji dictionary readings
        ("発条仕掛け", "はつじょうじかけ", "[発|はつ][条|じょう][仕|じ][掛|か]け"),

        // This is bogus data but it will solve because it's the correct length.
        ("発条仕掛け", "ああああけ", "[発|あ][条|あ][仕|あ][掛|あ]け"),

        // Repeat the above tests with non-normalized readings
        ("発条仕掛け", "ゼンマイじかけ", "[発条|ゼンマイ][仕|じ][掛|か]け"),
        ("発条仕掛け", "バねジカけ", "[発条|バね][仕|ジ][掛|カ]け"),
        ("発条仕掛け", "ハつじョうじカケ", "[発|ハつ][条|じョう][仕|じ][掛|カ]け"),
        ("発条仕掛け", "あアあアけ", "[発|あ][条|ア][仕|あ][掛|ア]け"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        var solver = FuriganaSolverProvider.GetFuriganaSolver();
        solver.AddCharacters(_kanji);
        solver.AddCompounds(_compounds);
        TestSolvable(solver, _data);
    }
}
