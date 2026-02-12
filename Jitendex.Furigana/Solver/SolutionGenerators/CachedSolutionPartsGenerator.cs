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
using Jitendex.Furigana.Models;
using Jitendex.Furigana.Models.TextUnits;
using Jitendex.Furigana.Models.TextUnits.Readings;

namespace Jitendex.Furigana.Solver.SolutionGenerators;

internal sealed class CachedSolutionPartsGenerator(ResourceCache resourceCache) : ISolutionPartsGenerator
{
    public ImmutableArray<List<SolutionPart>> Enumerate(in Entry entry, in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var textToReadings = GetValidReadings(entry, kanjiFormSlice, readingState);
        if (textToReadings.Count == 0) return [];

        var baseText = kanjiFormSlice.RawText();
        var @return = ImmutableArray.CreateBuilder<List<SolutionPart>>(textToReadings.Count);

        foreach (var (text, readings) in textToReadings)
        {
            if (baseText.IsKanaEquivalent(text))
            {
                @return.Add([new SolutionPart { BaseText = baseText }]);
            }
            else
            {
                @return.Add([new SolutionPart
                {
                    BaseText = baseText,
                    Furigana = readingState.RemainingText[..text.Length].ToString(),
                    Readings = [.. readings],
                }]);
            }
        }
        return @return.ToImmutableArray();
    }

    private Dictionary<string, List<IReading>> GetValidReadings(in Entry entry, in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var textToReadings = GetReadings(entry, kanjiFormSlice);
        var valid = new Dictionary<string, List<IReading>>(textToReadings.Count);
        foreach (var (text, readings) in textToReadings)
        {
            if (readingState.RemainingTextNormalized.StartsWith(text, StringComparison.Ordinal))
            {
                valid[text] = readings;
            }
        }
        return valid;
    }

    private Dictionary<string, List<IReading>> GetReadings(in Entry entry, in KanjiFormSlice kanjiFormSlice)
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
                return compound.Readings
                    .Select(static x => new KeyValuePair<string, List<IReading>>(x.Text, [x]))
                    .ToDictionary();
            }
        }
        return [];
    }

    private static Dictionary<string, List<IReading>> GetCharacterReadingTexts(in Entry entry, in KanjiFormSlice kanjiFormSlice, in JapaneseCharacter character)
    {
        var textToReadings = new Dictionary<string, List<IReading>>();

        foreach (var reading in character.Readings)
        {
            if (reading is NameReading && entry is not NameEntry)
            {
                continue;
            }
            if (reading.IsSuffix && kanjiFormSlice.ContainsFirstRune)
            {
                continue;
            }
            if (reading.IsPrefix && kanjiFormSlice.ContainsFinalRune)
            {
                continue;
            }
            foreach (var text in EnumerateReadingTexts(kanjiFormSlice, reading))
            {
                if (textToReadings.TryGetValue(text, out List<IReading>? readings))
                {
                    readings.Add(reading);
                }
                else
                {
                    textToReadings[text] = [reading];
                }
            }
        }

        return textToReadings;
    }

    /* TODO: These should be class methods */
    private static ImmutableArray<string> EnumerateReadingTexts(in KanjiFormSlice kanjiFormSlice, in CharacterReading characterReading) =>
        characterReading switch
        {
            NonKanjiReading nonKanjiReading => EnumerateNonKanjiReadingTexts(nonKanjiReading),
            NameReading nameReading => EnumerateNameReadingTexts(nameReading),
            OnReading onReading => EnumerateOnReadingTexts(kanjiFormSlice, onReading).ToImmutableArray(),
            VerbKunReading verbKunReading => EnumerateVerbKunReadingTexts(kanjiFormSlice, verbKunReading).ToImmutableArray(),
            SuffixedKunReading suffixedKunReading => EnumerateSuffixedKunReadingTexts(kanjiFormSlice, suffixedKunReading).ToImmutableArray(),
            KunReading kunReading => EnumerateKunReadingTexts(kanjiFormSlice, kunReading),
            _ => throw new NotImplementedException()
        };

    private static ImmutableArray<string> EnumerateNonKanjiReadingTexts(in NonKanjiReading nonKanjiReading)
    {
        return [nonKanjiReading.Text];
    }

    private static ImmutableArray<string> EnumerateNameReadingTexts(in NameReading nameReading)
    {
        return [nameReading.Text];
    }

    private static List<string> EnumerateOnReadingTexts(in KanjiFormSlice kanjiFormSlice, in OnReading onReading)
    {
        var @return = new List<string>
        {
            onReading.Text
        };

        if (!kanjiFormSlice.ContainsFirstRune)
        {
            foreach (var text in onReading.RendakuReadings)
            {
                @return.Add(text);
            }
        }

        if (!kanjiFormSlice.ContainsFinalRune && onReading.SokuonForm is not null)
        {
            @return.Add(onReading.SokuonForm);
        }

        if (!kanjiFormSlice.ContainsFirstRune && !kanjiFormSlice.ContainsFinalRune)
        {
            foreach (var text in onReading.RendakuSokuonReadings)
            {
                @return.Add(text);
            }
        }

        return @return;
    }

    private static ImmutableArray<string> EnumerateKunReadingTexts(in KanjiFormSlice kanjiFormSlice, in KunReading kunReading)
    {
        return GetStems(kanjiFormSlice, kunReading);
    }

    private static List<string> EnumerateSuffixedKunReadingTexts(in KanjiFormSlice kanjiFormSlice, in SuffixedKunReading kunReading)
    {
        var stems = GetStems(kanjiFormSlice, kunReading);
        var @return = new List<string>(stems);
        var remainingKanjiFormText = kanjiFormSlice.RemainingRunes.KatakanaToHiragana();

        for (int i = 0; i < kunReading.Suffix.Length; i++)
        {
            var remainingSuffix = kunReading.Suffix[i..];
            if (remainingKanjiFormText.StartsWith(remainingSuffix))
            {
                return @return;
            }

            foreach (var stem in stems)
            {
                @return.Add(stem + kunReading.Suffix[..(i + 1)]);
            }
        }

        return @return;
    }

    private static List<string> EnumerateVerbKunReadingTexts(in KanjiFormSlice kanjiFormSlice, VerbKunReading kunReading)
    {
        var stems = GetStems(kanjiFormSlice, kunReading);
        var @return = new List<string>(stems);
        var remainingKanjiFormText = kanjiFormSlice.RemainingRunes.KatakanaToHiragana();

        for (int i = 0; i < kunReading.Suffix.Length; i++)
        {
            var remainingSuffix = kunReading.Suffix[i..];
            if (remainingKanjiFormText.StartsWith(remainingSuffix))
            {
                return @return;
            }

            var remainingMasuSuffix = kunReading.MasuFormSuffix[i..];
            if (remainingKanjiFormText.StartsWith(remainingMasuSuffix))
            {
                return @return;
            }

            foreach (var stem in stems)
            {
                @return.Add(stem + kunReading.Suffix[..(i + 1)]);
            }
        }

        foreach (var stem in stems)
        {
            @return.Add(stem + kunReading.MasuFormSuffix);
        }

        return @return;
    }

    private static ImmutableArray<string> GetStems(in KanjiFormSlice kanjiFormSlice, in KunReading kunReading)
    {
        if (kanjiFormSlice.ContainsFirstRune)
        {
            return [kunReading.Text];
        }
        else
        {
            return kunReading.RendakuReadings.Add(kunReading.Text);
        }
    }
}
