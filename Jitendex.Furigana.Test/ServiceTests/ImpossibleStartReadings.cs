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

/// <summary>
/// Tests for unique and correct solutions when the length of the
/// kanji form text is unequal to the length of the reading text.
/// </summary>
/// <remarks>
/// Since readings of individual kanji cannot begin with 'っ', 'ょ', 'ゃ', 'ゅ', or 'ん',
/// we are able to solve these problems without any prior knowledge of usual kanji readings.
/// </remarks>
[TestClass]
public class ImpossibleStartReadings : ServiceTest
{
    private static readonly SolvableData _data =
    [
        // っ
        ("仏者", "ぶっしゃ", "[仏|ぶっ][者|しゃ]"),
        ("ご法度", "ごはっと", "ご[法|はっ][度|と]"),

        // ょ
        ("如意", "にょい", "[如|にょ][意|い]"),
        ("真如", "しんにょ", "[真|しん][如|にょ]"),

        // ゃ
        ("他社", "たしゃ", "[他|た][社|しゃ]"),
        ("三社", "さんしゃ", "[三|さん][社|しゃ]"),
        ("三輪車", "さんりんしゃ", "[三|さん][輪|りん][車|しゃ]"),
        ("不審者", "ふしんしゃ", "[不|ふ][審|しん][者|しゃ]"),

        // ゅ
        ("亜種", "あしゅ", "[亜|あ][種|しゅ]"),
        ("別種", "べっしゅ", "[別|べっ][種|しゅ]"),
        ("三鞭酒", "さんべんしゅ", "[三|さん][鞭|べん][酒|しゅ]"),

        // ん
        ("如何", "いかん", "[如|い][何|かん]"),
        ("阿呆陀羅", "あほんだら", "[阿|あ][呆|ほん][陀|だ][羅|ら]"),
        ("頑張る", "がんばる", "[頑|がん][張|ば]る"),
        ("真珠湾", "しんじゅわん", "[真|しん][珠|じゅ][湾|わん]"),

        // With two consecutive impossible-start characters
        ("勅勘", "ちょっかん", "[勅|ちょっ][勘|かん]"),

        // With kana following the kanji
        ("真さお", "まっさお", "[真|まっ]さお"),
        ("危機に瀕する", "ききにひんする", "[危|き][機|き]に[瀕|ひん]する"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        foreach (var (kanjiFormText, readingText, _) in _data)
        {
            Assert.AreNotEqual(kanjiFormText.Length, readingText.Length);
        }
        TestSolvable(_data);
    }
}
