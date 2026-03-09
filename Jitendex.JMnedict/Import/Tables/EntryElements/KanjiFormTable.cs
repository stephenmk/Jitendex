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
using Jitendex.Data.JMnedict.Entities.EntryItems;
using Jitendex.JMnedict.Import.Models;

namespace Jitendex.JMnedict.Import.Tables.EntryElements;

internal sealed class KanjiFormTable : Table<KanjiFormElement>
{
    protected override string Name => nameof(KanjiForm);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(KanjiForm.EntryId),
        nameof(KanjiForm.Order),
        nameof(KanjiForm.Text),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(KanjiForm.EntryId),
        nameof(KanjiForm.Order),
    ];

    protected override SqliteParameter[] Parameters(KanjiFormElement kanjiForm) =>
    [
        new("@0", kanjiForm.EntryId),
        new("@1", kanjiForm.Order),
        new("@2", kanjiForm.Text),
    ];
}
