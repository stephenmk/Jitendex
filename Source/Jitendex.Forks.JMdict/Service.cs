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
using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.Analyzers;
using Jitendex.Forks.JMdict.Services;

namespace Jitendex.Forks.JMdict;

internal sealed class Service
(
    ILogger<Service> logger,
    JMdictForkContext forkContext,
    HomeContext homeContext,
    DatabaseCopier databaseCopier,

    PatchAnalyzer patchAnalyzer,
    IntegrityAnalyzer integrityAnalyzer,

    RestrictionAnalyzer restrictionAnalyzer,
    ReadingRestrictionAnalyzer readingRestrictionAnalyzer,
    KanjiFormRestrictionAnalyzer kanjiFormRestrictionAnalyzer,
    KanjiFormBridgeAnalyzer kanjiFormBridgeAnalyzer,
    CrossReferenceAnalyzer crossReferenceAnalyzer,

    CompoundAnalyzer compoundAnalyzer,
    CharacterAnalyzer characterAnalyzer,
    CharacterReadingAnalyzer characterReadingAnalyzer,
    DerivedReadingAnalyzer derivedReadingAnalyzer,
    DerivedReadingTypeAnalyzer derivedReadingTypeAnalyzer,
    FuriganaSegmentAnalyzer furiganaSegmentAnalyzer
)
{
    public void Run()
    {
        forkContext.RecreateDatabase();

        using var homeTransaction = homeContext.Database.BeginTransaction();
        using var forkTransaction = forkContext.Database.BeginTransaction();

        logger.LogInformation("Copying data from the JMdict database file");
        databaseCopier.CopyDataFromJmdict();

        logger.LogInformation("Starting data analysis");
        RunSubroutines();

        forkTransaction.Commit();
        homeTransaction.Commit();

        forkContext.ExecuteVacuum();
    }

    private void RunSubroutines()
    {
        // Apply home-grown data patches.
        patchAnalyzer.Analyze();

        // Make the implicit relationships in the data explicit.
        restrictionAnalyzer.Analyze();
        readingRestrictionAnalyzer.Analyze();
        kanjiFormRestrictionAnalyzer.Analyze();
        kanjiFormBridgeAnalyzer.Analyze();
        crossReferenceAnalyzer.Analyze();

        // Transfer home-grown character information.
        compoundAnalyzer.Analyze();
        characterAnalyzer.Analyze();
        characterReadingAnalyzer.Analyze();

        // Add inflections of standard readings.
        derivedReadingTypeAnalyzer.Analyze();
        derivedReadingAnalyzer.Analyze();

        // Run furigana solver for all reading + kanji form pairs.
        furiganaSegmentAnalyzer.Analyze();

        // Check for miscellaneous data integrity issues.
        integrityAnalyzer.Analyze();
    }
}
