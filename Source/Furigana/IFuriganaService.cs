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

using System.Text;

namespace Jitendex.Furigana;

public interface IFuriganaService
{
    Solution? Solve(string text, string reading);
    Solution? SolveName(string text, string reading);
    Solution? SolveChineseLoanword(string text, string reading);
    Solution? SolveKoreanLoanword(string text, string reading);

    int AddCharacterReading(Rune character, string reading, bool isPrefix = false, bool isSuffix = false);
    int AddNameReading(Rune kanji, string reading, bool isPrefix = false, bool isSuffix = false);
    int AddHanziReading(Rune hanzi, string reading, bool isPrefix = false, bool isSuffix = false);
    int AddHanjaReading(Rune hanja, string reading, bool isPrefix = false, bool isSuffix = false);
    int AddCompoundReading(string compound, string reading, bool isPrefix = false, bool isSuffix = false);
}
