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

namespace Jitendex.Import;

public interface IFileArchive<TKey>
{
    FileInfo? GetFile(TKey key);
    (FileInfo, TKey)? GetNextFile(TKey previousKey);
    (FileInfo, TKey)? GetEarliestFile();
    (FileInfo, TKey)? GetLatestFile();
}

public interface IDocument<TKey>
{
    TKey ArchiveKey { get; init; }
}

public interface IDocumentDiff<TKey, TDocument>
    where TDocument : IDocument<TKey>
{
    TKey ArchiveKey { get; init; }
    TDocument Inserts { get; init; }
    TDocument Updates { get; init; }
    TDocument Deletes { get; init; }
}

public interface IDocumentDatabase<TKey, TDocument, TDiff>
    where TKey: struct
    where TDocument : IDocument<TKey>
    where TDiff : IDocumentDiff<TKey, TDocument>
{
    void EnsureCreated();
    TKey? GetLastKey();
    void Initialize(TDocument document);
    void Update(TDiff diff);
}

public interface IDocumentReader<TKey, TDocument>
    where TDocument : IDocument<TKey>
{
    Task<TDocument> ReadAsync(FileInfo file, TKey archiveKey);
}

public interface IDocumentDiffer<TKey, TDocument, TDiff>
    where TDocument : IDocument<TKey>
    where TDiff : IDocumentDiff<TKey, TDocument>
{
    TDiff Diff(TDocument docA, TDocument docB);
}
