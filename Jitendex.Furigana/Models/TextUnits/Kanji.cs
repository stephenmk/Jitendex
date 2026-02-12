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
using Jitendex.Furigana.Models.TextUnits.Readings;

namespace Jitendex.Furigana.Models.TextUnits;

public sealed class Kanji : JapaneseCharacter
{
    public override ImmutableArray<CharacterReading> Readings { get; }

    public Kanji(Rune rune, IEnumerable<string> readings, IEnumerable<string> nanori) : base(rune)
    {
        Readings = readings
            .Select(ReadingFactory)
            .Concat(nanori.Select(text => new NameReading(this, text)))
            .Distinct()
            .ToImmutableArray();
    }

    private CharacterReading ReadingFactory(string text)
    {
        var hyphenlessText = text.Replace("-", string.Empty);
        var normalText = hyphenlessText.Replace(".", string.Empty);

        return (normalText.IsAllKatakana(), normalText.IsAllHiragana()) switch
        {
            (true, false) => new OnReading(this, text),

            (false, true) => (hyphenlessText.Split("."), normalText.VerbToMasuStem()) switch
            {
                ({ Length: 1 }, _) => new KunReading(this, text),
                ({ Length: 2 }, null) => new SuffixedKunReading(this, text),
                ({ Length: 2 }, not null) => new VerbKunReading(this, text),
                _ => throw new ArgumentException($"Reading `{text}` has too many '.' delimiters", nameof(text))
            },

            (true, true)
                => throw new ArgumentException("Reading must not be empty", nameof(text)),

            (false, false)
                => throw new ArgumentException($"Reading `{text}` must either be all hiragana or all katakana", nameof(text))
        };
    }
}
