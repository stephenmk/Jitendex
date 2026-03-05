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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Jitendex.MiscData.ImportExport.Furigana;
using Jitendex.MiscData.ImportExport.Furigana.Tables;
using Jitendex.MiscData.ImportExport.JMdict;
using Jitendex.MiscData.ImportExport.JMdict.Tables;

namespace Jitendex.MiscData.ImportExport;

internal static class ServiceProvider
{
    public static Service GetService(DirectoryInfo? dataDirectory)
        => new ServiceCollection()

        // Service options.
        .AddTransient<ServiceOptions>(_ => new(dataDirectory))

        // Database context.
        .AddDbContext<MiscDataContext>()

        // Database tables.
        .AddTransient<CharacterTable>()
        .AddTransient<CharacterReadingTable>()
        .AddTransient<CharacterReadingTypeTable>()
        .AddTransient<CompoundTable>()
        .AddTransient<CompoundReadingTable>()
        .AddTransient<CrossReferenceSequenceTable>()

        // Import services.
        .AddTransient<CharacterService>()
        .AddTransient<CompoundService>()
        .AddTransient<CrossReferenceDataService>()

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
