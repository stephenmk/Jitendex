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

internal sealed class RepeatedKanjiAlgorithm : CharacterAlgorithm
{
    public override ImmutableArray<ImmutableArray<Solution.Part>> Solve(in TextSlice textSlice, in ReadingState readingState)
    {
        if (!IsValidTextSlice(textSlice))
        {
            return [];
        }

        var reading = RegexReading(textSlice, readingState);

        if (reading is null || reading.Length % 2 != 0)
        {
            return [];
        }

        int halfLength = reading.Length / 2;
        var reading1 = reading.AsSpan(0, halfLength);
        var reading2 = reading.AsSpan(halfLength, halfLength);

        if (!IsValidReadingPair(reading1, reading2))
        {
            return [];
        }

        return
        [[
            new Solution.Part(textSlice.RawRunes[0].ToString(), new(reading1)),
            new Solution.Part(textSlice.RawRunes[1].ToString(), new(reading2)),
        ]];
    }

    private bool IsValidTextSlice(in TextSlice textSlice)
    {
        var currentRune1 = textSlice.Runes[0];
        var currentRune2 = textSlice.Runes[1];

        if (!currentRune1.IsKanji() || currentRune1 != currentRune2)
        {
            return false;
        }

        if (!textSlice.PreviousRune.IsKanaOrDefault() || !textSlice.NextRune.IsKanaOrDefault())
        {
            return false;
        }

        return true;
    }

    private bool IsValidReadingPair(ReadOnlySpan<char> reading1, ReadOnlySpan<char> reading2)
    {
        if (reading1.IsKanaEquivalent(reading2))
        {
            return true;
        }

        foreach (var rendakuForm in reading1.ToRendakuForms())
        {
            if (rendakuForm.IsKanaEquivalent(reading2))
            {
                return true;
            }
        }

        return false;
    }
}
