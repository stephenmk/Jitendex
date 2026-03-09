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
using Jitendex.Data.JMdict.Entities.EntryItems.ReadingItems;
using Jitendex.JMdict.Import.Models;

namespace Jitendex.JMdict.Import.Tables.EntryElements.ReadingElements;

internal sealed class ReadingPriorityTable : Table<ReadingPriorityElement>
{
    protected override string Name => nameof(ReadingPriority);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(ReadingPriority.EntryId),
        nameof(ReadingPriority.ReadingOrder),
        nameof(ReadingPriority.Order),
        nameof(ReadingPriority.TagName),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(ReadingPriority.EntryId),
        nameof(ReadingPriority.ReadingOrder),
        nameof(ReadingPriority.Order),
    ];

    protected override SqliteParameter[] Parameters(ReadingPriorityElement priority) =>
    [
        new("@0", priority.EntryId),
        new("@1", priority.ParentOrder),
        new("@2", priority.Order),
        new("@3", priority.TagName),
    ];
}
