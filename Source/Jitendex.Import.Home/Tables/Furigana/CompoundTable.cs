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
using Jitendex.Data.Home.Entities.Furigana;
using Jitendex.Import.Home.Models;

namespace Jitendex.Import.Home.Tables.Furigana;

internal sealed class CompoundTable : Table<CompoundRow>
{
    protected override string Name => nameof(Compound);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Compound.Text)
    ];

    protected override IReadOnlyList<string> KeyColNames
        => throw new NotImplementedException($"The primary key for table {Name} is auto-incremented.");

    protected override SqliteParameter[] Parameters(CompoundRow row) =>
    [
        new("@0", row.Text),
    ];
}
