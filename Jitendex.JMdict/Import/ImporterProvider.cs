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
using Jitendex.JMdict.Import.Parsing;
using Jitendex.JMdict.Import.Parsing.EntryElementReaders;
using Jitendex.JMdict.Import.Parsing.EntryElementReaders.KanjiFormElementReaders;
using Jitendex.JMdict.Import.Parsing.EntryElementReaders.ReadingElementReaders;
using Jitendex.JMdict.Import.Parsing.EntryElementReaders.SenseElementReaders;

namespace Jitendex.JMdict.Import;

internal static class ImporterProvider
{
    public static Importer GetImporter() => new ServiceCollection()
        .AddTransient<Importer>()

        // File archive
        .AddEdrdgArchiveService()

        // Databases
        .AddDbContext<JmdictContext>()
        .AddTransient<Database>()

        // Top-level readers.
        .AddTransient<DocumentReader>()
        .AddTransient<DocumentTypeReader>()
        .AddTransient<EntriesReader>()
        .AddTransient<EntryReader>()

        // Entry element readers.
        .AddTransient<KanjiFormReader>()
        .AddTransient<ReadingReader>()
        .AddTransient<SenseReader>()

        // Kanji form element readers.
        .AddTransient<KInfoReader>()
        .AddTransient<KPriorityReader>()

        // Reading element readers.
        .AddTransient<RestrictionReader>()
        .AddTransient<RInfoReader>()
        .AddTransient<RPriorityReader>()

        // Sense element readers.
        .AddTransient<CrossReferenceReader>()
        .AddTransient<DialectReader>()
        .AddTransient<FieldReader>()
        .AddTransient<GlossReader>()
        .AddTransient<KanjiFormRestrictionReader>()
        .AddTransient<LanguageSourceReader>()
        .AddTransient<MiscReader>()
        .AddTransient<PartOfSpeechReader>()
        .AddTransient<ReadingRestrictionReader>()

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(static options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Build and return the Jmdict service.
        .BuildServiceProvider()
        .GetRequiredService<Importer>();
}
