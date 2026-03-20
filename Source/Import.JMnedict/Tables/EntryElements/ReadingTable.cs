/*
Copyright (c) 2026 Stephen Kraus
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
using Jitendex.Data.JMnedict.Entities.EntryItems;
using Jitendex.Import.JMnedict.Models;

namespace Jitendex.Import.JMnedict.Tables.EntryElements;

internal sealed class ReadingTable : Table<ReadingElement>
{
    protected override string Name => nameof(Reading);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Reading.EntryId),
        nameof(Reading.Order),
        nameof(Reading.Text),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Reading.EntryId),
        nameof(Reading.Order),
    ];

    protected override SqliteParameter[] Parameters(ReadingElement reading) =>
    [
        new("@0", reading.EntryId),
        new("@1", reading.Order),
        new("@2", reading.Text),
    ];
}
