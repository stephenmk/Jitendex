// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Program.cs, is part of Jitendex.
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

using System.CommandLine;
using Jitendex.Import;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jitendex.EdrdgDictionaryArchive;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (ParseArgs(args) is not ParsedArgs parsedArgs)
            return 1;

        if (GetFile(parsedArgs) is not FileInfo file)
            return 1;

        Console.WriteLine(file.FullName);
        return 0;
    }

    private readonly record struct ParsedArgs
    (
        DictionaryFile FileName,
        DateOnly? Date,
        DirectoryInfo? ArchiveDirectory
    );

    private static ParsedArgs? ParseArgs(string[] args)
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

        var rootCommand = new RootCommand("Retrieve a versioned file from the EDRDG Dictionary Archive")
        {
            filenameArgument,
            dateOption,
            archiveDirOption,
        };

        var parseResult = rootCommand.Parse(args);

        foreach (var parseError in parseResult.Errors)
            Console.Error.WriteLine(parseError.Message);

        if (parseResult.Errors.Count > 0)
            return null;

        return new ParsedArgs
        (
            parseResult.GetRequiredValue(filenameArgument),
            parseResult.GetValue(dateOption),
            parseResult.GetValue(archiveDirOption)
        );
    }

    private static FileInfo? GetFile(ParsedArgs args)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Logging.AddSimpleConsole(static options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });

        builder.Services.AddEdrdgArchiveService(options =>
        {
            options.File = args.FileName;
            options.ArchiveDirectory = args.ArchiveDirectory;
        });

        using var host = builder.Build();
        var service = host.Services.GetRequiredService<IFileArchive<DateOnly>>();

        return args.Date.HasValue
            ? service.GetFile(args.Date.Value)
            : service.GetLatestFile() is (FileInfo latestFile, DateOnly _)
            ? latestFile
            : null;
    }
}
