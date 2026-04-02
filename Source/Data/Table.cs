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

using System.Collections.Immutable;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data;

public abstract class Table<T>
{
    protected abstract string Name { get; }
    protected abstract ImmutableArray<string> ColumnNames { get; }
    protected abstract ImmutableArray<string> KeyColNames { get; }
    protected abstract object?[] ParameterValues(T item);

    private static readonly ImmutableArray<string> ParameterNames = Enumerable
        .Range(0, 2_000) // The maximum default column count in SQLite
        .Select(static i => $"@{i:X}")
        .ToImmutableArray();

    private string InsertCommandText =>
        $"""
        INSERT INTO "{Name}"
        ({string.Join(',', ColumnNames.Select(static name => $"\"{name}\""))}) VALUES
        ({string.Join(',', ParameterNames.AsSpan(..ColumnNames.Length))});
        """;

    private string InsertOrIgnoreCommandText =>
        $"""
        INSERT OR IGNORE INTO "{Name}"
        ({string.Join(',', ColumnNames.Select(static name => $"\"{name}\""))}) VALUES
        ({string.Join(',', ParameterNames.AsSpan(..ColumnNames.Length))});
        """;

    private string UpsertCommandText =>
        ColumnNames.Except(KeyColNames).ToArray() is var updateColNames && updateColNames is []
        ? InsertOrIgnoreCommandText
        : $"""
        INSERT INTO "{Name}"
        ({string.Join(',', ColumnNames.Select(static name => $"\"{name}\""))}) VALUES
        ({string.Join(',', ParameterNames.AsSpan(..ColumnNames.Length))})
        ON CONFLICT({string.Join(",", KeyColNames.Select(static name => $"\"{name}\""))})
        DO UPDATE SET
        {string.Join(",\n", updateColNames.Select(static name => $"\"{name}\" = excluded.\"{name}\""))};
        """;

    private string DeleteCommandText =>
        $"""
        DELETE FROM "{Name}"
        WHERE {string.Join("\nAND ", ColumnNames.Select(static (name, idx) => $"\"{name}\" IS {ParameterNames[idx]}"))};
        """;

    public void InsertItem(SqliteContext db, T item)
        => ExecuteNonQuery(db, [item], InsertCommandText);

    public void InsertItems(SqliteContext db, IEnumerable<T> items)
        => ExecuteNonQuery(db, items, InsertCommandText);

    public void InsertOrIgnoreItems(SqliteContext db, IEnumerable<T> items)
        => ExecuteNonQuery(db, items, InsertOrIgnoreCommandText, checkRows: false);

    public void UpsertItems(SqliteContext db, IEnumerable<T> items)
        => ExecuteNonQuery(db, items, UpsertCommandText);

    public void DeleteItems(SqliteContext db, IEnumerable<T> items)
        => ExecuteNonQuery(db, items, DeleteCommandText);

    private void ExecuteNonQuery(SqliteContext db, IEnumerable<T> items, string commandText, bool checkRows = true)
    {
        using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = commandText;

        var parameters = new DbParameter[ColumnNames.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            parameters[i] = command.CreateParameter();
            parameters[i].ParameterName = ParameterNames[i];
            command.Parameters.Add(parameters[i]);
        }

        foreach (var item in items)
        {
            var values = ParameterValues(item);
            for (int i = 0; i < values.Length; i++)
            {
                parameters[i].Value = values[i] ?? DBNull.Value;
            }
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected is not 1 && checkRows)
            {
                throw new InvalidOperationException($"{rowsAffected} rows affected (expected 1)");
            }
        }
    }
}
