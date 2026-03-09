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

internal sealed class RadicalNameTable : Table<RadicalNameElement>
{
    protected override string Name => nameof(RadicalName);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(RadicalName.UnicodeScalarValue),
        nameof(RadicalName.GroupOrder),
        nameof(RadicalName.Order),
        nameof(RadicalName.Text),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(RadicalName.UnicodeScalarValue),
        nameof(RadicalName.GroupOrder),
        nameof(RadicalName.Order),
    ];

    protected override SqliteParameter[] Parameters(RadicalNameElement radicalName) =>
    [
        new("@0", radicalName.EntryId),
        new("@1", radicalName.GroupOrder),
        new("@2", radicalName.Order),
        new("@3", radicalName.Text),
    ];
}
