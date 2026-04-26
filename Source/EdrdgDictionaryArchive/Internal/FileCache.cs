// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, FileCache.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using static Jitendex.AppDirectory.CacheSubdirectory;

namespace Jitendex.EdrdgDictionaryArchive.Internal;

internal sealed class FileCache
{
    public FileInfo? GetExistingFile(FileRequest request)
        => GetCachedFile(request) is var file && file.Exists
            ? file
            : null;

    public FileInfo WriteFile(FileRequest request, ReadOnlySpan<char> text)
    {
        var file = GetCachedFile(request);
        file.WriteCompressed(text);
        return file;
    }

    public void DeleteFile(FileRequest request)
    {
        if (GetExistingFile(request) is FileInfo file)
        {
            file.Delete();
        }
    }

    private FileInfo GetCachedFile(FileRequest request)
        => new(Path.Join
        (
            CacheDirectory(request).FullName,
            $"{request.Date.Year}-{request.Date.Month:D2}-{request.Date.Day:D2}.br")
        );

    private DirectoryInfo CacheDirectory(FileRequest request)
        => AppDirectory.CacheDirectory.Get(EdrdgArchiveDirectory)
            .CreateSubdirectory(request.File.ToDirectoryName());
}
