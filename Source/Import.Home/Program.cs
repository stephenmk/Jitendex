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

namespace Jitendex.Import.Home;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Argument<ProgramArgument> argument = new("argument")
        {
            Description = "Import from or export to flat-file data"
        };

        Option<DirectoryInfo> dataDirOption = new("--data-path")
        {
            Description = "Path to the jitendex-data directory",
        };

        var rootCommand = new RootCommand("Jitendex.MiscData: Process flat-files from jitendex-data")
        {
            argument,
            dataDirOption,
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

        var argumentResult = parseResult.GetRequiredValue(argument);
        var dataDirectory = parseResult.GetValue(dataDirOption);

        var service = ServiceProvider.GetService(dataDirectory);

        switch (argumentResult)
        {
            case ProgramArgument.Import:
                await service.ImportAsync();
                break;
            case ProgramArgument.Export:
                await service.ExportAsync();
                break;
        }

        return 0;
    }

    private enum ProgramArgument
    {
        Import,
        Export,
    }
}
