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

namespace Jitendex.Furigana.Internal.Models;

internal sealed class Knowledge : IReadOnlyKnowledge
{
    public Dictionary<string, List<Reading>> Compounds { get; init; } = [];
    public Dictionary<int, List<Reading>> Characters { get; init; } = [];
    public Dictionary<int, List<Reading>> NameKanji { get; init; } = [];
    public Dictionary<int, List<Reading>> Hanzi { get; init; } = [];
    public Dictionary<int, List<Reading>> Hanja { get; init; } = [];

    /// <remarks>
    /// This dictionary will be queried A LOT, so it's very worthwhile to avoid new memory allocations here.
    /// </remarks>
    public IReadOnlyList<Reading> GetCompoundReadings(ReadOnlySpan<Rune> runes)
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
