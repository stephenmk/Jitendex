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
using Jitendex.Data.JMdict.Entities.EntryItems.SenseItems;
using Jitendex.Import.JMdict.Models;

namespace Jitendex.Import.JMdict.Tables.EntryElements.SenseElements;

internal sealed class DialectTable : Table<DialectElement>
{
    protected override string Name => nameof(Dialect);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Dialect.EntryId),
        nameof(Dialect.SenseOrder),
        nameof(Dialect.Order),
        nameof(Dialect.TagName),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Dialect.EntryId),
        nameof(Dialect.SenseOrder),
        nameof(Dialect.Order),
    ];

    protected override SqliteParameter[] Parameters(DialectElement dialect) =>
    [
        new("@0", dialect.EntryId),
        new("@1", dialect.ParentOrder),
        new("@2", dialect.Order),
        new("@3", dialect.TagName),
    ];
}
