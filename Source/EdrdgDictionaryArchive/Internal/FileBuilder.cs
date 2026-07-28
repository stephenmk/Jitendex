// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, FileBuilder.cs, is part of Jitendex.
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

using System.Buffers;
using Jitendex.MinimalPatch;
using Microsoft.Extensions.Logging;

namespace Jitendex.EdrdgDictionaryArchive.Internal;

internal sealed class FileBuilder(ILogger<FileBuilder> logger, FileCache cache, FileArchive archive)
{
    private static readonly ArrayPool<char> _arrayPool = ArrayPool<char>.Shared;

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

    private readonly ref struct BuildBase
    {
        public readonly FileInfo File { get; init; }
        public readonly DateOnly Date { get; init; }
        public readonly ReadOnlySpan<Patch> Patches { get; init; }
    }

    private FileInfo? BuildFile(FileRequest request)
    {
        var buildBase = GetBuildBase(request);
        if (buildBase.File is null)
        {
            return null;
        }
        else if (buildBase.Patches.IsEmpty)
        {
            return buildBase.File;
        }

        var length = buildBase.File.Length();
        var @patchBuffer = _arrayPool.Rent(length / 2);
        var originBuffer = _arrayPool.Rent(length * 3 / 2);
        var outputBuffer = _arrayPool.Rent(length * 3 / 2);
        buildBase.File.ReadInto(originBuffer);

        foreach (var patch in buildBase.Patches)
        {
            logger.LogInformation("Patching file to date {Date:yyyy-MM-dd}", patch.Date);
            var patchFile = new FileInfo(patch.Path);
            int patchLength = patchFile.ReadInto(patchBuffer);

            length = Patcher.ApplyPatch
            (
                @patchBuffer.AsSpan(..patchLength),
                originBuffer.AsSpan(..length),
                outputBuffer
            );

            var patched = outputBuffer.AsSpan(..length);
            var newOrigin = originBuffer.AsSpan(..length);
            patched.CopyTo(newOrigin);
        }

        var builtFile = cache.WriteFile(request, outputBuffer.AsSpan(..length));

        var baseFileRequest = request with { Date = buildBase.Date };
        cache.DeleteFile(baseFileRequest);

        _arrayPool.Return(@patchBuffer);
        _arrayPool.Return(originBuffer);
        _arrayPool.Return(outputBuffer);

        return builtFile;
    }

    private BuildBase GetBuildBase(FileRequest request)
    {
        FileInfo? baseFile = null;
        DateOnly? baseDate = null;

        if (archive.GetExistingBaseFile(request) is (FileInfo file, DateOnly date))
        {
            baseFile = file;
            baseDate = date;
        }

        if (baseDate.HasValue && baseDate.Value.Equals(request.Date))
        {
            return baseFile is not null
                ? new() { File = baseFile, Date = baseDate.Value, Patches = [] }
                : default;
        }

        var allPatches = archive.GetPatches(request);
        if (allPatches.Count == 0)
        {
            return default;
        }

        var patches = new Patch[allPatches.Count];
        int i = 0;

        foreach (var patch in allPatches)
        {
            var cachedFileRequest = request with { Date = patch.Date };
            if (cache.GetExistingFile(cachedFileRequest) is FileInfo cachedFile)
            {
                baseFile = cachedFile;
                baseDate = patch.Date;
                i = 0;
            }
            else
            {
                patches[i++] = patch;
            }
        }

        return baseFile is not null && baseDate.HasValue
            ? new() { File = baseFile, Date = baseDate.Value, Patches = patches.AsSpan(..i) }
            : default;
    }
}
