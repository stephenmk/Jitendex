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
using Jitendex.SQLite;
using Jitendex.Data.JMnedict.Entities.EntryItems.ReadingItems;
using Jitendex.Import.JMnedict.Models;

namespace Jitendex.Import.JMnedict.Tables.EntryElements.ReadingElements;

internal sealed class RestrictionTable : Table<RestrictionElement>
{
    protected override string Name => nameof(Restriction);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Restriction.EntryId),
        nameof(Restriction.ReadingOrder),
        nameof(Restriction.Order),
        nameof(Restriction.KanjiFormText),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Restriction.EntryId),
        nameof(Restriction.ReadingOrder),
        nameof(Restriction.Order),
    ];

    protected override SqliteParameter[] Parameters(RestrictionElement restriction) =>
    [
        new("@0", restriction.EntryId),
        new("@1", restriction.ParentOrder),
        new("@2", restriction.Order),
        new("@3", restriction.KanjiFormText),
    ];
}
