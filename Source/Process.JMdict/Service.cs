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
using Jitendex.Process.JMdict.Services.CrossReferences;
using Jitendex.Process.JMdict.Services.DatabaseCopy;
using Jitendex.Process.JMdict.Services.Furigana;
using Jitendex.Process.JMdict.Services.Headwords;
using Jitendex.Process.JMdict.Services.IntegrityChecks;
using Jitendex.Process.JMdict.Services.Kanwa;
using Jitendex.Process.JMdict.Services.Patching;
using Jitendex.Process.JMdict.Services.Restrictions;
using Microsoft.Extensions.Logging;

namespace Jitendex.Process.JMdict;

internal sealed class Service
(
    ILogger<Service> logger,
    HomeContext homeContext,
    JMdictForkContext forkContext,

    // 01
    DatabaseCopyService databaseCopier,
    LicenseService licenseService,
    GraphicService graphicService,

    // 02
    PatchService patches,
    TrademarkService trademarkService,

    // 03
    KanjiFormRestrictionService kanjiFormRestrictions,
    ReadingRestrictionService readingRestrictions,
    RestrictionService restrictions,

    // 04
    EntryReferenceService entryReference,
    KanjiFormReferenceService kanjiFormReference,
    ReadingReferenceService readingReference,

    // 05
    CharacterReadingService characterReadings,
    CharacterService characters,
    DerivedReadingService derivedReadings,
    DerivedReadingTypeService derivedReadingTypes,

    // 06
    ReadingKanjiFormBridgeService kanjiFormBridges,
    FuriganaSegmentService furiganaSegments,

    // 07
    HeadwordService headwordService,
    HeadwordSenseService headwordSenseService,
    HeadwordReferenceService headwordReferenceService,

    // 08
    CheckForUkTagOnEntriesWithoutKanjiForms checkForUkTagOnEntriesWithoutKanjiForms,
    CheckForRightSingleQuotes checkForRightSingleQuotes,
    CheckForZeroWidthSpaces checkForZeroWidthSpaces,
    CheckForUnpairedPriorityTags checkForUnpairedPriorityTags,
    CheckForPriorityTagsOnRareForms checkForPriorityTagsOnRareForms,
    CheckForTransitivityTagOnSensesGlossedAsAdverbs checkForTransitivityTagOnSensesGlossedAsAdverbs,
    CheckForCrossReferencesToSearchOnlyForms checkForCrossReferencesToSearchOnlyForms,
    CheckForRestrictionsToEveryVisibleKanjiForm checkForRestrictionsToEveryVisibleKanjiForm
)
{
    public void Run()
    {
        forkContext.RecreateDatabase();

        using var homeTransaction = homeContext.Database.BeginTransaction();
        using var forkTransaction = forkContext.Database.BeginTransaction();

        Run01DatabaseCopy();
        Run02PatchServices();
        Run03RestrictionServices();
        Run04CrossReferenceServices();
        Run05KanwaServices();
        Run06FuriganaServices();
        Run07HeadwordServices();
        Run08IntegrityChecks();

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

        logger.LogInformation("Copying data from the home database file.");
        licenseService.Write();
        graphicService.Write();
    }

    private void Run02PatchServices()
    {
        logger.LogInformation("Applying home-grown data patches.");
        patches.Write();
        trademarkService.Write();
    }

    private void Run03RestrictionServices()
    {
        logger.LogInformation("Adding table relationships for restriction data.");

        readingRestrictions.Run01();
        kanjiFormRestrictions.Run02();
        restrictions.Run03();
        kanjiFormBridges.Write();
    }

    private void Run04CrossReferenceServices()
    {
        logger.LogInformation("Adding table relationships for cross reference data.");

        entryReference.Run();
        kanjiFormReference.Run();
        readingReference.Run();
    }

    private void Run05KanwaServices()
    {
        logger.LogInformation("Transfer home-grown character information.");
        characters.Write();
        characterReadings.Write();

        logger.LogInformation("Adding inflections of standard readings.");
        derivedReadingTypes.Write();
        derivedReadings.Write();
    }

    private void Run06FuriganaServices()
    {
        logger.LogInformation("Running furigana solver.");
        furiganaSegments.Write();
    }

    private void Run07HeadwordServices()
    {
        logger.LogInformation("Computing dictionary headwords.");
        headwordService.Write();
        headwordSenseService.Write();
        headwordReferenceService.Write();
    }

    private void Run08IntegrityChecks()
    {
        logger.LogInformation("Checking for miscellaneous data integrity issues.");

        checkForUkTagOnEntriesWithoutKanjiForms.Run();
        checkForRightSingleQuotes.Run();
        checkForZeroWidthSpaces.Run();
        checkForUnpairedPriorityTags.Run();
        checkForPriorityTagsOnRareForms.Run();
        checkForTransitivityTagOnSensesGlossedAsAdverbs.Run();
        checkForCrossReferencesToSearchOnlyForms.Run();
        checkForRestrictionsToEveryVisibleKanjiForm.Run();
    }
}
