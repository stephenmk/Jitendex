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
using Jitendex.KanjiVG.Entities;

namespace Jitendex.KanjiVG.Database;

internal static class ComponentGroupData
{
    // Column names
    private const string C1 = nameof(ComponentGroup.UnicodeScalarValue);
    private const string C2 = nameof(ComponentGroup.VariantTypeId);
    private const string C3 = nameof(ComponentGroup.StyleId);

    // Parameter names
    private const string P1 = $"@{C1}";
    private const string P2 = $"@{C2}";
    private const string P3 = $"@{C3}";

    private const string InsertSql =
        $"""
        INSERT INTO "{nameof(ComponentGroup)}"
        ("{C1}", "{C2}", "{C3}") VALUES
        ( {P1} ,  {P2} ,  {P3} );
        """;

    public static async Task InsertComponentGroupsAsync(this Context db, List<ComponentGroup> groups)
    {
        var allComponents = new List<Component>(groups.Count * 8);

        await using (var command = db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = InsertSql;

            foreach (var group in groups)
            {
                command.Parameters.AddRange(new SqliteParameter[]
                {
                    new(P1, group.UnicodeScalarValue),
                    new(P2, group.VariantTypeId),
                    new(P3, group.StyleId),
                });

                var commandExecution = command.ExecuteNonQueryAsync();

                allComponents.AddRange(EnumerateComponents(group.Components));

                await commandExecution;
                command.Parameters.Clear();
            }
        }

        await db.InsertComponentsAsync(allComponents);
    }

    public static IEnumerable<Component> EnumerateComponents(List<Component> components)
    {
        foreach (var component in components)
        {
            yield return component;
            foreach (var childComponent in EnumerateComponents(component.Children))
            {
                yield return childComponent;
            }
        }
    }
}
