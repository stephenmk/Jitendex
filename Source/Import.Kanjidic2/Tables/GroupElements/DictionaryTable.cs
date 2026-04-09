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
using Jitendex.Data.Kanjidic2.Entities.GroupItems;
using Jitendex.Import.Kanjidic2.TableRows;

namespace Jitendex.Import.Kanjidic2.Tables.GroupElements;

internal sealed class DictionaryTable : Table<DictionaryElement>
{
    protected override string Name { get; } = nameof(Dictionary);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Dictionary.UnicodeScalarValue),
        nameof(Dictionary.GroupOrder),
        nameof(Dictionary.Order),
        nameof(Dictionary.Text),
        nameof(Dictionary.TypeName),
        nameof(Dictionary.Volume),
        nameof(Dictionary.Page),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Dictionary.UnicodeScalarValue),
        nameof(Dictionary.GroupOrder),
        nameof(Dictionary.Order),
    ];

    protected override object?[] ParameterValues(DictionaryElement dictionary) =>
    [
        dictionary.EntryId,
        dictionary.GroupOrder,
        dictionary.Order,
        dictionary.Text,
        dictionary.TypeName,
        dictionary.Volume,
        dictionary.Page,
    ];
}
