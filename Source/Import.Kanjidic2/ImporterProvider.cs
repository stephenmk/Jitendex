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

using Jitendex.Data.Kanjidic2;
using Jitendex.EdrdgDictionaryArchive;
using Jitendex.Import.Kanjidic2.Readers;
using Jitendex.Import.Kanjidic2.Readers.GroupReaders;
using Jitendex.Import.Kanjidic2.TableRows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.Kanjidic2;

internal static class ImporterProvider
{
    public static Importer<DateOnly, Document, DocumentDiff> GetImporter(DirectoryInfo? archiveDirectory)
        => new ServiceCollection()

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Database context
        .AddDbContext<Kanjidic2Context>()

        // Importer interfaces.
        .AddEdrdgArchiveService(options =>
        {
            options.File = DictionaryFile.kanjidic2;
            options.ArchiveDirectory = archiveDirectory;
        })
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

        // Build and return the importer service.
        .AddTransient<Importer<DateOnly, Document, DocumentDiff>>()
        .BuildServiceProvider()
        .GetRequiredService<Importer<DateOnly, Document, DocumentDiff>>();
}
