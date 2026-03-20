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
using Jitendex.Data;
using Jitendex.Data.Kanjidic2.Entities.Groups;
using Jitendex.Import.Kanjidic2.Models;

namespace Jitendex.Import.Kanjidic2.Tables.Groups;

internal sealed class QueryCodeGroupTable : Table<QueryCodeGroupElement>
{
    protected override string Name => nameof(QueryCodeGroup);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(QueryCodeGroup.UnicodeScalarValue),
        nameof(QueryCodeGroup.Order),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(QueryCodeGroup.UnicodeScalarValue),
        nameof(QueryCodeGroup.Order),
    ];

    protected override SqliteParameter[] Parameters(QueryCodeGroupElement group) =>
    [
        new("@0", group.EntryId),
        new("@1", group.Order),
    ];
}
