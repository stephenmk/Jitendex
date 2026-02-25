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

using Jitendex.Import;
using Jitendex.Kanjidic2.Import.Analysis;
using Jitendex.Kanjidic2.Import.Models;

namespace Jitendex.Kanjidic2.Import;

internal sealed class Importer
(
    IFileArchive<DateOnly> fileArchive,
    IDocumentReader<DateOnly, Document> reader,
    IDocumentDiffer<DateOnly, Document, DocumentDiff> differ,
    IDocumentDatabase<DateOnly, Document, DocumentDiff> database,
    Kanjidic2Context context,
    Analyzer analyzer
)
{
    public async Task ImportAsync()
    {
        database.EnsureCreated();

        var previousDocument = database.GetLastKey() is DateOnly lastKey
            ? await GetPreviousDocumentAsync(lastKey)
            : await InitializeDatabaseAsync();

        if (previousDocument is null)
        {
            return;
        }

        using var transaction = context.Database.BeginTransaction();

        analyzer.Clean();
        await UpdateDatabaseAsync(previousDocument);
        await analyzer.AnalyzeAsync();

        transaction.Commit();
    }

    private async Task<Document?> InitializeDatabaseAsync()
    {
        if (fileArchive.GetEarliestFile() is (FileInfo file, DateOnly date))
        {
            var document = await reader.ReadAsync(file, date);
            database.Initialize(document);
            return document;
        }
        else
        {
            return null;
        }
    }

    private async Task<Document?> GetPreviousDocumentAsync(DateOnly previousDate)
    {
        if (fileArchive.GetFile(previousDate) is FileInfo file)
        {
            return await reader.ReadAsync(file, previousDate);
        }
        else
        {
            return null;
        }
    }

    private async Task UpdateDatabaseAsync(Document previousDoc)
    {
        while (fileArchive.GetNextFile(previousDoc.ArchiveKey) is (FileInfo nextFile, DateOnly nextDate))
        {
            var nextDoc = await reader.ReadAsync(nextFile, nextDate);
            var diff = differ.Diff(previousDoc, nextDoc);
            database.Update(diff);
            previousDoc = nextDoc;
        }
    }
}
