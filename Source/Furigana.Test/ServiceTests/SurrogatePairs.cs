// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, SurrogatePairs.cs, is part of Jitendex.
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
public class SurrogatePairs : ServiceTest
{
    private static readonly SolvableData _data =
    [
        ("𩺊", "あら", "[𩺊|あら]"),

        // 1 furigana character
        ("𠮟かり", "しかり", "[𠮟|し]かり"),

        // 2 furigana characters
        ("しょう𤸎", "しょうかち", "しょう[𤸎|かち]"),

        // Repeated
        ("𩺊𩺊", "あらあら", "[𩺊|あら][𩺊|あら]"),
        ("𩺊々", "あらあら", "[𩺊|あら][々|あら]"),
    ];

    [TestMethod]
    public void TestSolvable()
    {
        foreach (var (text, _, solution) in _data)
        {
            Assert.IsTrue(text.Any(char.IsSurrogate));
            Assert.IsTrue(solution.Any(char.IsSurrogate));
        }
        TestSolvable(_data);
    }
}
