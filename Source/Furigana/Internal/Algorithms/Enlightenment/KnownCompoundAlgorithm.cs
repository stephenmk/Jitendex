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

namespace Jitendex.Furigana.Internal.Algorithms.Enlightenment;

internal sealed class KnownCompoundAlgorithm(IReadOnlyKnowledge knowledge)
{
    public ImmutableArray<ImmutableArray<Solution.Part>> Solve(in TextSlice textSlice, in ReadingState readingState)
    {
        var texts = GetValidReadingTexts(textSlice, readingState);

        if (texts.Count == 0)
        {
            return [];
        }

        var baseText = textSlice.RawRunes.FastToString();
        var partsLists = ImmutableArray.CreateBuilder<ImmutableArray<Solution.Part>>(texts.Count);

        foreach (var (text, readingIds) in texts)
        {
            var furigana = baseText.IsKanaEquivalent(text)
                ? null
                : new string(readingState.RemainingText[..text.Length]);

            var part = new Solution.Part(baseText, furigana)
            {
                ReadingIds = ImmutableArray.Create(readingIds)
            };
            partsLists.Add([part]);
        }

        return partsLists.MoveToImmutable();
    }

    private Dictionary<string, int[]> GetValidReadingTexts(in TextSlice textSlice, in ReadingState readingState)
    {
        var texts = GetCompoundTexts(textSlice);
        if (texts.Count == 0)
        {
            return [];
        }
        int i = 0;
        var invalidKeys = new string[texts.Count];
        foreach (var key in texts.Keys)
        {
            if (!readingState.RemainingTextNormalized.StartsWith(key, StringComparison.Ordinal))
            {
                invalidKeys[i++] = key;
            }
        }
        foreach (var key in invalidKeys.AsSpan(0, i))
        {
            texts.Remove(key);
        }
        return texts;
    }

    private Dictionary<string, int[]> GetCompoundTexts(in TextSlice textSlice)
    {
        var readings = knowledge.GetCompoundReadings(textSlice.Runes);
        return readings.Count == 0
            ? []
            : FilterReadings(textSlice, readings, readings.Count);
    }

    private static Dictionary<string, int[]> FilterReadings(in TextSlice textSlice, IEnumerable<Reading> readings, int readingCount)
    {
        var texts = new Dictionary<string, int[]>(readingCount);

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
            if (texts.TryGetValue(reading.Text, out var readingIds))
            {
                texts[reading.Text] = [.. readingIds, reading.Id];
            }
            else
            {
                texts[reading.Text] = [reading.Id];
            }
        }

        return texts;
    }
}
