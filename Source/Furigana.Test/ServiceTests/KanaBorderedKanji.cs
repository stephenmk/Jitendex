// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, KanaBorderedKanji.cs, is part of Jitendex.
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

/// <summary>
/// Tests problems in which a single kanji is bordered by either kana or nothing on both sides.
/// </summary>
/// <remarks>
/// All tests here require at least one kanji to have multiple furigana characters.
/// Tests for equal-length problems belong in the <see cref="EqualLengthTexts"/> class.
/// </remarks>
[TestClass]
public class KanaBorderedKanji : ServiceTest
{
    private static readonly SolvableData _data =
    [
        // Suffixed kanji
        ("難しい", "むずかしい", "[難|むずか]しい"),

        // Prefixed kanji
        ("ばね秤", "ばねばかり", "ばね[秤|ばかり]"),

        // Prefixed and suffixed kanji
        ("ありがたい事に", "ありがたいことに", "ありがたい[事|こと]に"),

        // Two kanji separated by a kana
        ("真っ青", "まっさお", "[真|ま]っ[青|さお]"),
        ("桜ん坊", "さくらんぼ", "[桜|さくら]ん[坊|ぼ]"),
        ("桜ん坊", "さくらんぼう", "[桜|さくら]ん[坊|ぼう]"),
        ("持ち運ぶ", "もちはこぶ", "[持|も]ち[運|はこ]ぶ"),
        ("弄り回す", "いじりまわす", "[弄|いじ]り[回|まわ]す"),
        ("掻っ攫う", "かっさらう", "[掻|か]っ[攫|さら]う"),

        // Two kanji separated by multiple kana
        ("険しい路", "けわしいみち", "[険|けわ]しい[路|みち]"),

        // Many kanji and kana separators
        ("毒を以て毒を制す", "どくをもってどくをせいす", "[毒|どく]を[以|もっ]て[毒|どく]を[制|せい]す"),

        // Note that "好き嫌い" isn't solvable by the default service,
        // but the "者が" delimiter here allows for a default solution.
        ("好き者が嫌い", "すきものがきらい", "[好|す]き[者|もの]が[嫌|きら]い"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        foreach (var (text, reading, _) in _data)
        {
            Assert.AreNotEqual(text.Length, reading.Length);
        }
        TestSolvable(_data);
    }
}
