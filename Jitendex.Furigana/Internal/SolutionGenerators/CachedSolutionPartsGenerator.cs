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

internal sealed class CachedSolutionPartsGenerator(ResourceCache cache) : ISolutionPartsGenerator
{
    public ImmutableArray<List<SolutionPart>> Enumerate(Entry entry, in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var texts = GetValidReadingTexts(entry, kanjiFormSlice, readingState);

        if (texts.Count == 0)
        {
            return [];
        }

        var baseText = kanjiFormSlice.RawRunes.FastToString();
        var partsLists = ImmutableArray.CreateBuilder<List<SolutionPart>>(texts.Count);

        foreach (var text in texts)
        {
            var furigana = baseText.IsKanaEquivalent(text)
                ? null
                : readingState.RemainingText[..text.Length].ToString();

            var part = new SolutionPart(baseText, furigana);
            partsLists.Add([part]);
        }

        return partsLists.MoveToImmutable();
    }

    private HashSet<string> GetValidReadingTexts(Entry entry, in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var texts = GetCachedTexts(entry, kanjiFormSlice);
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

    private List<string> GetCachedTexts(Entry entry, in KanjiFormSlice kanjiFormSlice)
        => kanjiFormSlice.Runes switch
        {
            { Length: 1 } => GetCharacterTexts(entry, kanjiFormSlice),
            _ => GetCompoundTexts(kanjiFormSlice)
        };

    private List<string> GetCharacterTexts(Entry entry, in KanjiFormSlice kanjiFormSlice)
        => entry switch
        {
            NameEntry => GetSpecialCharacterReadings(kanjiFormSlice, cache.NameKanji),
            ChineseEntry => GetSpecialCharacterReadings(kanjiFormSlice, cache.Hanzi),
            KoreanEntry => GetSpecialCharacterReadings(kanjiFormSlice, cache.Hanja),
            Entry => GetCharacterReadings(kanjiFormSlice),
        };

    private List<string> GetSpecialCharacterReadings(in KanjiFormSlice kanjiFormSlice, Dictionary<int, List<string>> dictionary)
    {
        var characterReadings = GetCharacterReadings(kanjiFormSlice);
        if (dictionary.TryGetValue(kanjiFormSlice.Runes[0].Value, out var readings))
        {
            characterReadings.AddRange(readings);
        }
        return characterReadings;
    }

    private List<string> GetCharacterReadings(in KanjiFormSlice kanjiFormSlice)
    {
        if (!cache.Characters.TryGetValue(kanjiFormSlice.Runes[0].Value, out var readings))
        {
            return [];
        }

        var texts = new List<string>(readings.Count);

        foreach (var reading in readings)
        {
            if (reading.IsSuffix && kanjiFormSlice.ContainsFirstRune)
            {
                continue;
            }
            if (reading.IsPrefix && kanjiFormSlice.ContainsFinalRune)
            {
                continue;
            }
            texts.Add(reading.Text);
        }

        return texts;
    }

    private List<string> GetCompoundTexts(in KanjiFormSlice kanjiFormSlice)
        => cache.Compounds.TryGetValue(kanjiFormSlice.Runes.FastToString(), out var readings)
            ? readings
            : [];
}
