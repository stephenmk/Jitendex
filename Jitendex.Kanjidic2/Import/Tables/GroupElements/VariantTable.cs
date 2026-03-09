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

using Microsoft.Data.Sqlite;
using Jitendex.SQLite;
using Jitendex.Data.Kanjidic2.Entities.GroupItems;
using Jitendex.Kanjidic2.Import.Models;

namespace Jitendex.Kanjidic2.Import.Tables.GroupElements;

internal sealed class VariantTable : Table<VariantElement>
{
    protected override string Name => nameof(Variant);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Variant.UnicodeScalarValue),
        nameof(Variant.GroupOrder),
        nameof(Variant.Order),
        nameof(Variant.Text),
        nameof(Variant.TypeName),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Variant.UnicodeScalarValue),
        nameof(Variant.GroupOrder),
        nameof(Variant.Order),
    ];

    protected override SqliteParameter[] Parameters(VariantElement variant) =>
    [
        new("@0", variant.EntryId),
        new("@1", variant.GroupOrder),
        new("@2", variant.Order),
        new("@3", variant.Text),
        new("@4", variant.TypeName),
    ];
}
