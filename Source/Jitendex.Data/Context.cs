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
using Microsoft.EntityFrameworkCore.Storage;
using Jitendex.Data.EntityFrameworkCore;
using static Jitendex.AppDirectory.CacheSubdirectory;

namespace Jitendex.Data;

public abstract class SqliteContext : DbContext
{
    private readonly string _dbPath;

    public SqliteContext(DatabaseFile databaseFile)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = GetDataSource(databaseFile),
            Pooling = true,
        };
        _dbPath = builder.ToString();
    }

    protected sealed override void OnConfiguring(DbContextOptionsBuilder options) => options
        .UseSqlite(_dbPath)
        .ReplaceService<IRelationalCommandBuilderFactory, SqliteRelationalCommandBuilderFactory>();

    /// <summary>
    /// Delete and recreate the database file.
    /// </summary>
    public void RecreateDatabase()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public long GetLastInsertRowId()
    {
        using var command = Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT last_insert_rowid();";
        if (command.ExecuteScalar() is long rowId)
        {
            return rowId;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Wait until all data is imported before checking foreign key constraints.
    /// </summary>
    public void ExecuteDeferForeignKeysPragma()
        => Database.ExecuteSqlRaw("PRAGMA defer_foreign_keys = ON;");

    /// <summary>
    /// Rebuild the database file compactly.
    /// </summary>
    public void ExecuteVacuum()
        => Database.ExecuteSqlRaw("VACUUM;");

#pragma warning disable EF1002

    public void AttachDatabase(DatabaseFile databaseFile)
        => Database.ExecuteSqlRaw($"ATTACH DATABASE '{GetDataSource(databaseFile)}' AS '{databaseFile}';");

    public void DetachDatabase(DatabaseFile databaseFile)
        => Database.ExecuteSqlRaw($"DETACH DATABASE '{databaseFile}';");

#pragma warning restore EF1002

    private static string GetDataSource(DatabaseFile databaseFile)
        => Path.Join
        (
            AppDirectory.Cache.Get(SqliteDirectory).FullName,
            databaseFile.ToFilename()
        );
}
