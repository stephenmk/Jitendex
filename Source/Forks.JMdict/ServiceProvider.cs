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
using Microsoft.Extensions.DependencyInjection;
using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.Services;
using Jitendex.Forks.JMdict.Services.Furigana;
using Jitendex.Forks.JMdict.Services.Kanwa;
using Jitendex.Forks.JMdict.Services.Patching;
using Jitendex.Forks.JMdict.Services.References;
using Jitendex.Forks.JMdict.Services.Restrictions;
using Jitendex.Forks.JMdict.Tables.Furigana;
using Jitendex.Forks.JMdict.Tables.Kanwa;
using Jitendex.Forks.JMdict.Tables.References;
using Jitendex.Forks.JMdict.Tables.Restrictions;

namespace Jitendex.Forks.JMdict;

internal static class ServiceProvider
{
    public static Service GetService() => new ServiceCollection()
        .AddTransient<Service>()

        // Databases
        .AddDbContext<JMdictContext>()
        .AddDbContext<JMdictForkContext>()
        .AddDbContext<HomeContext>()

        // Helpers
        .AddTransient<DatabaseCopyService>()
        .AddTransient<CrossReferenceTextParser>()
        .AddTransient<CrossReferenceCacheService>()

        // Services
        .AddTransient<PatchService>()
        .AddTransient<PatchRebaser>()
        .AddTransient<IntegrityService>()

        .AddTransient<RestrictionService>()
        .AddTransient<ReadingRestrictionService>()
        .AddTransient<KanjiFormRestrictionService>()

        .AddTransient<CrossReferenceService>()

        .AddTransient<CharacterService>()
        .AddTransient<CharacterReadingService>()
        .AddTransient<DerivedReadingService>()
        .AddTransient<DerivedReadingTypeService>()
        .AddTransient<KanjiFormBridgeService>()
        .AddTransient<FuriganaSegmentService>()

        // Tables
        .AddTransient<RestrictionLinkTable>()
        .AddTransient<ReadingRestrictionLinkTable>()
        .AddTransient<KanjiFormRestrictionLinkTable>()

        .AddTransient<AmbiguousReferenceTable>()
        .AddTransient<EntryReferenceTable>()
        .AddTransient<ReadingReferenceTable>()
        .AddTransient<KanjiFormReferenceTable>()

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
        .AddTransient<ReadingKanjiFormBridgeTable>()
        .AddTransient<FuriganaSegmentTable>()
        .AddTransient<CharacterReadingLinkTable>()
        .AddTransient<CompoundReadingLinkTable>()

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
