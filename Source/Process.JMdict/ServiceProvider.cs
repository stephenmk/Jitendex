// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, ServiceProvider.cs, is part of Jitendex.
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
using Jitendex.Process.JMdict.Services.Patching.Helpers;
using Jitendex.Process.JMdict.Services.Restrictions;
using Jitendex.Process.JMdict.Tables.Furigana;
using Jitendex.Process.JMdict.Tables.Headwords;
using Jitendex.Process.JMdict.Tables.Kanwa;
using Jitendex.Process.JMdict.Tables.Media;
using Jitendex.Process.JMdict.Tables.References;
using Jitendex.Process.JMdict.Tables.Restrictions;
using Microsoft.Extensions.DependencyInjection;

namespace Jitendex.Process.JMdict;

internal static class ServiceProvider
{
    public static IServiceCollection AddJMdictProcess(this IServiceCollection services)
        => services

        .AddTransient<Service>()

        // Databases
        .AddDbContext<JMdictContext>()
        .AddDbContext<JMdictForkContext>()
        .AddDbContext<HomeContext>()

        // Process Steps
        .AddDatabaseCopyServices()
        .AddPatchingServices()
        .AddRestrictionsServices()
        .AddCrossReferencesServices()
        .AddKanwaServices()
        .AddFuriganaServices()
        .AddHeadwordsServices()
        .AddIntegrityChecksServices();

    private static IServiceCollection AddDatabaseCopyServices(this IServiceCollection services)
        => services
            // Tables
            .AddTransient<GraphicTable>()
            .AddTransient<LicenseTable>()
            // Services
            .AddTransient<DatabaseCopyService>()
            .AddTransient<LicenseService>()
            .AddTransient<GraphicService>();

    private static IServiceCollection AddPatchingServices(this IServiceCollection services)
        => services
            // Helpers
            .AddTransient<PatchDateChecker>()
            .AddTransient<PatchRebaser>()
            .AddTransient<PatchStacker>()
            .AddTransient<PatchStackSquasher>()
            // Services
            .AddTransient<PatchService>()
            .AddTransient<TrademarkService>();

    private static IServiceCollection AddRestrictionsServices(this IServiceCollection services)
        => services
            // Tables
            .AddTransient<RestrictionLinkTable>()
            .AddTransient<ReadingRestrictionLinkTable>()
            .AddTransient<KanjiFormRestrictionLinkTable>()
            .AddTransient<ReadingKanjiFormBridgeTable>()
            // Services
            .AddTransient<KanjiFormRestrictionService>()
            .AddTransient<ReadingRestrictionService>()
            .AddTransient<RestrictionService>()
            .AddTransient<ReadingKanjiFormBridgeService>();

    private static IServiceCollection AddCrossReferencesServices(this IServiceCollection services)
        => services
            // Tables
            .AddTransient<EntryReferenceTable>()
            .AddTransient<ReadingReferenceTable>()
            .AddTransient<KanjiFormReferenceTable>()
            // Services
            .AddTransient<EntryReferenceService>()
            .AddTransient<KanjiFormReferenceService>()
            .AddTransient<ReadingReferenceService>();

    private static IServiceCollection AddKanwaServices(this IServiceCollection services)
        => services
            // Tables
            .AddTransient<CompoundTable>()
            .AddTransient<CompoundReadingTable>()
            .AddTransient<CompoundReadingTypeTable>()
            .AddTransient<CompoundCharacterTable>()
            .AddTransient<CharacterTable>()
            .AddTransient<CharacterReadingTable>()
            .AddTransient<CharacterReadingTypeTable>()
            .AddTransient<CharacterReadingOkuriganaTable>()
            .AddTransient<DerivedCharacterReadingTable>()
            .AddTransient<DerivedCharacterReadingTypeTable>()
            .AddTransient<VariantTable>()
            .AddTransient<VariantTypeTable>()
            // Services.
            .AddTransient<CharacterReadingService>()
            .AddTransient<CharacterService>()
            .AddTransient<DerivedReadingService>()
            .AddTransient<DerivedReadingTypeService>();

    private static IServiceCollection AddFuriganaServices(this IServiceCollection services)
        => services
            // Tables
            .AddTransient<FuriganaSegmentTable>()
            .AddTransient<CharacterReadingLinkTable>()
            .AddTransient<CompoundReadingLinkTable>()
            // Services
            .AddTransient<FuriganaSegmentService>();

    private static IServiceCollection AddHeadwordsServices(this IServiceCollection services)
        => services
            // Tables
            .AddTransient<HeadwordTable>()
            .AddTransient<HeadwordRedirectTable>()
            .AddTransient<HeadwordSenseTable>()
            .AddTransient<HeadwordRuleTable>()
            .AddTransient<HeadwordReferenceTable>()
            .AddTransient<HeadwordTagTable>()
            // Services
            .AddTransient<HeadwordService>()
            .AddTransient<HeadwordSenseService>()
            .AddTransient<HeadwordReferenceService>();

    private static IServiceCollection AddIntegrityChecksServices(this IServiceCollection services)
        => services
            .AddTransient<CheckForUkTagOnEntriesWithoutKanjiForms>()
            .AddTransient<CheckForRightSingleQuotes>()
            .AddTransient<CheckForZeroWidthSpaces>()
            .AddTransient<CheckForUnpairedPriorityTags>()
            .AddTransient<CheckForPriorityTagsOnRareForms>()
            .AddTransient<CheckForTransitivityTagOnSensesGlossedAsAdverbs>()
            .AddTransient<CheckForCrossReferencesToSearchOnlyForms>()
            .AddTransient<CheckForRestrictionsToEveryVisibleKanjiForm>()
            .AddTransient<CheckForSearchOnlyFormsAtEnd>();
}
