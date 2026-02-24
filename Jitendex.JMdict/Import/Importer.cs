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
using Jitendex.Import;
using Jitendex.JMdict.Import.Models;
using Jitendex.JMdict.Import.Parsing;

namespace Jitendex.JMdict.Import;

internal sealed class Importer
(
    ILogger<Importer> logger,
    IFileArchive<DateOnly> fileArchive,
    JmdictContext context,
    DocumentReader reader,
    Database database
)
{
    public async Task ImportAsync()
    {
        context.Database.EnsureCreated();
        var previousDate = GetPreviousDate();

        var previousDocument = previousDate == default
            ? await InitializeDatabaseAsync()
            : await GetPreviousDocumentAsync(previousDate);

        if (previousDocument is null)
        {
            logger.LogWarning("Unable to retrieve previous document");
            return;
        }

        await UpdateDatabaseAsync(previousDocument);
    }

    private DateOnly GetPreviousDate() => context.FileHeaders
        .OrderByDescending(static x => x.Id)
        .Take(1)
        .Select(static x => x.Date)
        .FirstOrDefault();

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

    private async Task UpdateDatabaseAsync(Document previousDocument)
    {
        while (fileArchive.GetNextFile(previousDocument.Header.Date) is (FileInfo nextFile, DateOnly nextDate))
        {
            var nextDocument = await reader.ReadAsync(nextFile, nextDate);
            var diff = new DocumentDiff(previousDocument, nextDocument);
            database.Update(diff);
            previousDocument = nextDocument;
        }
    }
}
