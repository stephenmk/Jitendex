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
using Jitendex.KanjiVG.Readers;
using Jitendex.KanjiVG.Readers.Lookups;

namespace Jitendex.KanjiVG;

internal record Files
{
    public required FileInfo SvgArchive { get; init; }
}

internal static class ReaderProvider
{
    public static KanjiVGReader GetReader(Files paths) => new ServiceCollection()
        .AddLogging(builder =>
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // XML file resource.
        .AddTransient<Files>(provider => paths)
        .AddTransient<KanjiFiles>()

        // Global lookup types
        .AddSingleton<VariantTypeCache>()
        .AddSingleton<CommentCache>()
        .AddSingleton<ComponentGroupStyleCache>()
        .AddSingleton<StrokeNumberGroupStyleCache>()
        .AddSingleton<ComponentCharacterCache>()
        .AddSingleton<ComponentOriginalCache>()
        .AddSingleton<ComponentPositionCache>()
        .AddSingleton<ComponentRadicalCache>()
        .AddSingleton<ComponentPhonCache>()
        .AddSingleton<StrokeTypeCache>()

        // Top-level readers.
        .AddTransient<EntriesReader>()
        .AddTransient<EntryReader>()

        // Stroke Path Components
        .AddTransient<ComponentGroupReader>()
        .AddTransient<ComponentAttributesReader>()
        .AddTransient<ComponentReader>()
        .AddTransient<StrokeReader>()

        // Stroke Numbers
        .AddTransient<StrokeNumberGroupReader>()
        .AddTransient<StrokeNumberReader>()

        // Build and return the KanjiVG service.
        .AddTransient<KanjiVGReader>()
        .BuildServiceProvider()
        .GetRequiredService<KanjiVGReader>();
}
