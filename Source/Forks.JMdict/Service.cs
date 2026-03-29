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
using Jitendex.Forks.JMdict.Services.Media;
using Jitendex.Forks.JMdict.Services.Patching;
using Jitendex.Forks.JMdict.Services.References;
using Jitendex.Forks.JMdict.Services.Restrictions;

namespace Jitendex.Forks.JMdict;

internal sealed class Service
(
    ILogger<Service> logger,
    HomeContext homeContext,
    JMdictForkContext forkContext,
    DatabaseCopyService databaseCopier,
    PatchService patches,
    RestrictionService restrictions,
    ReadingRestrictionService readingRestrictions,
    KanjiFormRestrictionService kanjiFormRestrictions,
    CrossReferenceService crossReferences,
    GraphicService graphicService,
    CharacterService characters,
    CharacterReadingService characterReadings,
    DerivedReadingService derivedReadings,
    DerivedReadingTypeService derivedReadingTypes,
    KanjiFormBridgeService kanjiFormBridges,
    FuriganaSegmentService furiganaSegments,
    IntegrityService integrityChecker
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
        RunGraphicServices();

        RunPostprocessing();

        forkTransaction.Commit();
        homeTransaction.Commit();

        forkContext.ExecuteVacuum();
    }

    private void RunPreprocessing()
    {
        logger.LogInformation("Copying data from the JMdict database file.");
        databaseCopier.CopyDataFromJmdict();
    }

    private void RunPatchServices()
    {
        logger.LogInformation("Applying home-grown data patches.");
        patches.Write();
    }

    private void RunRestrictionServices()
    {
        logger.LogInformation("Making the implicit relationships in the data explicit.");
        restrictions.Write();
        readingRestrictions.Write();
        kanjiFormRestrictions.Write();
    }

    private void RunKanwaServices()
    {
        logger.LogInformation("Transfer home-grown character information.");
        characters.Write();
        characterReadings.Write();

        logger.LogInformation("Adding inflections of standard readings.");
        derivedReadingTypes.Write();
        derivedReadings.Write();
    }

    private void RunFuriganaServices()
    {
        logger.LogInformation("Bridging readings with corresponding kanji forms");
        kanjiFormBridges.Write();

        logger.LogInformation("Running furigana solver.");
        furiganaSegments.Write();
    }

    private void RunReferenceServices()
    {
        logger.LogInformation("Deducing cross reference relationships.");
        crossReferences.Write();
    }

    private void RunGraphicServices()
    {
        logger.LogInformation("Transferring graphics data.");
        graphicService.Write();
    }

    private void RunPostprocessing()
    {
        logger.LogInformation("Checking for miscellaneous data integrity issues.");
        integrityChecker.Write();
    }
}
