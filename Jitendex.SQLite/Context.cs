/*
Copyright (c) 2025 Stephen Kraus
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
using Jitendex.SQLite.EntityFrameworkCore;
using static Jitendex.AppDirectory.CacheSubdirectory;

namespace Jitendex.SQLite;

public abstract class SqliteContext : DbContext
{
    private readonly string _dbPath;

    public SqliteContext(string dbFilename)
    {
        var directory = AppDirectory.Cache.Get(SqliteDirectory);
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Join(directory.FullName, dbFilename),
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

    /// <summary>
    /// Delete and recreate the database file.
    /// </summary>
    public async Task RecreateDatabaseAsync()
    {
        await Database.EnsureDeletedAsync();
        await Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// For faster importing into a new db file, write data to memory rather than to the disk.
    /// </summary>
    /// <remarks>See: https://www.sqlite.org/pragma.html</remarks>
    public void ExecuteFastNewDatabasePragma()
        => Database.ExecuteSqlRaw
        (
            """
            PRAGMA synchronous  = OFF;
            PRAGMA journal_mode = OFF;
            PRAGMA temp_store   = MEMORY;
            PRAGMA cache_size   = -200000;
            PRAGMA locking_mode = EXCLUSIVE;
            """
        );

    /// <summary>
    /// For faster importing into a new db file, write data to memory rather than to the disk.
    /// </summary>
    /// <remarks>See: https://www.sqlite.org/pragma.html</remarks>
    public async Task ExecuteFastNewDatabasePragmaAsync()
        => await Database.ExecuteSqlRawAsync
        (
            """
            PRAGMA synchronous  = OFF;
            PRAGMA journal_mode = OFF;
            PRAGMA temp_store   = MEMORY;
            PRAGMA cache_size   = -200000;
            PRAGMA locking_mode = EXCLUSIVE;
            """
        );

    /// <summary>
    /// Wait until all data is imported before checking foreign key constraints.
    /// </summary>
    public void ExecuteDeferForeignKeysPragma()
        => Database.ExecuteSqlRaw("PRAGMA defer_foreign_keys = ON;");

    /// <summary>
    /// Wait until all data is imported before checking foreign key constraints.
    /// </summary>
    public async Task ExecuteDeferForeignKeysPragmaAsync()
        => await Database.ExecuteSqlRawAsync("PRAGMA defer_foreign_keys = ON;");


    /// <summary>
    /// Rebuild the database file compactly.
    /// </summary>
    public void ExecuteVacuum()
        => Database.ExecuteSqlRaw("VACUUM;");

    /// <summary>
    /// Rebuild the database file compactly.
    /// </summary>
    public async Task ExecuteVacuumAsync()
        => await Database.ExecuteSqlRawAsync("VACUUM;");
}
