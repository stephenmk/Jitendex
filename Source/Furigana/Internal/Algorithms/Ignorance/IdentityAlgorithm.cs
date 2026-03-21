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

using System.Collections.Immutable;
using Jitendex.JapaneseTextUtils;
using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana.Internal.Algorithms.Ignorance;

/// <summary>
/// Returns a solution if the surface form slice is identical to the start of the remaining reading.
/// </summary>
/// <remarks>
/// This is a speed hack for entries containing long strings of kana.
/// It cannot solve anything that the other algorithms cannot solve.
/// </remarks>
internal sealed class IdentityAlgorithm : CharacterAlgorithm
{
    public override ImmutableArray<ImmutableArray<Solution.Part>> Solve(in TextSlice textSlice, in ReadingState readingState)
    {
        if (!textSlice.RawRunes.AreAllKana())
        {
            return [];
        }

        var length = textSlice.RawRunes.Length;
        var normalizedText = length < 100
            ? stackalloc char[length]
            : new char[length];

        int i = 0;
        foreach (var rune in textSlice.RawRunes)
        {
            var normalizedChar = ((char)rune.Value).KatakanaToHiragana();
            normalizedText[i++] = normalizedChar;
        }

        if (readingState.RemainingTextNormalized.StartsWith(normalizedText, StringComparison.Ordinal))
        {
            var baseText = textSlice.RawRunes.FastToString();
            return [[new Solution.Part(baseText, null)]];
        }

        return [];
    }
}
