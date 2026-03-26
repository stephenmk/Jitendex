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

using Jitendex.Data;
using Jitendex.Data.KanjiVG.Entities;
using Jitendex.Import.KanjiVG.Models;

namespace Jitendex.Import.KanjiVG.Tables;

internal sealed class LookupTable<T> : Table<T> where T : ILookupElement
{
    protected override string Name { get; } = ElementNameToEntityName(typeof(T).Name);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(ILookup.Id),
        nameof(ILookup.Text),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(ILookup.Id)
    ];

    protected override object?[] ParameterValues(T lookup) =>
    [
        lookup.Id,
        lookup.Text,
    ];

    #pragma warning disable format
    private static string ElementNameToEntityName(string elementName) => elementName switch
    {
        nameof(VariantTypeElement)            => nameof(VariantType),
        nameof(CommentElement)                => nameof(Comment),
        nameof(ComponentGroupStyleElement)    => nameof(ComponentGroupStyle),
        nameof(StrokeNumberGroupStyleElement) => nameof(StrokeNumberGroupStyle),
        nameof(ComponentCharacterElement)     => nameof(ComponentCharacter),
        nameof(ComponentOriginalElement)      => nameof(ComponentOriginal),
        nameof(ComponentPositionElement)      => nameof(ComponentPosition),
        nameof(ComponentRadicalElement)       => nameof(ComponentRadical),
        nameof(ComponentPhonElement)          => nameof(ComponentPhon),
        nameof(StrokeTypeElement)             => nameof(StrokeType),
        _ => throw new ArgumentOutOfRangeException(nameof(elementName), $"Value: `{elementName}`")
    };
    #pragma warning restore format
}
