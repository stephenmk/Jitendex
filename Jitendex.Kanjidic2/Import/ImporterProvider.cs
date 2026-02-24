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

using Jitendex.EdrdgDictionaryArchive;
using Jitendex.Kanjidic2.Import.Analysis;
using Jitendex.Kanjidic2.Import.Parsing;
using Jitendex.Kanjidic2.Import.Parsing.GroupReaders;

namespace Jitendex.Kanjidic2.Import;

internal static class ImporterProvider
{
    public static Importer GetImporter(DirectoryInfo? archiveDirectory) => new ServiceCollection()
        .AddTransient<Importer>()

        // File archive
        .AddEdrdgArchiveService(DictionaryFile.kanjidic2, archiveDirectory)

        // Database
        .AddDbContext<Kanjidic2Context>()
        .AddTransient<Database>()

        // Top-level readers.
        .AddTransient<Kanjidic2Reader>()
        .AddTransient<HeaderReader>()
        .AddTransient<EntriesReader>()
        .AddTransient<EntryReader>()

        // Group readers.
        .AddTransient<CodepointGroupReader>()
        .AddTransient<DictionaryGroupReader>()
        .AddTransient<MiscGroupReader>()
        .AddTransient<QueryCodeGroupReader>()
        .AddTransient<RadicalGroupReader>()
        .AddTransient<ReadingMeaningGroupReader>()

        // Subgroup readers.
        .AddTransient<ReadingMeaningReader>()

        // Analyzers
        .AddTransient<Analyzer>()
        .AddTransient<DerivedReadingAnalyzer>()

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Build and return the Kanjidic2 service.
        .BuildServiceProvider()
        .GetRequiredService<Importer>();
}
