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
using Jitendex.JMdictAnalysis.Entities.EntryItems.SenseItems;

namespace Jitendex.JMdictAnalysis.Import.Services.Tables;

internal sealed class ReadingRestrictionTable : Table<ReadingRestrictionRow>
{
    protected override string Name => nameof(ReadingRestriction);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(ReadingRestriction.EntryId),
        nameof(ReadingRestriction.SenseOrder),
        nameof(ReadingRestriction.Order),
        nameof(ReadingRestriction.ReadingOrder),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(ReadingRestriction.EntryId),
        nameof(ReadingRestriction.SenseOrder),
        nameof(ReadingRestriction.Order),
    ];

    protected override SqliteParameter[] Parameters(ReadingRestrictionRow update) =>
    [
        new("@0", update.EntryId),
        new("@1", update.SenseOrder),
        new("@2", update.Order),
        new("@3", update.ReadingOrder),
    ];
}
