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

namespace Jitendex.Furigana.Internal.Models;

internal readonly ref struct Entry
{
    public ReadOnlySpan<char> Text { get; }
    public ReadOnlySpan<Rune> TextRunes { get; }
    public ReadOnlySpan<Rune> NormalizedTextRunes { get; }

    public ReadOnlySpan<char> Reading { get; }
    public ReadOnlySpan<char> NormalizedReading { get; }

    public EntryType Type { get; }

    public Entry(ReadOnlySpan<char> text, ReadOnlySpan<char> reading, EntryType type)
    {
        if (ContainsSurrogate(reading))
        {
            throw new ArgumentException
            (
                message: "Reading must not contain characters with surrogate code units.",
                paramName: nameof(reading)
            );
        }

        Text = text;
        TextRunes = ConvertToRunes(text);
        NormalizedTextRunes = TextRunes.IterationMarksToKanji();

        Reading = reading;
        NormalizedReading = reading.KatakanaToHiragana();

        Type = type;
    }

    private static bool ContainsSurrogate(ReadOnlySpan<char> characters)
    {
        foreach (var character in characters)
        {
            if (char.IsSurrogate(character))
            {
                return true;
            }
        }
        return false;
    }

    private static ReadOnlySpan<Rune> ConvertToRunes(ReadOnlySpan<char> characters)
    {
        var runes = new Rune[characters.Length]; // Length is always > or = to number of runes.
        int runeCount = 0;
        foreach (var rune in characters.EnumerateRunes())
        {
            runes[runeCount++] = rune;
        }
        return runes.AsSpan(0, runeCount);
    }
}
