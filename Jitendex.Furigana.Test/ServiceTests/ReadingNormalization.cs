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
public class ReadingNormalization : ServiceTest
{
    private static readonly SolvableData _data =
    [
        // Entirely kana
        ("ア", "あ", "ア"),
        ("アイウエオ", "あいうえお", "アイウエオ"),
        ("あいうえお", "アイウエオ", "あいうえお"),
        ("あイうエお", "あいうえお", "あイうエお"),

        // Equal text lengths, normalized readings
        ("木ノ葉", "このは", "[木|こ]ノ[葉|は]"),
        ("木ノ葉", "きのは", "[木|き]ノ[葉|は]"),
        ("真ッさオ", "まっさお", "[真|ま]ッさオ"),
        ("田ン圃", "たんぼ", "[田|た]ン[圃|ぼ]"),

        // Equal text lengths, non-normalized readings
        ("木の葉", "コのは", "[木|コ]の[葉|は]"),
        ("余所見", "ヨそミ", "[余|ヨ][所|そ][見|ミ]"),
        ("真っサお", "マっさお", "[真|マ]っサお"),

        // Non-equal text lengths, normalized readings
        ("アリガタイ事ニ", "ありがたいことに", "アリガタイ[事|こと]ニ"),
        ("ありがたい事ニ", "ありがたいことに", "ありがたい[事|こと]ニ"),
        ("アリガタイ事に", "ありがたいことに", "アリガタイ[事|こと]に"),
        ("アリガタイ事ニ", "ありがたいことに", "アリガタイ[事|こと]ニ"),
        ("アりガたイ事ニ", "ありがたいことに", "アりガたイ[事|こと]ニ"),

        // Non-equal text lengths, non-normalized readings
        ("阿呆陀羅", "アほンだラ", "[阿|ア][呆|ほン][陀|だ][羅|ラ]"),
        ("嗹", "レン", "[嗹|レン]"),
        ("ありがたい事に", "ありがたいコトに", "ありがたい[事|コト]に"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        TestSolvable(_data);
    }
}
