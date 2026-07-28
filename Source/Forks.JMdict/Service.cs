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
using Jitendex.Forks.JMdict.Services.DatabaseCopy;
using Jitendex.Forks.JMdict.Services.Furigana;
using Jitendex.Forks.JMdict.Services.Headwords;
using Jitendex.Forks.JMdict.Services.Kanwa;
using Jitendex.Forks.JMdict.Services.Links;
using Jitendex.Forks.JMdict.Services.Media;
using Jitendex.Forks.JMdict.Services.Patching;
using Jitendex.Forks.JMdict.Services.PostProcessing;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.JMdict;

internal sealed class Service
(
    ILogger<Service> logger,
    HomeContext homeContext,
    JMdictForkContext forkContext,

    // 01
    DatabaseCopyService databaseCopier,

    // 02
    PatchService patches,
    TrademarkService trademarkService,

    // 03
    CrossReferenceService crossReferences,
    KanjiFormRestrictionService kanjiFormRestrictions,
    ReadingRestrictionService readingRestrictions,
    RestrictionService restrictions,

    // 04
    CharacterReadingService characterReadings,
    CharacterService characters,
    DerivedReadingService derivedReadings,
    DerivedReadingTypeService derivedReadingTypes,

    // 05
    KanjiFormBridgeService kanjiFormBridges,
    FuriganaSegmentService furiganaSegments,

    // 06
    GraphicService graphicService,

    // 07
    HeadwordService headwordService,
    HeadwordSenseService headwordSenseService,
    HeadwordReferenceService headwordReferenceService,

    // 08
    IntegrityService integrityChecker
)
{
    public void Run()
    {
        forkContext.RecreateDatabase();

        using var homeTransaction = homeContext.Database.BeginTransaction();
        using var forkTransaction = forkContext.Database.BeginTransaction();

        Run01DatabaseCopy();
        Run02PatchServices();
        Run03LinkServices();
        Run04KanwaServices();
        Run05FuriganaServices();
        Run06GraphicServices();
        Run07HeadwordServices();
        Run08Postprocessing();

        forkTransaction.Commit();
        homeTransaction.Commit();

        logger.LogInformation("Vacuuming database file.");
        forkContext.ExecuteVacuum();

        logger.LogInformation("Finished.");
    }

    private void Run01DatabaseCopy()
    {
        logger.LogInformation("Copying data from the JMdict database file.");
        databaseCopier.CopyDataFromJmdict();
    }

    private void Run02PatchServices()
    {
        logger.LogInformation("Applying home-grown data patches.");
        patches.Write();
        trademarkService.Write();
    }

    private void Run03LinkServices()
    {
        logger.LogInformation("Making the implicit relationships in the data explicit.");
        restrictions.Write();
        readingRestrictions.Write();
        kanjiFormRestrictions.Write();
        crossReferences.Write();
    }

    private void Run04KanwaServices()
    {
        logger.LogInformation("Transfer home-grown character information.");
        characters.Write();
        characterReadings.Write();

        logger.LogInformation("Adding inflections of standard readings.");
        derivedReadingTypes.Write();
        derivedReadings.Write();
    }

    private void Run05FuriganaServices()
    {
        logger.LogInformation("Bridging readings with corresponding kanji forms");
        kanjiFormBridges.Write();

        logger.LogInformation("Running furigana solver.");
        furiganaSegments.Write();
    }

    private void Run06GraphicServices()
    {
        logger.LogInformation("Transferring graphics data.");
        graphicService.Write();
    }

    private void Run07HeadwordServices()
    {
        logger.LogInformation("Computing dictionary headwords.");
        headwordService.Write();
        headwordSenseService.Write();
        headwordReferenceService.Write();
    }

    private void Run08Postprocessing()
    {
        logger.LogInformation("Checking for miscellaneous data integrity issues.");
        integrityChecker.Write();
    }
}
