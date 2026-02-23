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
using Jitendex.EdrdgDictionaryArchive;
using Jitendex.JMnedict.Import.Parsing;
using Jitendex.JMnedict.Import.Parsing.EntryElementReaders;
using Jitendex.JMnedict.Import.Parsing.EntryElementReaders.KanjiFormElementReaders;
using Jitendex.JMnedict.Import.Parsing.EntryElementReaders.ReadingElementReaders;
using Jitendex.JMnedict.Import.Parsing.EntryElementReaders.TranslationElementReaders;

namespace Jitendex.JMnedict.Import;

internal static class ImporterProvider
{
    public static Importer GetImporter() => new ServiceCollection()
        .AddTransient<Importer>()

        // File archive
        .AddEdrdgArchiveService()

        // Databases
        .AddDbContext<JMnedictContext>()
        .AddTransient<Database>()

        // Top-level readers.
        .AddTransient<DocumentReader>()
        .AddTransient<DocumentTypeReader>()
        .AddTransient<EntriesReader>()
        .AddTransient<EntryReader>()

        // Entry element readers.
        .AddTransient<KanjiFormReader>()
        .AddTransient<ReadingReader>()
        .AddTransient<TranslationReader>()

        // Kanji form element readers.
        .AddTransient<KInfoReader>()
        .AddTransient<KPriorityReader>()

        // Reading element readers.
        .AddTransient<RestrictionReader>()
        .AddTransient<RInfoReader>()
        .AddTransient<RPriorityReader>()

        // Translation element readers.
        .AddTransient<CrossReferenceReader>()
        .AddTransient<DetailReader>()
        .AddTransient<NameTypeReader>()

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(static options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Build and return the JMnedict service.
        .BuildServiceProvider()
        .GetRequiredService<Importer>();
}
