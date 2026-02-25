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
using Jitendex.SQLite;

namespace Jitendex.KanjiVG.Database;

internal static class StrokeData
{
    // Column names
    private const string C1 = nameof(Stroke.UnicodeScalarValue);
    private const string C2 = nameof(Stroke.VariantTypeId);
    private const string C3 = nameof(Stroke.GlobalOrder);
    private const string C4 = nameof(Stroke.LocalOrder);
    private const string C5 = nameof(Stroke.ComponentGlobalOrder);
    private const string C6 = nameof(Stroke.TypeId);
    private const string C7 = nameof(Stroke.PathData);

    // Parameter names
    private const string P1 = $"@{C1}";
    private const string P2 = $"@{C2}";
    private const string P3 = $"@{C3}";
    private const string P4 = $"@{C4}";
    private const string P5 = $"@{C5}";
    private const string P6 = $"@{C6}";
    private const string P7 = $"@{C7}";

    private const string InsertSql =
        $"""
        INSERT INTO "{nameof(Stroke)}"
        ("{C1}", "{C2}", "{C3}", "{C4}", "{C5}", "{C6}", "{C7}") VALUES
        ( {P1} ,  {P2} ,  {P3} ,  {P4} ,  {P5} ,  {P6} ,  {P7} );
        """;

    public static async Task InsertStrokesAsync(this Context db, List<Stroke> strokes)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = InsertSql;

        foreach (var stroke in strokes)
        {
            command.Parameters.AddRange(new SqliteParameter[]
            {
                new(P1, stroke.UnicodeScalarValue),
                new(P2, stroke.VariantTypeId),
                new(P3, stroke.GlobalOrder),
                new(P4, stroke.LocalOrder),
                new(P5, stroke.ComponentGlobalOrder),
                new(P6, stroke.TypeId),
                new(P7, stroke.PathData),
            });

            await command.ExecuteNonQueryAsync();
            command.Parameters.Clear();
        }
    }
}
