/*
Copyright (c) 2025 Stephen Kraus
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

namespace Jitendex.Furigana.Test;

internal static class ResourceMethods
{
    public static IEnumerable<JapaneseCharacter> VocabKanji(Dictionary<string, IEnumerable<string>> dataDictionary)
        => dataDictionary
            .Select(static item => new JapaneseCharacter
            (
                item.Key.EnumerateRunes().First(),
                item.Value.Select(static x => new CharacterReading(x.KatakanaToHiragana(), false, false)).ToImmutableArray(),
                []
            ));

    public static IEnumerable<JapaneseCharacter> NameKanji(Dictionary<string, (IEnumerable<string>, IEnumerable<string>)> dataDictionary)
        => dataDictionary
            .Select(static item => new JapaneseCharacter
            (
                item.Key.EnumerateRunes().First(),
                item.Value.Item1.Select(static x => new CharacterReading(x.KatakanaToHiragana(), false, false)).ToImmutableArray(),
                item.Value.Item2.Select(static x => new CharacterReading(x.KatakanaToHiragana(), false, false)).ToImmutableArray()
            ));

    public static IEnumerable<JapaneseCompound> Compounds(Dictionary<string, IEnumerable<string>> dataDictionary)
        => dataDictionary
            .Select(static item => new JapaneseCompound
            (
                item.Key,
                item.Value.ToImmutableArray()
            ));
}
