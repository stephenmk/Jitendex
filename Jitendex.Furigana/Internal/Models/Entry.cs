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
using System.Text;
using Jitendex.JapaneseTextUtils;

namespace Jitendex.Furigana.Internal.Models;

internal record Entry
{
    public string Text { get; }
    public ImmutableArray<Rune> TextRunes { get; }
    public ImmutableArray<Rune> NormalizedTextRunes { get; }

    public string Reading { get; }
    public string NormalizedReading { get; }

    public Entry(string text, string reading)
    {
        if (reading.Any(char.IsSurrogate))
        {
            throw new ArgumentException
            (
                message: "Reading must not contain characters with surrogate code units.",
                paramName: nameof(reading)
            );
        }

        Text = text;
        TextRunes = [.. text.EnumerateRunes()];
        NormalizedTextRunes = [.. TextRunes.IterationMarksToKanji()];

        Reading = reading;
        NormalizedReading = reading.KatakanaToHiragana();
    }
}

internal sealed record NameEntry(string Text, string Reading) : Entry(Text, Reading);
internal sealed record ChineseEntry(string Text, string Reading) : Entry(Text, Reading);
internal sealed record KoreanEntry(string Text, string Reading) : Entry(Text, Reading);
