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

internal sealed class QueryCodeTable : Table<QueryCodeElement>
{
    protected override string Name => nameof(QueryCode);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(QueryCode.UnicodeScalarValue),
        nameof(QueryCode.GroupOrder),
        nameof(QueryCode.Order),
        nameof(QueryCode.Text),
        nameof(QueryCode.TypeName),
        nameof(QueryCode.Misclassification),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(QueryCode.UnicodeScalarValue),
        nameof(QueryCode.GroupOrder),
        nameof(QueryCode.Order),
    ];

    protected override SqliteParameter[] Parameters(QueryCodeElement queryCode) =>
    [
        new("@0", queryCode.EntryId),
        new("@1", queryCode.GroupOrder),
        new("@2", queryCode.Order),
        new("@3", queryCode.Text),
        new("@4", queryCode.TypeName),
        new("@5", queryCode.Misclassification.Nullable()),
    ];
}
