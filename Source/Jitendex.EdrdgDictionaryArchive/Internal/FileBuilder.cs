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
using Jitendex.MinimalPatch;

namespace Jitendex.EdrdgDictionaryArchive.Internal;

internal sealed class FileBuilder(ILogger<FileBuilder> logger, FileCache cache, FileArchive archive)
{
    public FileInfo? GetFile(FileRequest request)
        => cache.GetExistingFile(request) ?? BuildFile(request);

    public (FileInfo, DateOnly)? GetNextFile(FileRequest request)
    {
        if (archive.GetNextPatchDate(request) is not DateOnly nextDate)
        {
            return null;
        }
        var nextFileRequest = request with { Date = nextDate };
        var nextFile = GetFile(nextFileRequest);
        return nextFile is not null
            ? (nextFile, nextDate)
            : null;
    }

    public (FileInfo, DateOnly)? GetEarliestFile(FileRequest request)
        => archive.GetExistingBaseFile(request);

    public (FileInfo, DateOnly)? GetLatestFile(FileRequest request)
    {
        var latestDate = archive.GetLatestPatchDate(request);
        var latestFileRequest = request with { Date = latestDate };
        var latestFile = GetFile(latestFileRequest);
        return latestFile is not null
            ? (latestFile, latestDate)
            : null;
    }

    private sealed record BuildBase(DateOnly Date, FileInfo File, IReadOnlyList<Patch> Patches);

    private FileInfo? BuildFile(FileRequest request)
    {
        if (GetBuildBase(request) is not BuildBase buildBase)
        {
            return null;
        }

        int length = buildBase.File.Length();
        Span<char> @patchBuffer = new char[length / 10];
        Span<char> originBuffer = new char[length * 3 / 2];
        Span<char> outputBuffer = new char[length * 3 / 2];
        buildBase.File.ReadInto(originBuffer);

        foreach (var patch in buildBase.Patches)
        {
            logger.LogInformation("Patching file to date {Date:yyyy-MM-dd}", patch.Date);
            var patchFile = new FileInfo(patch.Path);
            int patchLength = patchFile.ReadInto(patchBuffer);

            length = Patcher.ApplyPatch
            (
                @patchBuffer[..patchLength],
                originBuffer[..length],
                outputBuffer
            );

            outputBuffer[..length].CopyTo(originBuffer[..length]);
        }

        var builtFile = cache.WriteFile(request, outputBuffer[..length]);

        var baseFileRequest = request with { Date = buildBase.Date };
        cache.DeleteFile(baseFileRequest);

        return builtFile;
    }

    private BuildBase? GetBuildBase(FileRequest request)
    {
        var allPatches = archive.GetPatches(request);
        if (allPatches.Count == 0)
        {
            return null;
        }

        FileInfo? baseFile = null;
        DateOnly baseDate = default;

        if (archive.GetExistingBaseFile(request) is (FileInfo file, DateOnly date))
        {
            baseFile = file;
            baseDate = date;
        }

        List<Patch> patches = new(allPatches.Count);

        foreach (var patch in allPatches)
        {
            var cachedFileRequest = request with { Date = patch.Date };
            if (cache.GetExistingFile(cachedFileRequest) is FileInfo cachedFile)
            {
                baseFile = cachedFile;
                baseDate = patch.Date;
                patches.Clear();
            }
            else
            {
                patches.Add(patch);
            }
        }

        return baseFile is not null
            ? new(baseDate, baseFile, patches)
            : null;
    }
}
