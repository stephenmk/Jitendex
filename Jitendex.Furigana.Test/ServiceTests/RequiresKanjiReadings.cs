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
public class RequiresKanjiReadings : ServiceTest
{
    private static readonly Dictionary<string, string[]> _kanji = new()
    {
        ["御"] = ["ギョ", "ゴ", "おん", "お", "み"],
        ["姉"] = ["シ", "あね", "はは", "ねえ"],
        ["母"] = ["ボ", "はは", "も", "かあ"],
        ["兄"] = ["ケイ", "キョウ", "あに", "にい"],
        ["東"] = ["トウ", "ひがし"],
        ["京"] = ["キョウ", "ケイ", "キン", "みやこ"],
        ["湾"] = ["ワン", "いりえ"],
        ["日"] = ["ニチ", "ジツ", "ひ", "び", "か"],
        ["独"] = ["ドク", "トク", "ひと"],
        ["協"] = ["キョウ"],
        ["会"] = ["カイ", "エ", "あ", "あつ"],
        ["可"] = ["カ", "コク", "べ"],
        ["能"] = ["ノウ", "よ", "あた"],
        ["津"] = ["シン", "つ"],
        ["波"] = ["ハ", "なみ"],
        ["問"] = ["モン", "と", "とん"],
        ["題"] = ["ダイ"],
        ["質"] = ["シツ", "シチ", "チ", "たち", "ただ", "もと", "わりふ"],
        ["乱"] = ["ラン", "ロン", "みだ", "おさ", "わた"],
        ["脈"] = ["ミャク", "すじ",],
        ["蝶"] = ["チョウ"],
        ["夫"] = ["フ", "フウ", "ブ", "おっと", "それ"],
        ["好"] = ["コウ", "この", "す", "よ", "い"],
        ["嫌"] = ["ケン", "ゲン", "きら", "いや"],
    };

    private static readonly SolvableData _data =
    [
        ("御姉さん", "おねえさん", "[御|お][姉|ねえ]さん"),
        ("御母さん", "おかあさん", "[御|お][母|かあ]さん"),
        ("御兄さん", "おにいさん", "[御|お][兄|にい]さん"),
        ("東京湾", "とうきょうわん", "[東|とう][京|きょう][湾|わん]"),
        ("日独協会", "にちどくきょうかい", "[日|にち][独|どく][協|きょう][会|かい]"),

        // Two characters, but no kanji repeats
        ("可能", "かのう", "[可|か][能|のう]"),
        ("津波", "つなみ", "[津|つ][波|なみ]"),
        ("問題", "もんだい", "[問|もん][題|だい]"),
        ("質問", "しつもん", "[質|しつ][問|もん]"),
        ("乱脈", "らんみゃく", "[乱|らん][脈|みゃく]"),

        // Despite being repeated kanji, the reading length is odd
        ("蝶蝶", "ちょうちょ", "[蝶|ちょう][蝶|ちょ]"),
        ("夫夫", "ふうふ", "[夫|ふう][夫|ふ]"),

        // Have to be especially careful with this one because
        // the solver can use both the stem 'す' and the inflected
        // masu-form 'すき' as possible readings for 好.
        // By checking for okurigana in the kanji form text,
        // we can limit the potential readings to just 'す'.
        ("好き嫌い", "すききらい", "[好|す]き[嫌|きら]い"),
    ];

    private static readonly UnsolvableData _unsolvableData =
        _data.Select(static x => (x.KanjiFormText, x.ReadingText));

    [TestMethod]
    public void TestSolvable()
    {
        AddCharacters(_kanji);
        TestSolvable(_data);
    }

    [TestMethod]
    public void TestUnsolvable()
    {
        TestUnsolvable(_unsolvableData);
    }
}
