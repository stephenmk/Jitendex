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

namespace Jitendex.Furigana.Internal.SolutionGenerators;

internal sealed class DefaultSingleCharacterParts : DefaultCharacterParts
{
    public override ImmutableArray<List<SolutionPart>> Enumerate(in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var baseText = kanjiFormSlice.RawRunes.FastToString();
        var readings = DefaultSingleCharacterReadings(kanjiFormSlice, readingState);
        var partsBuilder = ImmutableArray.CreateBuilder<List<SolutionPart>>(readings.Length);
        foreach (var reading in readings)
        {
            if (baseText.IsKanaEquivalent(reading))
            {
                var part = new SolutionPart(baseText, null);
                partsBuilder.Add([part]);
            }
            else
            {
                var part = new SolutionPart
                (
                    BaseText: baseText,
                    Furigana: readingState.RemainingText[..reading.Length].ToString()
                );
                partsBuilder.Add([part]);
            }
        }
        return partsBuilder.MoveToImmutable();
    }

    private static ImmutableArray<string> DefaultSingleCharacterReadings(in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var currentRune = kanjiFormSlice.Runes[0];

        if (currentRune.IsKana())
        {
            if (readingState.FirstRemainingChar.IsKanaEquivalent((char)currentRune.Value))
            {
                return [currentRune.ToString()];
            }
        }

        if (kanjiFormSlice.PreviousRune.IsKanaOrDefault() && kanjiFormSlice.NextRune.IsKanaOrDefault())
        {
            var regexReading = RegexReading(kanjiFormSlice, readingState);
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
                readingsBuilder.Add(remainingText[..i].ToString());
            }
            return readingsBuilder.MoveToImmutable();
        }

        if (readingState.FirstRemainingChar == currentRune.Value)
        {
            return [readingState.FirstRemainingChar.ToString()];
        }

        return [];
    }

    private static bool IsImpossibleKanjiReadingFirst(char c) => c switch
    {
        'っ' or
        'ょ' or
        'ゃ' or
        'ゅ' or
        'ん' => true,
        _ => false
    };
}
