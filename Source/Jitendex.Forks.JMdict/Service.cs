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
using Jitendex.Forks.JMdict.Services;
using Jitendex.Forks.JMdict.Services.Furigana;
using Jitendex.Forks.JMdict.Services.Kanwa;
using Jitendex.Forks.JMdict.Services.References;
using Jitendex.Forks.JMdict.Services.Restrictions;

namespace Jitendex.Forks.JMdict;

internal sealed class Service
(
    ILogger<Service> logger,
    HomeContext homeContext,
    JMdictForkContext forkContext,
    DatabaseCopyService databaseCopier,
    PatchService patchService,
    RestrictionService restrictionService,
    ReadingRestrictionService readingRestrictionService,
    KanjiFormRestrictionService kanjiFormRestrictionService,
    CrossReferenceService crossReferenceService,
    CompoundService compoundService,
    CharacterService characterService,
    CharacterReadingService characterReadingService,
    DerivedReadingService derivedReadingService,
    DerivedReadingTypeService derivedReadingTypeService,
    KanjiFormBridgeService kanjiFormBridgeService,
    FuriganaSegmentService furiganaSegmentService,
    IntegrityService integrityService
)
{
    public void Run()
    {
        forkContext.RecreateDatabase();

        using var homeTransaction = homeContext.Database.BeginTransaction();
        using var forkTransaction = forkContext.Database.BeginTransaction();

        RunPreprocessing();

        RunPatchServices();
        RunRestrictionServices();
        RunKanwaServices();
        RunFuriganaServices();
        RunReferenceServices();

        RunPostprocessing();

        forkTransaction.Commit();
        homeTransaction.Commit();

        forkContext.ExecuteVacuum();
    }

    private void RunPreprocessing()
    {
        logger.LogInformation("Copying data from the JMdict database file");
        databaseCopier.CopyDataFromJmdict();
    }

    private void RunPatchServices()
    {
        // Apply home-grown data patches.
        patchService.Write();
    }

    private void RunRestrictionServices()
    {
        // Make the implicit relationships in the data explicit.
        restrictionService.Write();
        readingRestrictionService.Write();
        kanjiFormRestrictionService.Write();
    }

    private void RunKanwaServices()
    {
        // Transfer home-grown character information.
        compoundService.Write();
        characterService.Write();
        characterReadingService.Write();

        // Add inflections of standard readings.
        derivedReadingTypeService.Write();
        derivedReadingService.Write();
    }

    private void RunFuriganaServices()
    {
        kanjiFormBridgeService.Write();
        // Run furigana solver for all reading + kanji form pairs.
        furiganaSegmentService.Write();
    }

    private void RunReferenceServices()
    {
        crossReferenceService.Write();
    }

    private void RunPostprocessing()
    {
        // Check for miscellaneous data integrity issues.
        integrityService.Write();
    }
}
