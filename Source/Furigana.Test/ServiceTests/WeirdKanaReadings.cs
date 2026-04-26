// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, WeirdKanaReadings.cs, is part of Jitendex.
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
public class WeirdKanaReadings : ServiceTest
{
    private static readonly Dictionary<string, string[]> _kanji = new()
    {
        ["一"] = ["イチ", "イツ", "ひと"],
        ["ヶ"] = ["か", "が"],
        ["ヵ"] = ["か", "が"],
        ["ケ"] = ["か", "が"],
        ["月"] = ["ゲツ", "ガツ", "つき"],
    };

    private static readonly SolvableData _data =
    [
        ("一ヶ月", "いっかげつ", "[一|いっ][ヶ|か][月|げつ]"),
        ("一ヵ月", "いっかげつ", "[一|いっ][ヵ|か][月|げつ]"),
        ("一ケ月", "いっかげつ", "[一|いっ][ケ|か][月|げつ]"),

        ("一ケ月", "いっけげつ", "[一|いっ]ケ[月|げつ]"),
        ("一ケ月", "いっケげつ", "[一|いっ]ケ[月|げつ]"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        AddCharacters(_kanji);
        TestSolvable(_data);
    }
}
