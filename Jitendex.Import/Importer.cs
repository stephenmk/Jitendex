/*
Copyright (c) 2025-2026 Stephen Kraus
SPDX-License-Identifier: AGPL-3.0-or-later

This file is part of Jitendex.

Jitendex is free software: you can redistribute it and/or modify it under the terms
of the GNU Affero General Public License as published by the Free Software Foundation,
either version 3 of the License or (at your option) any later version.

Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with Jitendex.
If not, see <https://www.gnu.org/licenses/>.
*/

namespace Jitendex.Import;

public sealed class Importer<TKey>
(
    IDocumentDatabase<TKey> database,
    IFileArchive<TKey> fileArchive,
    IDocumentReader<TKey> reader,
    IDocumentDiffer<TKey> documentDiffer
)
{
    public async Task ImportAsync()
    {
        database.EnsureCreated();

        var previousDocument = database.GetLastKey() is TKey lastKey
            ? await GetPreviousDocumentAsync(lastKey)
            : await InitializeDatabaseAsync();

        if (previousDocument is null)
        {
            return;
        }

        await UpdateDatabaseAsync(previousDocument);
    }

    private async Task<IDocument<TKey>?> InitializeDatabaseAsync()
    {
        if (fileArchive.GetEarliestFile() is (FileInfo file, TKey key))
        {
            var document = await reader.ReadAsync(file, key);
            database.Initialize(document);
            return document;
        }
        else
        {
            return null;
        }
    }

    private async Task<IDocument<TKey>?> GetPreviousDocumentAsync(TKey lastKey)
    {
        if (fileArchive.GetFile(lastKey) is FileInfo file)
        {
            return await reader.ReadAsync(file, lastKey);
        }
        else
        {
            return null;
        }
    }

    private async Task UpdateDatabaseAsync(IDocument<TKey> previousDoc)
    {
        while (fileArchive.GetNextFile(previousDoc.ArchiveKey) is (FileInfo nextFile, TKey nextKey))
        {
            var nextDoc = await reader.ReadAsync(nextFile, nextKey);
            var diff = documentDiffer.Diff(previousDoc, nextDoc);
            database.Update(diff);
            previousDoc = nextDoc;
        }
    }
}
