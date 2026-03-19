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

internal sealed class SingleCharacterAlgorithm : CharacterAlgorithm
{
    public override ImmutableArray<ImmutableArray<Solution.Part>> Solve(in TextSlice textSlice, in ReadingState readingState)
    {
        var readings = DefaultSingleCharacterReadings(textSlice, readingState);

        if (readings.Length == 0)
        {
            return [];
        }

        var baseText = textSlice.RawRunes.FastToString();
        var partsBuilder = ImmutableArray.CreateBuilder<ImmutableArray<Solution.Part>>(readings.Length);
        foreach (var reading in readings)
        {
            var rubyText = baseText.IsKanaEquivalent(reading)
                ? null
                : new string(readingState.RemainingText[..reading.Length]);

            partsBuilder.Add([new(baseText, rubyText)]);
        }
        return partsBuilder.MoveToImmutable();
    }

    private static ImmutableArray<string> DefaultSingleCharacterReadings(in TextSlice textSlice, in ReadingState readingState)
    {
        var currentRune = textSlice.Runes[0];

        if (currentRune.IsKana())
        {
            return readingState.FirstRemainingChar.IsKanaEquivalent((char)currentRune.Value)
                ? [currentRune.ToString()]
                : [];
        }

        if (textSlice.PreviousRune.IsKanaOrDefault() && textSlice.NextRune.IsKanaOrDefault())
        {
            var regexReading = RegexReading(textSlice, readingState);
            if (regexReading is not null)
            {
                return [regexReading];
            }
        }

        if (currentRune.IsKanji())
        {
            if (IsImpossibleKanjiReadingFirst(readingState.FirstRemainingNormalizedChar))
            {
                return [];
            }
            var remainingText = readingState.RemainingText;
            var readingsBuilder = ImmutableArray.CreateBuilder<string>(remainingText.Length);
            for (int i = 1; i <= remainingText.Length; i++)
            {
                readingsBuilder.Add(new(remainingText[..i]));
            }
            return readingsBuilder.MoveToImmutable();
        }

        if (readingState.FirstRemainingChar == currentRune.Value)
        {
            return [readingState.FirstRemainingChar.ToString()];
        }

        return [];
    }
}
