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
using Jitendex.Forks.JMdict.Services.DatabaseCopy;
using Jitendex.Forks.JMdict.Services.Furigana;
using Jitendex.Forks.JMdict.Services.Headwords;
using Jitendex.Forks.JMdict.Services.Kanwa;
using Jitendex.Forks.JMdict.Services.Links;
using Jitendex.Forks.JMdict.Services.Media;
using Jitendex.Forks.JMdict.Services.Patching;
using Jitendex.Forks.JMdict.Services.PostProcessing;
using Jitendex.Forks.JMdict.Tables.Furigana;
using Jitendex.Forks.JMdict.Tables.Headwords;
using Jitendex.Forks.JMdict.Tables.Kanwa;
using Jitendex.Forks.JMdict.Tables.Media;
using Jitendex.Forks.JMdict.Tables.References;
using Jitendex.Forks.JMdict.Tables.Restrictions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.JMdict;

internal static class ServiceProvider
{
    public static Service GetService() => new ServiceCollection()
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
        .AddTransient<IntegrityService>()
        .AddTransient<TrademarkService>()
        .AddTransient<RestrictionService>()
        .AddTransient<ReadingRestrictionService>()
        .AddTransient<KanjiFormRestrictionService>()
        .AddTransient<CrossReferenceService>()
        .AddTransient<GraphicService>()
        .AddTransient<CharacterService>()
        .AddTransient<CharacterReadingService>()
        .AddTransient<DerivedReadingService>()
        .AddTransient<DerivedReadingTypeService>()
        .AddTransient<KanjiFormBridgeService>()
        .AddTransient<FuriganaSegmentService>()
        .AddTransient<HeadwordService>()
        .AddTransient<HeadwordSenseService>()
        .AddTransient<HeadwordReferenceService>()
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

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(static options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Build and return the Service service.
        .BuildServiceProvider()
        .GetRequiredService<Service>();
}
