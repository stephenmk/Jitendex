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
public class PathologicalCases : ServiceTest
{
    private static readonly UnsolvableData _unsolvableData =
    [
        // Despite having strings of equal length, kanji
        // readings cannot begin with っ, ょ, ゃ, ゅ, ん.
        ("馬鹿馬鹿", "っょゃゅ"),

        // Somebody forgot to split the kanji form text
        ("金棒引き・鉄棒引き", "かなぼうひき"),

        // Punctuation mismatch
        ("何ーー？", "なにー？"),

        // Sometimes ン is read as なん
        ("ン十年", "なんじゅうねん"),
    ];

    [TestMethod]
    public void TestUnsolvable()
    {
        TestUnsolvable(_unsolvableData);
    }
}
