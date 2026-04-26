// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, VariantTable.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using Jitendex.Data;
using Jitendex.Data.Kanjidic2.Entities.GroupItems;
using Jitendex.Import.Kanjidic2.TableRows;

namespace Jitendex.Import.Kanjidic2.Tables.GroupElements;

internal sealed class VariantTable : Table<VariantElement>
{
    protected override string Name { get; } = nameof(Variant);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Variant.UnicodeScalarValue),
        nameof(Variant.GroupOrder),
        nameof(Variant.Order),
        nameof(Variant.Text),
        nameof(Variant.TypeName),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Variant.UnicodeScalarValue),
        nameof(Variant.GroupOrder),
        nameof(Variant.Order),
    ];

    protected override object?[] ParameterValues(VariantElement variant) =>
    [
        variant.EntryId,
        variant.GroupOrder,
        variant.Order,
        variant.Text,
        variant.TypeName,
    ];
}
