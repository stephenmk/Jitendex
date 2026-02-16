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

internal readonly ref struct KanjiFormSlice
{
    public readonly ReadOnlySpan<Rune> PriorRunes;
    public readonly ReadOnlySpan<Rune> Runes;
    public readonly ReadOnlySpan<Rune> RemainingRunes;

    public readonly ReadOnlySpan<Rune> RawRunes;

    public readonly Rune PreviousRune;
    public readonly Rune NextRune;

    public readonly bool ContainsFirstRune;
    public readonly bool ContainsFinalRune;

    public string Text() => Runes.FastToString();
    public string RawText() => RawRunes.FastToString();

    public KanjiFormSlice(Entry entry, int sliceStart, int sliceEnd)
    {
        var normalizedRunesSpan = entry.NormalizedKanjiFormRunes.AsSpan();

        PriorRunes = normalizedRunesSpan[..sliceStart];
        Runes = normalizedRunesSpan[sliceStart..sliceEnd];
        RemainingRunes = normalizedRunesSpan[sliceEnd..];

        RawRunes = entry.KanjiFormRunes.AsSpan()[sliceStart..sliceEnd];

        PreviousRune = PriorRunes.Length > 0 ? PriorRunes[^1] : default;
        NextRune = RemainingRunes.Length > 0 ? RemainingRunes[0] : default;

        ContainsFirstRune = sliceStart == 0;
        ContainsFinalRune = sliceEnd == entry.KanjiFormRunes.Length;
    }
}
