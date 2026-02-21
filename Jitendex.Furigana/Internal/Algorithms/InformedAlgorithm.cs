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

internal sealed class InformedAlgorithm(ReadingKnowledge cache) : IAlgorithm
{
    public ImmutableArray<ImmutableArray<Solution.Part>> Solve(Entry entry, in TextSlice textSlice, in ReadingState readingState)
    {
        var texts = GetValidReadingTexts(entry, textSlice, readingState);

        if (texts.Count == 0)
        {
            return [];
        }

        var baseText = textSlice.RawRunes.FastToString();
        var partsLists = ImmutableArray.CreateBuilder<ImmutableArray<Solution.Part>>(texts.Count);

        foreach (var text in texts)
        {
            var furigana = baseText.IsKanaEquivalent(text)
                ? null
                : readingState.RemainingText[..text.Length].ToString();

            var part = new Solution.Part(baseText, furigana);
            partsLists.Add([part]);
        }

        return partsLists.MoveToImmutable();
    }

    private HashSet<string> GetValidReadingTexts(Entry entry, in TextSlice textSlice, in ReadingState readingState)
    {
        var texts = GetCachedTexts(entry, textSlice);
        var validTexts = new HashSet<string>(texts.Count);
        foreach (var text in texts)
        {
            if (readingState.RemainingTextNormalized.StartsWith(text, StringComparison.Ordinal))
            {
                validTexts.Add(text);
            }
        }
        return validTexts;
    }

    private List<string> GetCachedTexts(Entry entry, in TextSlice textSlice)
        => textSlice.Runes switch
        {
            { Length: 1 } => GetCharacterTexts(entry, textSlice),
            _ => GetCompoundTexts(textSlice)
        };

    private List<string> GetCharacterTexts(Entry entry, in TextSlice textSlice)
        => entry switch
        {
            NameEntry => GetSpecialCharacterReadings(textSlice, cache.NameKanji),
            ChineseEntry => GetSpecialCharacterReadings(textSlice, cache.Hanzi),
            KoreanEntry => GetSpecialCharacterReadings(textSlice, cache.Hanja),
            Entry => GetCharacterReadings(textSlice),
        };

    private List<string> GetSpecialCharacterReadings(in TextSlice textSlice, Dictionary<int, List<string>> dictionary)
    {
        var characterReadings = GetCharacterReadings(textSlice);
        if (dictionary.TryGetValue(textSlice.Runes[0].Value, out var readings))
        {
            characterReadings.AddRange(readings);
        }
        return characterReadings;
    }

    private List<string> GetCharacterReadings(in TextSlice textSlice)
    {
        if (!cache.Characters.TryGetValue(textSlice.Runes[0].Value, out var readings))
        {
            return [];
        }

        var texts = new List<string>(readings.Count);

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

    private List<string> GetCompoundTexts(in TextSlice textSlice)
        => cache.Compounds.TryGetValue(textSlice.Runes.FastToString(), out var readings)
            ? readings
            : [];
}
