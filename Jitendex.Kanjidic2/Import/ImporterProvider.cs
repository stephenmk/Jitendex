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
using Jitendex.Import;
using Jitendex.Kanjidic2.Import.Models;
using Jitendex.Kanjidic2.Import.Parsing;
using Jitendex.Kanjidic2.Import.Parsing.GroupReaders;

namespace Jitendex.Kanjidic2.Import;

internal static class ImporterProvider
{
    public static Importer GetImporter(DirectoryInfo? archiveDirectory)
        => new ServiceCollection()

        // Database context
        .AddDbContext<Kanjidic2Context>()

        // Importer interfaces.
        .AddEdrdgArchiveService(DictionaryFile.kanjidic2, archiveDirectory)
        .AddTransient<IDocumentReader<DateOnly, Document>, DocumentReader>()
        .AddTransient<IDocumentDiffer<DateOnly, Document, DocumentDiff>, DocumentDiffer>()
        .AddTransient<IDocumentDatabase<DateOnly, Document, DocumentDiff>, DocumentDatabase>()

        // Top-level reader.
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

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Build and return the importer service.
        .AddTransient<Importer>()
        .BuildServiceProvider()
        .GetRequiredService<Importer>();
}
