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

using Microsoft.Extensions.Logging;
using Jitendex.JMdict.Fork.Analysis.Services;
using Jitendex.JMdict.Fork.Analysis.Services.Analyzers;

namespace Jitendex.JMdict.Fork.Analysis;

internal sealed class Analyzer
(
    ILogger<Analyzer> logger,
    JMdictForkContext context,
    Database database,

    RestrictionAnalyzer restrictionAnalyzer,
    ReadingRestrictionAnalyzer readingRestrictionAnalyzer,
    KanjiFormRestrictionAnalyzer kanjiFormRestrictionAnalyzer,
    KanjiFormBridgeAnalyzer kanjiFormBridgeAnalyzer,
    FuriganaSegmentAnalyzer furiganaSegmentAnalyzer,
    CrossReferenceAnalyzer crossReferenceAnalyzer,

    CrossReferenceCacheService crossReferenceCacheService,
    FuriganaSolverService furiganaSolverService
)
{
    public async Task AnalyzeAsync(DirectoryInfo? dataDirectory)
    {
        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        logger.LogInformation("Copying data from the JMdict database file");
        database.TransferDataFromJmdict();

        logger.LogInformation("Starting data analysis");

        restrictionAnalyzer.Analyze();
        readingRestrictionAnalyzer.Analyze();
        kanjiFormRestrictionAnalyzer.Analyze();
        kanjiFormBridgeAnalyzer.Analyze();

        var referenceCache = await crossReferenceCacheService.LoadAsync(dataDirectory);
        crossReferenceAnalyzer.Analyze(referenceCache);
        await crossReferenceCacheService.ExportAsync(dataDirectory);

        var furiganaSolver = await furiganaSolverService.LoadAsync(dataDirectory);

        await furiganaSegmentAnalyzer.Analyze(furiganaSolver);

        transaction.Commit();
        context.ExecuteVacuum();
    }
}
