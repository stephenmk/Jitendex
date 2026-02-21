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
    public Solution? Solve(string text, string reading);
    public Solution? SolveName(string text, string reading);
    public Solution? SolveChineseLoanword(string text, string reading);
    public Solution? SolveKoreanLoanword(string text, string reading);

    public void AddCharacterReading(Rune character, string reading, bool isPrefix = false, bool isSuffix = false);
    public void AddNameReading(Rune kanji, string reading);
    public void AddHanziReading(Rune hanzi, string reading);
    public void AddHanjaReading(Rune hanja, string reading);
    public void AddCompoundReading(string compound, string reading);
}
