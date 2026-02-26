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

using Jitendex.Chise.Readers;
using Jitendex.Chise.Database;

namespace Jitendex.Chise;

internal static class DatabaseInitializer
{
    public static async Task WriteAsync(IdsCollector collector)
    {
        await using var context = new Context();

        // Delete and recreate the database file.
        await context.RecreateDatabaseAsync();

        // For faster importing, write data to memory rather than to the disk.
        await context.ExecuteFastNewDatabasePragmaAsync();

        // Using a transaction decreases the runtime by 10 seconds.
        // Using multiple smaller transactions doesn't seem to improve upon that.
        await using var transaction = await context.Database.BeginTransactionAsync();

        // Wait until all data is imported before checking foreign key constraints.
        await context.ExecuteDeferForeignKeysPragmaAsync();

        // Begin inserting data.
        await context.InsertCodepointsAsync(collector.Codepoints.Values);
        await context.InsertUnicodeCharactersAsync(collector.UnicodeCharacters.Values);
        await context.InsertSequencesAsync(collector.Sequences.Values);
        await context.InsertComponentsAsync(collector.Components.Values);
        await context.InsertComponentSequencesAsync(collector.Components.Values);
        await context.InsertComponentPositionsAsync(collector.ComponentPositions.Values);

        await transaction.CommitAsync();

        // Write database to the disk.
        await context.SaveChangesAsync();

        // Rebuild the database compactly.
        await context.ExecuteVacuumAsync();
    }
}
