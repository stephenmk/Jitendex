/*
Copyright (c) 2026 Stephen Kraus
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
using Jitendex.HomeData.Entities.Furigana;

namespace Jitendex.HomeData.ImportExport.Furigana.Models;

internal sealed record CharacterReadingsObject
{
    public ImmutableArray<string> Kunyomi { get; init; } = [];
    public ImmutableArray<string> Onyomi { get; init; } = [];
    public ImmutableArray<string> Chinese { get; init; } = [];
    public ImmutableArray<string> Korean { get; init; } = [];
    public ImmutableArray<string> Kana { get; init; } = [];
    public ImmutableArray<string> Alphanumeric { get; init; } = [];
    public ImmutableArray<string> Symbol { get; init; } = [];
    public ImmutableArray<string> Unknown { get; init; } = [];

    public List<CharacterReadingRow> ToReadingRows(int characterValue)
    {
        var readingRows = new List<CharacterReadingRow>();

        readingRows.AddRange(Kunyomi.Select(x =>
            ToReadingRow(characterValue, x, CharacterReadingTypeId.Kunyomi)));
        readingRows.AddRange(Onyomi.Select(x =>
            ToReadingRow(characterValue, x, CharacterReadingTypeId.Onyomi)));
        readingRows.AddRange(Chinese.Select(x =>
            ToReadingRow(characterValue, x, CharacterReadingTypeId.Chinese)));
        readingRows.AddRange(Korean.Select(x =>
            ToReadingRow(characterValue, x, CharacterReadingTypeId.Korean)));
        readingRows.AddRange(Kana.Select(x =>
            ToReadingRow(characterValue, x, CharacterReadingTypeId.Kana)));
        readingRows.AddRange(Alphanumeric.Select(x =>
            ToReadingRow(characterValue, x, CharacterReadingTypeId.Alphanumeric)));
        readingRows.AddRange(Symbol.Select(x =>
            ToReadingRow(characterValue, x, CharacterReadingTypeId.Symbol)));
        readingRows.AddRange(Unknown.Select(x =>
            ToReadingRow(characterValue, x, CharacterReadingTypeId.Unknown)));

        return readingRows;
    }

    private static CharacterReadingRow ToReadingRow(int characterValue, string text, CharacterReadingTypeId readingType)
    {
        var split = text.Replace("-", "").Split('.');
        return new
        (
            CharacterValue: characterValue,
            Text: split[0],
            IsPrefix: text.EndsWith('-'),
            IsSuffix: text.StartsWith('-'),
            Okurigana: split.Length == 2 ? split[1] : null,
            ReadingTypeId: (int)readingType
        );
    }
}
