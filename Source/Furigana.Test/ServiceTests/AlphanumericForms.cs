/*
Copyright (c) 2026 Stephen Kraus
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
public class AlphanumericForms : ServiceTest
{
    private static readonly Dictionary<string, string[]> _kanji = new()
    {
        ["Ｄ"] = ["ディー"],
        ["３"] = ["スリー"],
    };

    private static readonly SolvableData _data =
    [
        ("３Ｄレンダリング", "スリーディーレンダリング", "[３|スリー][Ｄ|ディー]レンダリング"),
    ];

    private static readonly UnsolvableData _unsolvableData =
        _data.Select(static x => (x.Text, x.Reading));

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
