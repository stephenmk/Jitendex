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

internal static class ComponentSequenceData
{
    // Column names
    private const string C1 = $"{nameof(Component.Sequences)}{nameof(Sequence.Text)}";
    private const string C2 = $"{nameof(Sequence.Components)}{nameof(Component.CodepointId)}";
    private const string C3 = $"{nameof(Sequence.Components)}{nameof(Component.PositionId)}";

    // Parameter names
    private const string P1 = $"@{C1}";
    private const string P2 = $"@{C2}";
    private const string P3 = $"@{C3}";

    private const string InsertSql =
        $"""
        INSERT INTO "{nameof(Component)}{nameof(Sequence)}"
        ("{C1}", "{C2}", "{C3}") VALUES
        ( {P1} ,  {P2} ,  {P3} );
        """;

    public static async Task InsertComponentSequencesAsync(this ChiseContext db, IEnumerable<Component> components)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = InsertSql;

        foreach (var component in components)
        {
            foreach (var sequence in component.Sequences)
            {
                command.Parameters.AddRange(new SqliteParameter[]
                {
                    new(P1, sequence.Text),
                    new(P2, component.CodepointId),
                    new(P3, component.PositionId),
                });

                await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
            }
        }
    }
}
