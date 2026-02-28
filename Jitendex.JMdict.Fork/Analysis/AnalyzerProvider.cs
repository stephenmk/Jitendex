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
using Jitendex.Kanjidic2;
using Jitendex.JMdict.Fork.Analysis.Services;
using Jitendex.JMdict.Fork.Analysis.Services.Analyzers;

namespace Jitendex.JMdict.Fork.Analysis;

internal static class AnalyzerProvider
{
    public static Analyzer GetAnalyzer() => new ServiceCollection()
        .AddTransient<Analyzer>()

        // Databases
        .AddDbContext<JMdictForkContext>()
        .AddDbContext<Kanjidic2Context>()
        .AddTransient<Database>()

        // Analyzers
        .AddTransient<RestrictionAnalyzer>()
        .AddTransient<ReadingRestrictionAnalyzer>()
        .AddTransient<KanjiFormRestrictionAnalyzer>()
        .AddTransient<KanjiFormBridgeAnalyzer>()
        .AddTransient<FuriganaSegmentAnalyzer>()
        .AddTransient<CrossReferenceAnalyzer>()

        // Helpers
        .AddTransient<CrossReferenceTextParser>()
        .AddTransient<CrossReferenceCacheService>()
        .AddTransient<FuriganaSolverService>()

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(static options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Build and return the Analyzer service.
        .BuildServiceProvider()
        .GetRequiredService<Analyzer>();
}
