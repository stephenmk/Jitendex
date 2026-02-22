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

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jitendex.EdrdgDictionaryArchive;

public static class Program
{
    public static int Main(string[] args)
    {
        Argument<DictionaryFile> filenameArgument = new("file")
        {
            Description = "File to retrieve"
        };

        Option<DateOnly?> dateOption = new("--date")
        {
            Description = "Date of the file to retrieve"
        };

        Option<DirectoryInfo> archiveDirOption = new("--archive-path")
        {
            Description = "Path to the edrdg-dictionary-archive directory",
        };

        var rootCommand = new RootCommand("Jitendex.EdrdgDictionaryArchive: Retrieve a versioned file from the EDRDG Dictionary Archive")
        {
            filenameArgument,
            dateOption,
            archiveDirOption,
        };

        var parseResult = rootCommand.Parse(args);

        foreach (var parseError in parseResult.Errors)
        {
            Console.Error.WriteLine(parseError.Message);
        }

        if (parseResult.Errors.Count > 0)
        {
            return 1;
        }

        var filename = parseResult.GetRequiredValue(filenameArgument);
        var date = parseResult.GetValue(dateOption);
        var archiveDirectory = parseResult.GetValue(archiveDirOption);

        var file = GetFile(filename, date, archiveDirectory);

        if (file is not null)
        {
            Console.WriteLine(file.FullName);
            return 0;
        }
        else
        {
            return 1;
        }
    }

    private static FileInfo? GetFile(DictionaryFile filename, DateOnly? date, DirectoryInfo? archiveDirectory)
    {
        var service = GetService();
        return date is not null
            ? service.GetFile(filename, (DateOnly)date, archiveDirectory)
            : service.GetLatestFile(filename, archiveDirectory) is (FileInfo latestFile, DateOnly _)
            ? latestFile
            : null;
    }

    private static IEdrdgArchiveService GetService()
        => new ServiceCollection()
            .AddEdrdgArchiveService()
            .AddLogging(static builder =>
                builder.AddSimpleConsole(static options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                }))
            .BuildServiceProvider()
            .GetRequiredService<IEdrdgArchiveService>();
}
