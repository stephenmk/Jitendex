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
using Microsoft.EntityFrameworkCore;
using Jitendex.Chise.Entities;

namespace Jitendex.Chise.Import.Tables;

internal static class ComponentPositionTable
{
    // Column names
    private const string C1 = nameof(ComponentPosition.Id);

    // Parameter names
    private const string P1 = $"@{C1}";

    private const string InsertSql =
        $"""
        INSERT INTO "{nameof(ComponentPosition)}"
        ("{C1}") VALUES
        ( {P1} );
        """;

    public static async Task InsertComponentPositionsAsync(this ChiseContext db, IEnumerable<ComponentPosition> positions)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = InsertSql;

        foreach (var position in positions)
        {
            command.Parameters.AddRange(new SqliteParameter[]
            {
                new(P1, position.Id),
            });

            await command.ExecuteNonQueryAsync();
            command.Parameters.Clear();
        }
    }
}
