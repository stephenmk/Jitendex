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
using Jitendex.KanjiVG.Models;

namespace Jitendex.KanjiVG.Database;

internal static class LookupData
{
    // Column names
    private const string C1 = nameof(ILookup.Id);
    private const string C2 = nameof(ILookup.Text);

    // Parameter names
    private const string P1 = $"@{C1}";
    private const string P2 = $"@{C2}";

    public static async Task InsertLookupsAsync<T>(this Context db, IEnumerable<T> lookups) where T : ILookup
    {
        var InsertSql =
            $"""
            INSERT INTO "{typeof(T).Name}"
            ("{C1}", "{C2}") VALUES
            ( {P1} ,  {P2} );
            """;

        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = InsertSql;

        foreach (var lookup in lookups)
        {
            command.Parameters.AddRange(new SqliteParameter[]
            {
                new(P1, lookup.Id),
                new(P2, lookup.Text),
            });

            await command.ExecuteNonQueryAsync();
            command.Parameters.Clear();
        }
    }
}
