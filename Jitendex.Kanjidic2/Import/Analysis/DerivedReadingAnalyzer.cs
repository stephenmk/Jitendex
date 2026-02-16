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

using System.Text;
using Jitendex.JapaneseTextUtils;
using Microsoft.Extensions.Logging;

namespace Jitendex.Kanjidic2.Import.Analysis;

internal partial class DerivedReadingAnalyzer(ILogger<DerivedReadingAnalyzer> logger, Kanjidic2Context context)
{
    private static readonly DerivedReadingTable DerivedReadingTable = new();
    private sealed record ReadingKey(int EntryId, int GroupOrder, int ReadingMeaningOrder, int ReadingOrder);
    private sealed record DerivedReadingData(string Text, bool IsPrefix, bool IsSuffix);

    public void Analyze()
    {
        var entries = context.Readings
            .Where(static r => r.TypeName == "ja_on" || r.TypeName == "ja_kun")
            .GroupBy(static r => r.UnicodeScalarValue)
            .Select(static g => new
            {
                Id = g.Key,
                Readings = g.Select(static r => new
                {
                    r.UnicodeScalarValue,
                    r.GroupOrder,
                    r.ReadingMeaningOrder,
                    r.Order,
                    r.Text,
                    IsKunyomi = r.TypeName == "ja_kun"
                })
            });

        var tableItems = new List<DerivedReadingElement>();

        foreach (var entry in entries)
        {
            var textToDerivedReadings = new Dictionary<string, List<DerivedReadingElement>>();
            var textToData = new Dictionary<string, HashSet<DerivedReadingData>>();
            foreach (var reading in entry.Readings)
            {
                if (textToDerivedReadings.ContainsKey(reading.Text))
                {
                    LogDuplicateReading(new(entry.Id), reading.Text);
                    continue;
                }
                var key = new ReadingKey(reading.UnicodeScalarValue, reading.GroupOrder, reading.ReadingMeaningOrder, reading.Order);
                textToDerivedReadings[reading.Text] = reading.IsKunyomi
                    ? GetDerivedKunReadings(reading.Text.KatakanaToHiragana(), key)
                    : GetDerivedOnReadings(reading.Text.KatakanaToHiragana(), key);

                textToData[reading.Text] = textToDerivedReadings[reading.Text]
                    .Select(static r => (r.IsPrefix, r.IsSuffix) switch
                    {
                        (true, true) => new List<DerivedReadingData> { new(r.Text, true, true) },
                        (true, false) => [new(r.Text, true, false), new(r.Text, true, true)],
                        (false, true) => [new(r.Text, false, true), new(r.Text, true, true)],
                        (false, false) => [new(r.Text, false, false), new(r.Text, false, true), new(r.Text, true, false), new(r.Text, true, true)],
                    })
                    .SelectMany(static x => x)
                    .ToHashSet();
            }
            while (SearchForRedundantText(textToData) is string redundantText)
            {
                LogRedundantReading(new(entry.Id), redundantText);
                textToDerivedReadings.Remove(redundantText);
                textToData.Remove(redundantText);
            }
            tableItems.AddRange(FilterReadings(textToDerivedReadings));
        }

        DerivedReadingTable.InsertItems(context, tableItems);
    }

    private List<DerivedReadingElement> FilterReadings(Dictionary<string, List<DerivedReadingElement>> textToDerivedReadings)
    {
        var filteredReadings = new List<DerivedReadingElement>();
        var usedReadingTexts = new HashSet<string>();
        foreach (var derivedReadings in textToDerivedReadings.Values)
        {
            var firstReading = derivedReadings[0];
            if (usedReadingTexts.Add(firstReading.Text))
            {
                filteredReadings.Add(firstReading);
            }
        }
        foreach (var derivedReadings in textToDerivedReadings.Values)
        {
            int order = 1;
            foreach (var derivedReading in derivedReadings[1..])
            {
                if (usedReadingTexts.Add(derivedReading.Text))
                {
                    filteredReadings.Add(derivedReading with { Order = order++ });
                }
            }
        }
        return filteredReadings;
    }

    private string? SearchForRedundantText(Dictionary<string, HashSet<DerivedReadingData>> textToData)
    {
        foreach (var (text, data) in textToData)
        {
            foreach (var (searchText, searchData) in textToData)
            {
                if (string.Equals(text, searchText, StringComparison.Ordinal))
                {
                    continue;
                }
                bool dataIsSubsetOfSearchData = true;
                foreach (var datum in data)
                {
                    if (!searchData.Contains(datum))
                    {
                        dataIsSubsetOfSearchData = false;
                        break;
                    }
                }
                if (dataIsSubsetOfSearchData)
                {
                    return text;
                }
            }
        }
        return null;
    }

    private List<DerivedReadingElement> GetDerivedKunReadings(ReadOnlySpan<char> text, ReadingKey key)
    {
        var strippedReading = StripHyphens(text);
        var normalizedReading = strippedReading.Text.KatakanaToHiragana();

        var split = normalizedReading.Split('.');
        if (split.Length == 1)
        {
            return GetDerivedKunStems(split[0], key, strippedReading.IsPrefix, strippedReading.IsSuffix);
        }
        else
        {
            return GetDerivedSuffixedKunReadings(split[0], split[1], key, strippedReading.IsPrefix, strippedReading.IsSuffix);
        }
    }

    private List<DerivedReadingElement> GetDerivedSuffixedKunReadings(string stem, string okurigana, ReadingKey key, bool isPrefix, bool isSuffix)
    {
        var readings = GetDerivedKunStems(stem, key, false, isSuffix);
        if ((stem + okurigana).VerbToMasuStem() is string masuStem)
        {
            readings.AddRange(GetDerivedKunStems(masuStem, key, isPrefix, isSuffix));
        }
        return readings;
    }

    private List<DerivedReadingElement> GetDerivedKunStems(string text, ReadingKey key, bool isPrefix, bool isSuffix)
    {
        var reading = new DerivedReadingElement
        (
            key.EntryId,
            key.GroupOrder,
            key.ReadingMeaningOrder,
            key.ReadingOrder,
            0,
            text,
            isPrefix,
            isSuffix,
            "kunyomi"
        );
        var readings = new List<DerivedReadingElement> { reading };

        if (!isSuffix)
        {
            foreach (var rendakuReading in text.ToRendakuForms())
            {
                readings.Add(new
                (
                    key.EntryId,
                    key.GroupOrder,
                    key.ReadingMeaningOrder,
                    key.ReadingOrder,
                    0,
                    rendakuReading,
                    IsPrefix: isPrefix,
                    IsSuffix: true,
                    "kunyomi-rendaku"
                ));
            }
        }

        return readings;
    }

    private List<DerivedReadingElement> GetDerivedOnReadings(ReadOnlySpan<char> text, ReadingKey key)
    {
        var readings = new List<DerivedReadingElement>();
        var strippedReading = StripHyphens(text);
        var normalizedReading = strippedReading.Text.KatakanaToHiragana();

        readings.Add(new
        (
            key.EntryId,
            key.GroupOrder,
            key.ReadingMeaningOrder,
            key.ReadingOrder,
            0,
            normalizedReading,
            strippedReading.IsPrefix,
            strippedReading.IsSuffix,
            "onyomi"
        ));

        if (!strippedReading.IsPrefix && normalizedReading.ToSokuonForm() is string sokuonReading)
        {
            readings.Add(new
            (
                key.EntryId,
                key.GroupOrder,
                key.ReadingMeaningOrder,
                key.ReadingOrder,
                0,
                sokuonReading,
                IsPrefix: true,
                strippedReading.IsSuffix,
                "onyomi-sokuon"
            ));
            if (!strippedReading.IsSuffix)
            {
                foreach (var rendakuReading in sokuonReading.ToRendakuForms())
                {
                    readings.Add(new
                    (
                        key.EntryId,
                        key.GroupOrder,
                        key.ReadingMeaningOrder,
                        key.ReadingOrder,
                        0,
                        rendakuReading,
                        IsPrefix: true,
                        IsSuffix: true,
                        "onyomi-sokuon-rendaku"
                    ));
                }
            }
        }

        if (!strippedReading.IsSuffix)
        {
            foreach (var rendakuReading in normalizedReading.ToRendakuForms())
            {
                readings.Add(new
                (
                    key.EntryId,
                    key.GroupOrder,
                    key.ReadingMeaningOrder,
                    key.ReadingOrder,
                    0,
                    rendakuReading,
                    strippedReading.IsPrefix,
                    IsSuffix: true,
                    "onyomi-rendaku"
                ));
            }
        }

        return readings;
    }

    private readonly ref struct StrippedReading
    {
        public readonly ReadOnlySpan<char> Text { get; init; }
        public readonly bool IsPrefix { get; init; }
        public readonly bool IsSuffix { get; init; }
    }

    private StrippedReading StripHyphens(ReadOnlySpan<char> text)
        => (text.EndsWith('-'), text.StartsWith('-')) switch
        {
            (true, true) => new() { Text = text[1..^1], IsPrefix = true, IsSuffix = true },
            (true, false) => new() { Text = text[..^1], IsPrefix = true, IsSuffix = false },
            (false, true) => new() { Text = text[1..], IsPrefix = false, IsSuffix = true },
            (false, false) => new() { Text = text, IsPrefix = false, IsSuffix = false },
        };

    [LoggerMessage(LogLevel.Warning,
    "Entry for character {rune} contains redundant reading `{Reading}`")]
    protected partial void LogDuplicateReading(Rune rune, string reading);

    [LoggerMessage(LogLevel.Warning,
    "Entry for character {rune} contains redundant reading `{Reading}`")]
    protected partial void LogRedundantReading(Rune rune, string reading);
}
