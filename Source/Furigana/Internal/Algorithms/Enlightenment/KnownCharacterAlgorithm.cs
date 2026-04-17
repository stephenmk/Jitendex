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

using Jitendex.Furigana.Internal.Models;
using Jitendex.JapaneseTextUtils;

namespace Jitendex.Furigana.Internal.Algorithms.Enlightenment;

internal sealed class KnownCharacterAlgorithm(IReadOnlyKnowledge knowledge)
{
    public ImmutableArray<ImmutableArray<SolutionPart>> Solve(EntryType entryType, in TextSlice textSlice, in ReadingState readingState)
    {
        var texts = GetValidReadingTexts(entryType, textSlice, readingState);

        if (texts.Count == 0)
        {
            return [];
        }

        var baseText = textSlice.RawRunes.FastToString();
        var partsLists = ImmutableArray.CreateBuilder<ImmutableArray<SolutionPart>>(texts.Count);

        foreach (var (text, readingIds) in texts)
        {
            var furigana = baseText.IsKanaEquivalent(text)
                ? null
                : new string(readingState.RemainingText[..text.Length]);

            var part = new SolutionPart(baseText, furigana)
            {
                ReadingIds = ImmutableArray.Create(readingIds)
            };
            partsLists.Add([part]);
        }

        return partsLists.MoveToImmutable();
    }

    private Dictionary<string, int[]> GetValidReadingTexts(EntryType entryType, in TextSlice textSlice, in ReadingState readingState)
    {
        var texts = GetReadingTexts(entryType, textSlice);
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
        foreach (var key in invalidKeys.AsSpan(..i))
        {
            texts.Remove(key);
        }
        return texts;
    }

    private Dictionary<string, int[]> GetReadingTexts(EntryType entryType, in TextSlice textSlice)
    {
        var rune = textSlice.Runes[0];
        var characterReadings = knowledge.GetCharacterReadings(rune);

        #pragma warning disable format
        var specialReadings = entryType switch
        {
            EntryType.Regular => [],
            EntryType.Name    => knowledge.GetNameKanjiReadings(rune),
            EntryType.Chinese => knowledge.GetHanziReadings(rune),
            EntryType.Korean  => knowledge.GetHanjaReadings(rune),
            _                 => throw new ArgumentOutOfRangeException()
        };
        #pragma warning restore format

        int readingCount = characterReadings.Count + specialReadings.Count;

        if (readingCount == 0)
        {
            return [];
        }

        var readings = characterReadings.Concat(specialReadings);

        return FilterReadings(textSlice, readings, readingCount);
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
