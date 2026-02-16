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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Jitendex.Kanjidic2.Entities.SubgroupItems;

namespace Jitendex.Kanjidic2.Import.Analysis;

internal sealed class Analyzer(ILogger<Analyzer> logger, Kanjidic2Context context, DerivedReadingAnalyzer derivedReadingAnalyzer)
{
    public void Clean()
    {
        logger.LogInformation("Deleting previous analysis data from database");
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText =
            $"""
            DELETE FROM "{nameof(DerivedReading)}";
            """;
        command.ExecuteNonQuery();
    }

    public async Task AnalyzeAsync()
    {
        logger.LogInformation("Starting data analysis");
        derivedReadingAnalyzer.Analyze();
    }
}
