// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Service.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.Services;
using Jitendex.Forks.JMdict.Services.Furigana;
using Jitendex.Forks.JMdict.Services.Headwords;
using Jitendex.Forks.JMdict.Services.Kanwa;
using Jitendex.Forks.JMdict.Services.Links;
using Jitendex.Forks.JMdict.Services.Media;
using Jitendex.Forks.JMdict.Services.Patching;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.JMdict;

internal sealed class Service
(
    ILogger<Service> logger,
    HomeContext homeContext,
    JMdictForkContext forkContext,
    DatabaseCopyService databaseCopier,
    PatchService patches,
    TrademarkService trademarkService,
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
    HeadwordService headwordService,
    HeadwordSenseService headwordSenseService,
    HeadwordReferenceService headwordReferenceService,
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
        RunGraphicServices();
        RunHeadwordServices();

        RunPostprocessing();

        forkTransaction.Commit();
        homeTransaction.Commit();

        logger.LogInformation("Vacuuming database file.");
        forkContext.ExecuteVacuum();

        logger.LogInformation("Finished.");
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
        trademarkService.Write();
    }

    private void RunRestrictionServices()
    {
        logger.LogInformation("Making the implicit relationships in the data explicit.");
        restrictions.Write();
        readingRestrictions.Write();
        kanjiFormRestrictions.Write();
        crossReferences.Write();
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

    private void RunGraphicServices()
    {
        logger.LogInformation("Transferring graphics data.");
        graphicService.Write();
    }

    private void RunHeadwordServices()
    {
        logger.LogInformation("Computing dictionary headwords.");
        headwordService.Write();
        headwordSenseService.Write();
        headwordReferenceService.Write();
    }

    private void RunPostprocessing()
    {
        logger.LogInformation("Checking for miscellaneous data integrity issues.");
        integrityChecker.Write();
    }
}
