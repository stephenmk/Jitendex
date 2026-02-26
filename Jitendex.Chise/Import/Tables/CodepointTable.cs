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
using Jitendex.SQLite;
using Jitendex.Chise.Entities;

namespace Jitendex.Chise.Import.Tables;

internal static class CodepointTable
{
    // Column names
    private const string C1 = nameof(Codepoint.Id);
    private const string C2 = nameof(Codepoint.UnicodeScalarValue);
    private const string C3 = nameof(Codepoint.SequenceText);
    private const string C4 = nameof(Codepoint.AltSequenceText);

    // Parameter names
    private const string P1 = $"@{C1}";
    private const string P2 = $"@{C2}";
    private const string P3 = $"@{C3}";
    private const string P4 = $"@{C4}";

    private const string InsertSql =
        $"""
        INSERT INTO "{nameof(Codepoint)}"
        ("{C1}", "{C2}", "{C3}", "{C4}") VALUES
        ( {P1} ,  {P2} ,  {P3} ,  {P4} );
        """;

    public static async Task InsertCodepointsAsync(this ChiseContext db, IEnumerable<Codepoint> codepoints)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = InsertSql;

        foreach (var codepoint in codepoints)
        {
            command.Parameters.AddRange(new SqliteParameter[]
            {
                new(P1, codepoint.Id),
                new(P2, codepoint.UnicodeScalarValue.Nullable()),
                new(P3, codepoint.SequenceText.Nullable()),
                new(P4, codepoint.AltSequenceText.Nullable()),
            });

            await command.ExecuteNonQueryAsync();
            command.Parameters.Clear();
        }
    }
}
