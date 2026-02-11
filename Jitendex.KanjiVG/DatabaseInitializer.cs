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

using Jitendex.KanjiVG.Database;
using Jitendex.KanjiVG.Models;

namespace Jitendex.KanjiVG;

internal static class DatabaseInitializer
{
    public static async Task WriteAsync(KanjiVGDocument kanjivg)
    {
        await using var context = new Context();

        // Delete and recreate the database file.
        await context.RecreateDatabaseAsync();

        // For faster importing, write data to memory rather than to the disk.
        await context.ExecuteFastNewDatabasePragmaAsync();

        // Begin inserting data.
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            await context.InsertLookupsAsync(kanjivg.VariantTypes);
            await context.InsertLookupsAsync(kanjivg.Comments);
            await context.InsertLookupsAsync(kanjivg.ComponentGroupStyles);
            await context.InsertLookupsAsync(kanjivg.StrokeNumberGroupStyles);
            await context.InsertLookupsAsync(kanjivg.ComponentCharacters);
            await context.InsertLookupsAsync(kanjivg.ComponentOriginals);
            await context.InsertLookupsAsync(kanjivg.ComponentPositions);
            await context.InsertLookupsAsync(kanjivg.ComponentRadicals);
            await context.InsertLookupsAsync(kanjivg.ComponentPhons);
            await context.InsertLookupsAsync(kanjivg.StrokeTypes);
            await context.InsertEntriesAsync(kanjivg.Entries);
            await transaction.CommitAsync();
        }

        // Write database to the disk.
        await context.SaveChangesAsync();

        // Rebuild the database compactly.
        await context.ExecuteVacuumAsync();
    }
}
