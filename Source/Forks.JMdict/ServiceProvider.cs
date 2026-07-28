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
using Jitendex.Forks.JMdict.Services.CrossReferences;
using Jitendex.Forks.JMdict.Services.DatabaseCopy;
using Jitendex.Forks.JMdict.Services.Furigana;
using Jitendex.Forks.JMdict.Services.Headwords;
using Jitendex.Forks.JMdict.Services.IntegrityChecks;
using Jitendex.Forks.JMdict.Services.Kanwa;
using Jitendex.Forks.JMdict.Services.Media;
using Jitendex.Forks.JMdict.Services.Patching;
using Jitendex.Forks.JMdict.Services.Restrictions;
using Jitendex.Forks.JMdict.Tables.Furigana;
using Jitendex.Forks.JMdict.Tables.Headwords;
using Jitendex.Forks.JMdict.Tables.Kanwa;
using Jitendex.Forks.JMdict.Tables.Media;
using Jitendex.Forks.JMdict.Tables.References;
using Jitendex.Forks.JMdict.Tables.Restrictions;
using Microsoft.Extensions.DependencyInjection;

namespace Jitendex.Forks.JMdict;

internal static class ServiceProvider
{
    public static IServiceCollection AddJMdictForkService(this IServiceCollection services)
        => services

        .AddTransient<Service>()

    #region Databases
        .AddDbContext<JMdictContext>()
        .AddDbContext<JMdictForkContext>()
        .AddDbContext<HomeContext>()
    #endregion

    #region Services
        .AddTransient<DatabaseCopyService>()

        .AddTransient<PatchService>()
        .AddTransient<PatchRebaser>()
        .AddTransient<TrademarkService>()

        .AddTransient<KanjiFormRestrictionService>()
        .AddTransient<ReadingRestrictionService>()
        .AddTransient<RestrictionService>()

        .AddTransient<EntryReferenceService>()
        .AddTransient<KanjiFormReferenceService>()
        .AddTransient<ReadingReferenceService>()

        .AddTransient<CharacterReadingService>()
        .AddTransient<CharacterService>()
        .AddTransient<DerivedReadingService>()
        .AddTransient<DerivedReadingTypeService>()

        .AddTransient<KanjiFormBridgeService>()
        .AddTransient<FuriganaSegmentService>()

        .AddTransient<GraphicService>()

        .AddTransient<HeadwordService>()
        .AddTransient<HeadwordSenseService>()
        .AddTransient<HeadwordReferenceService>()

        .AddTransient<CheckForUkTagOnEntriesWithoutKanjiForms>()
        .AddTransient<CheckForRightSingleQuotes>()
        .AddTransient<CheckForZeroWidthSpaces>()
        .AddTransient<CheckForUnpairedPriorityTags>()
        .AddTransient<CheckForPriorityTagsOnRareForms>()
        .AddTransient<CheckForTransitivityTagOnSensesGlossedAsAdverbs>()
        .AddTransient<CheckForCrossReferencesToSearchOnlyForms>()
        .AddTransient<CheckForRestrictionsToEveryVisibleKanjiForm>()
    #endregion

    #region Tables
        .AddTransient<RestrictionLinkTable>()
        .AddTransient<ReadingRestrictionLinkTable>()
        .AddTransient<KanjiFormRestrictionLinkTable>()
        .AddTransient<EntryReferenceTable>()
        .AddTransient<ReadingReferenceTable>()
        .AddTransient<KanjiFormReferenceTable>()
        .AddTransient<GraphicTable>()
        .AddTransient<GraphicLicenseTable>()
        .AddTransient<SenseGraphicTable>()
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
        .AddTransient<ReadingKanjiFormBridgeTable>()
        .AddTransient<FuriganaSegmentTable>()
        .AddTransient<CharacterReadingLinkTable>()
        .AddTransient<CompoundReadingLinkTable>()
        .AddTransient<HeadwordTable>()
        .AddTransient<HeadwordRedirectTable>()
        .AddTransient<HeadwordSenseTable>()
        .AddTransient<HeadwordRuleTable>()
        .AddTransient<HeadwordReferenceTable>()
        .AddTransient<HeadwordTagTable>()
    #endregion
    ;
}
