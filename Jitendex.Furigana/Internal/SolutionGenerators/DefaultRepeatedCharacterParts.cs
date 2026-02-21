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

internal sealed class DefaultRepeatedCharacterParts : DefaultCharacterParts
{
    public override ImmutableArray<List<Solution.Part>> Enumerate(in KanjiFormSlice kanjiFormSlice, in ReadingState readingState)
    {
        var currentRune1 = kanjiFormSlice.Runes[0];
        var currentRune2 = kanjiFormSlice.Runes[1];

        if (!currentRune1.IsKanji() || currentRune1 != currentRune2)
        {
            return [];
        }

        if (!kanjiFormSlice.PreviousRune.IsKanaOrDefault() || !kanjiFormSlice.NextRune.IsKanaOrDefault())
        {
            return [];
        }

        var reading = RegexReading(kanjiFormSlice, readingState);

        if (reading is null || reading.Length % 2 != 0)
        {
            return [];
        }

        int halfLength = reading.Length / 2;

        return
        [[
            new Solution.Part
            (
                BaseText: kanjiFormSlice.RawRunes[0].ToString(),
                RubyText: reading[..halfLength]
            ),
            new Solution.Part
            (
                BaseText: kanjiFormSlice.RawRunes[1].ToString(),
                RubyText: reading[halfLength..]
            )
        ]];
    }
}
