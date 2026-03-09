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

namespace Jitendex.Data;

public abstract class Table<T>
{
    protected abstract string Name { get; }
    protected abstract IReadOnlyList<string> ColumnNames { get; }
    protected abstract IReadOnlyList<string> KeyColNames { get; }
    protected abstract SqliteParameter[] Parameters(T item);

    private string InsertCommandText =>
        $"""
        INSERT INTO "{Name}"
        ({string.Join(',', ColumnNames.Select(static name => $"\"{name}\""))}) VALUES
        ({string.Join(',', ColumnNames.Select(static (_, idx) => $"@{idx:X}"))});
        """;

    private string InsertOrIgnoreCommandText =>
        $"""
        INSERT OR IGNORE INTO "{Name}"
        ({string.Join(',', ColumnNames.Select(static name => $"\"{name}\""))}) VALUES
        ({string.Join(',', ColumnNames.Select(static (_, idx) => $"@{idx:X}"))});
        """;

    private string UpdateCommandText =>
        $"""
        UPDATE "{Name}"
        SET   {string.Join(',', ColumnNames.Select(static (name, idx) => $"\"{name}\" = @{idx:X}"))}
        WHERE {string.Join(" AND ", KeyColNames.Select(static (name, idx) => $"\"{name}\" = @{idx:X}"))};
        """;

    private string DeleteCommandText =>
        $"""
        DELETE FROM "{Name}"
        WHERE {string.Join(" AND ", KeyColNames.Select(static (name, idx) => $"\"{name}\" = @{idx:X}"))};
        """;

    public void InsertItem(SqliteContext db, T item)
        => ExecuteNonQuery(db, [item], InsertCommandText);

    public void InsertItems(SqliteContext db, IEnumerable<T> items)
        => ExecuteNonQuery(db, items, InsertCommandText);

    public void InsertOrIgnoreItems(SqliteContext db, IEnumerable<T> items)
        => ExecuteNonQuery(db, items, InsertOrIgnoreCommandText, checkRows: false);

    public void UpdateItems(SqliteContext db, IEnumerable<T> items)
        => ExecuteNonQuery(db, items, UpdateCommandText);

    public void DeleteItems(SqliteContext db, IEnumerable<T> items)
        => ExecuteNonQuery(db, items, DeleteCommandText);

    private void ExecuteNonQuery(SqliteContext db, IEnumerable<T> items, string commandText, bool checkRows = true)
    {
        using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = commandText;
        foreach (var item in items)
        {
            // Extremely hot path: millions of loops here for DB initializations.
            // AddRange() & Clear() have shown to be more efficient than
            // updating the command.Parameters values on every loop.
            command.Parameters.AddRange(Parameters(item));
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected != 1 && checkRows)
            {
                Console.Error.WriteLine(commandText);
                foreach (var parameter in command.Parameters)
                {
                    Console.Error.WriteLine($"{parameter}");
                }
                throw new Exception($"{rowsAffected} rows affected (expected 1)");
            }
            command.Parameters.Clear();
        }
    }
}
