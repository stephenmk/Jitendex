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
using static Jitendex.EdrdgDictionaryArchive.DictionaryFile;

namespace Jitendex.EdrdgDictionaryArchive.Internal;

internal sealed record Patch(DateOnly Date, string Path);

internal sealed class FileArchive(ILogger<FileArchive> logger)
{
    public (FileInfo, DateOnly)? GetExistingBaseFile(FileRequest request)
    {
        var baseFilePath = Path.Join
        (
            request.ArchiveDirectory.FullName,
            request.File.ToDirectoryName(),
            $"{request.File.ToFileName()}.br"
        );
        var file = new FileInfo(baseFilePath);
        if (!file.Exists)
        {
            logger.LogInformation("Base file for {File} is missing at path `{Path}`", request.File.ToFileName(), request.ArchiveDirectory.FullName);
            return null;
        }
        var baseFileDate = GetBaseFileDate(request.File);
        return (file, baseFileDate);
    }

    public DateOnly GetLatestPatchDate(FileRequest request)
    {
        var patchesDirectory = PatchesDirectory(request);
        var yearDir = patchesDirectory.GetSortedDirectories().Last();
        var monthDir = yearDir.GetSortedDirectories().Last();
        var patchFile = monthDir.GetSortedFiles().Last();
        return new DateOnly
        (
            year: int.Parse(yearDir.Name),
            month: int.Parse(monthDir.Name),
            day: int.Parse(patchFile.Name.AsSpan(0, 2))
        );
    }

    public DateOnly? GetNextPatchDate(FileRequest request)
    {
        var patchesDirectory = PatchesDirectory(request);
        foreach (var yearDir in patchesDirectory.GetSortedDirectories())
        {
            int year = int.Parse(yearDir.Name);
            if (year < request.Date.Year)
            {
                continue;
            }
            foreach (var monthDir in yearDir.GetSortedDirectories())
            {
                int month = int.Parse(monthDir.Name);
                if (year == request.Date.Year && month < request.Date.Month)
                {
                    continue;
                }
                foreach (var patchFile in monthDir.GetSortedFiles())
                {
                    int day = int.Parse(patchFile.Name.AsSpan(0, 2));
                    var date = new DateOnly(year, month, day);
                    if (request.Date < date)
                    {
                        return date;
                    }
                }
            }
        }
        return null;
    }

    public IReadOnlyList<Patch> GetPatches(FileRequest request)
    {
        List<Patch> patches = [];
        var patchesDirectory = PatchesDirectory(request);
        foreach (var yearDir in patchesDirectory.GetSortedDirectories())
        {
            int year = int.Parse(yearDir.Name);
            foreach (var monthDir in yearDir.GetSortedDirectories())
            {
                int month = int.Parse(monthDir.Name);
                foreach (var patchFile in monthDir.GetSortedFiles())
                {
                    int day = int.Parse(patchFile.Name.AsSpan(0, 2));
                    var patchDate = new DateOnly(year, month, day);
                    var patchPath = GetPatchPath(patchesDirectory, patchDate);
                    patches.Add(new(patchDate, patchPath));

                    if (patchDate == request.Date)
                    {
                        return patches;
                    }
                }
            }
        }
        logger.LogInformation("Requested file {File} for date {Date:yyyy-MM-dd} does not exist in the archive", request.File.ToFileName(), request.Date);
        return [];
    }

    private static DirectoryInfo PatchesDirectory(FileRequest request)
        => new(Path.Join(request.ArchiveDirectory.FullName, request.File.ToDirectoryName(), "patches"));

    private static string GetPatchPath(DirectoryInfo directory, DateOnly date)
        => Path.Join(directory.FullName, $"{date.Year}", $"{date.Month:D2}", $"{date.Day:D2}.patch.br");

     private static DateOnly GetBaseFileDate(DictionaryFile file)
        => file switch
        {
            JMdict => new(2023, 8, 20),
            JMdict_e => new(2025, 10, 9),
            JMdict_e_examp => new(2023, 8, 26),
            JMnedict => new(2023, 8, 20),
            kanjidic2 => new(2023, 8, 20),
            examples => new(2023, 9, 25),
            _ => throw new ArgumentOutOfRangeException(nameof(file))
        };
}
