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
public class EqualLengthTexts : ServiceTest
{
    private static readonly SolvableData _data =
    [
        ("木の葉", "このは", "[木|こ]の[葉|は]"),
        ("こ之は", "このは", "こ[之|の]は"),
        ("ぜんまい仕かけ", "ぜんまいじかけ", "ぜんまい[仕|じ]かけ"),
        ("余所見", "よそみ", "[余|よ][所|そ][見|み]"),
        ("御坊っちゃん", "おぼっちゃん", "[御|お][坊|ぼ]っちゃん"),

        // Don't capture the impossible start characters (っ, ん, etc.) if not followed by a kanji
        ("真っさお", "まっさお", "[真|ま]っさお"),
        ("を呼んで", "をよんで", "を[呼|よ]んで"),
        ("田ん圃", "たんぼ", "[田|た]ん[圃|ぼ]"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        foreach (var (kanjiFormText, readingText, _) in _data)
        {
            Assert.AreEqual(kanjiFormText.Length, readingText.Length);
        }
        TestSolvable(_data);
    }
}
