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

namespace Jitendex.Furigana.Internal.Algorithms;

internal sealed class InformedAlgorithm(IReadOnlyKnowledge cache) : IAlgorithm
{
    public ImmutableArray<ImmutableArray<Solution.Part>> Solve(EntryType entryType, in TextSlice textSlice, in ReadingState readingState)
    {
        var texts = GetValidReadingTexts(entryType, textSlice, readingState);

        if (texts.Length == 0)
        {
            return [];
        }

        var baseText = textSlice.RawRunes.FastToString();
        var partsLists = ImmutableArray.CreateBuilder<ImmutableArray<Solution.Part>>(texts.Length);

        foreach (var text in texts)
        {
            var furigana = baseText.IsKanaEquivalent(text)
                ? null
                : new string(readingState.RemainingText[..text.Length]);

            var part = new Solution.Part(baseText, furigana);
            partsLists.Add([part]);
        }

        return partsLists.MoveToImmutable();
    }

    private ReadOnlySpan<string> GetValidReadingTexts(EntryType entryType, in TextSlice textSlice, in ReadingState readingState)
    {
        var texts = GetCachedTexts(entryType, textSlice);
        if (texts.Count == 0)
        {
            return [];
        }
        Span<string> validTexts = new string[texts.Count];
        int validTextCount = 0;
        foreach (var text in texts)
        {
            if (readingState.RemainingTextNormalized.StartsWith(text, StringComparison.Ordinal))
            {
                validTexts[validTextCount++] = text;
            }
        }
        return validTexts[..validTextCount];
    }

    private HashSet<string> GetCachedTexts(EntryType entryType, in TextSlice textSlice)
        => textSlice.Runes switch
        {
            { Length: 1 } => GetCharacterTexts(entryType, textSlice),
                        _ => GetCompoundTexts(textSlice)
        };

    private HashSet<string> GetCharacterTexts(EntryType entryType, in TextSlice textSlice)
    {
        var rune = textSlice.Runes[0];

        var characterReadings = cache.GetCharacterReadings(rune);
        var specialReadings = entryType switch
        {
            EntryType.Regular => [],
            EntryType.Name    => cache.GetNameKanjiReadings(rune),
            EntryType.Chinese => cache.GetHanziReadings(rune),
            EntryType.Korean  => cache.GetHanjaReadings(rune),
                            _ => throw new ArgumentOutOfRangeException()
        };

        int readingCount = characterReadings.Count + specialReadings.Count;

        if (readingCount == 0)
        {
            return [];
        }

        var readings = characterReadings.Concat(specialReadings);

        return FilterReadings(textSlice, readings, readingCount);
    }

    private HashSet<string> GetCompoundTexts(in TextSlice textSlice)
    {
        var readings = cache.GetCompoundReadings(textSlice.Runes);
        return readings.Count == 0
            ? []
            : FilterReadings(textSlice, readings, readings.Count);
    }

    private static HashSet<string> FilterReadings(in TextSlice textSlice, IEnumerable<Reading> readings, int readingCount)
    {
        var texts = new HashSet<string>(readingCount);

        foreach (var reading in readings)
        {
            if (reading.IsSuffix && textSlice.ContainsFirstRune)
            {
                continue;
            }
            if (reading.IsPrefix && textSlice.ContainsFinalRune)
            {
                continue;
            }
            texts.Add(reading.Text);
        }

        return texts;
    }
}
