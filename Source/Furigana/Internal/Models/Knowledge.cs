// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Knowledge.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using System.Text;
using Jitendex.JapaneseTextUtils;

namespace Jitendex.Furigana.Internal.Models;

internal sealed class Knowledge : IReadOnlyKnowledge
{
    private int NextId = 0;
    private const char CompoundPartSeparator = '|';
    private readonly Dictionary<string, List<ImmutableArray<Reading>>> Compounds = [];
    private readonly Dictionary<int, List<Reading>> Characters = [];
    private readonly Dictionary<int, List<Reading>> NameKanji = [];
    private readonly Dictionary<int, List<Reading>> Hanzi = [];
    private readonly Dictionary<int, List<Reading>> Hanja = [];

    public int AddCharacterReading(Rune character, string reading, bool isPrefix, bool isSuffix)
        => AddCharacterReading(character.Value, new Reading(NextId, reading, isPrefix, isSuffix), Characters);

    public int AddNameReading(Rune kanji, string reading, bool isPrefix, bool isSuffix)
        => AddCharacterReading(kanji.Value, new Reading(NextId, reading, isPrefix, isSuffix), NameKanji);

    public int AddHanziReading(Rune hanzi, string reading, bool isPrefix, bool isSuffix)
        => AddCharacterReading(hanzi.Value, new Reading(NextId, reading, isPrefix, isSuffix), Hanzi);

    public int AddHanjaReading(Rune hanja, string reading, bool isPrefix, bool isSuffix)
        => AddCharacterReading(hanja.Value, new Reading(NextId, reading, isPrefix, isSuffix), Hanja);

    private int AddCharacterReading(int key, Reading value, Dictionary<int, List<Reading>> dictionary)
    {
        if (dictionary.TryGetValue(key, out var readings))
        {
            readings.Add(value);
        }
        else
        {
            dictionary.Add(key, [value]);
        }
        return NextId++;
    }

    public int AddCompoundReading(string compound, string reading, bool isPrefix, bool isSuffix)
    {
        var readingParts = reading.Contains(CompoundPartSeparator)
            ? GetReadingParts(compound, reading, isPrefix, isSuffix)
            : [new Reading(NextId, reading, isPrefix, isSuffix)];

        if (Compounds.TryGetValue(compound, out var readings))
        {
            readings.Add(readingParts);
        }
        else
        {
            Compounds.Add(compound, [readingParts]);
        }
        return NextId++;
    }

    private ImmutableArray<Reading> GetReadingParts(string compound, string reading, bool isPrefix, bool isSuffix)
    {
        var split = reading.Split(CompoundPartSeparator);
        var runes = compound.EnumerateRunes().ToArray();
        if (runes.Length != split.Length)
        {
            throw new ArgumentException($"Partitioned reading `{reading}` must contain a part for each rune in `{compound}`");
        }
        var builder = ImmutableArray.CreateBuilder<Reading>(runes.Length);
        for (int i = 0; i < runes.Length; i++)
        {
            bool isPrefixPart = isPrefix && i == runes.Length - 1;
            bool isSuffixPart = isSuffix && i == 0;
            int id = Characters.TryGetValue(runes[i].Value, out var characterReadings)
                && characterReadings
                    .Where(r => r.Text.IsKanaEquivalent(split[i]))
                    .Where(r => r.IsPrefix == isPrefixPart)
                    .Where(r => r.IsSuffix == isSuffixPart)
                    .Select(static r => (int?)r.Id)
                    .FirstOrDefault() is int matchingReadingId
                ? matchingReadingId
                : NextId;

            var characterReading = new Reading(id, split[i], isPrefixPart, isSuffixPart);
            builder.Add(characterReading);
        }
        return builder.MoveToImmutable();
    }

    // This dictionary will be queried A LOT, so it's very worthwhile to avoid new memory allocations here.
    public IReadOnlyList<ImmutableArray<Reading>> GetCompoundReadings(ReadOnlySpan<Rune> runes)
    {
        var compoundLength = runes.SumUtf16SequenceLengths();
        var compound = compoundLength < 100
            ? stackalloc char[compoundLength]
            : new char[compoundLength];
        int charsWritten = 0;
        foreach (var rune in runes)
        {
            charsWritten += rune.EncodeToUtf16(compound[charsWritten..]);
        }
        var lookup = Compounds.GetAlternateLookup<ReadOnlySpan<char>>();
        return lookup.TryGetValue(compound, out var readings) ? readings : [];
    }

    public IReadOnlyList<Reading> GetCharacterReadings(Rune rune) => GetReadings(rune.Value, Characters);
    public IReadOnlyList<Reading> GetNameKanjiReadings(Rune rune) => GetReadings(rune.Value, NameKanji);
    public IReadOnlyList<Reading> GetHanziReadings(Rune rune) => GetReadings(rune.Value, Hanzi);
    public IReadOnlyList<Reading> GetHanjaReadings(Rune rune) => GetReadings(rune.Value, Hanja);

    private static IReadOnlyList<Reading> GetReadings(int key, Dictionary<int, List<Reading>> dictionary)
        => dictionary.TryGetValue(key, out var readings) ? readings : [];
}
