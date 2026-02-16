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

internal sealed class CachedSolutionPartsGenerator(ResourceCache resourceCache) : ISolutionPartsGenerator
{
    public ImmutableArray<List<SolutionPart>> Enumerate(Entry entry, in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var texts = GetValidReadingTexts(entry, kanjiFormSlice, readingState);

        if (texts.Count == 0)
        {
            return [];
        }

        var baseText = kanjiFormSlice.RawText();
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

    private List<string> GetValidReadingTexts(Entry entry, in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var texts = GetReadingTexts(entry, kanjiFormSlice);
        var validTexts = new List<string>(texts.Count);
        foreach (var text in texts)
        {
            if (readingState.RemainingTextNormalized.StartsWith(text, StringComparison.Ordinal))
            {
                validTexts.Add(text);
            }
        }
        return validTexts;
    }

    private IReadOnlyList<string> GetReadingTexts(Entry entry, in KanjiFormSlice kanjiFormSlice)
    {
        if (kanjiFormSlice.Runes.Length == 1)
        {
            var rune = kanjiFormSlice.Runes[0];
            if (resourceCache.Characters.TryGetValue(rune.Value, out JapaneseCharacter? character))
            {
                return GetCharacterReadingTexts(entry, kanjiFormSlice, character);
            }
        }
        else
        {
            var text = kanjiFormSlice.Text();
            if (resourceCache.Compounds.TryGetValue(text, out JapaneseCompound? compound))
            {
                return compound.Readings;
            }
        }
        return [];
    }

    private static List<string> GetCharacterReadingTexts(Entry entry, in KanjiFormSlice kanjiFormSlice, JapaneseCharacter character)
    {
        var texts = new List<string>();

        if (entry is NameEntry)
        {
            foreach (var reading in character.NameReadings)
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
        }

        foreach (var reading in character.VocabReadings)
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
}
