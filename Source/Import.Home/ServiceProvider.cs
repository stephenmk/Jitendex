/*
Copyright (c) 2026 Stephen Kraus
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

using Jitendex.Data.Home;
using Jitendex.Import.Home.Services;
using Jitendex.Import.Home.Services.JMdict;
using Jitendex.Import.Home.Services.Kanwa;
using Jitendex.Import.Home.Services.Media;
using Jitendex.Import.Home.Services.Tatoeba;
using Jitendex.Import.Home.Tables.JMdict;
using Jitendex.Import.Home.Tables.Kanwa;
using Jitendex.Import.Home.Tables.Media;
using Jitendex.Import.Home.Tables.Tatoeba;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.Home;

internal static class ServiceProvider
{
    public static Service GetService(DirectoryInfo? dataDirectory)
        => new ServiceCollection()

        // Service options.
        .AddTransient<ServiceOptions>(_ => new(dataDirectory))

        // Database context.
        .AddDbContext<HomeContext>()

        // Database tables.
        .AddTransient<CharacterTable>()
        .AddTransient<CharacterReadingTable>()
        .AddTransient<CharacterReadingTypeTable>()
        .AddTransient<CompoundTable>()
        .AddTransient<CompoundReadingTable>()
        .AddTransient<CompoundReadingTypeTable>()
        .AddTransient<VariantTable>()
        .AddTransient<VariantTypeTable>()
        .AddTransient<CrossReferenceSequenceTable>()
        .AddTransient<TrademarkGlossTable>()
        .AddTransient<JMdictPatchTable>()
        .AddTransient<JMdictPatchApprovalTable>()
        .AddTransient<JMdictPatchRecallTable>()
        .AddTransient<ExampleTable>()
        .AddTransient<ExampleFuriganaTable>()
        .AddTransient<GraphicTable>()
        .AddTransient<SenseGraphicTable>()
        .AddTransient<GraphicLicenseTable>()
        .AddTransient<KanjiAliveAudioTable>()

        // Import services.
        .AddTransient<CharacterService>()
        .AddTransient<VariantService>()
        .AddTransient<CompoundService>()
        .AddTransient<CrossReferenceDataService>()
        .AddTransient<UserService>()
        .AddTransient<JMdictPatchService>()
        .AddTransient<TrademarkService>()
        .AddTransient<ExampleFuriganaService>()
        .AddTransient<GraphicService>()
        .AddTransient<AudioService>()

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Build and return the importer service.
        .AddTransient<Service>()
        .BuildServiceProvider()
        .GetRequiredService<Service>();
}
