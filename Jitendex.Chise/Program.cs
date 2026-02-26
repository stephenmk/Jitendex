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
using Jitendex.Chise.Import;

namespace Jitendex.Chise;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var chiseIdsDirectoryArgument = new Argument<DirectoryInfo>("chise-ids-dir")
        {
            Description = "Path to 'chise-ids' directory",
        };

        var rootCommand = new RootCommand("Jitendex.Chise: Import CHISE Ideographic Description Sequences (IDS)")
        {
            chiseIdsDirectoryArgument
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

        var chiseIdsDir = parseResult.GetRequiredValue(chiseIdsDirectoryArgument);

        var logger = new Logger();
        var reader = new DocumentReader(logger);
        var document = reader.Read(chiseIdsDir);
        using var context = new ChiseContext();
        var database = new DocumentDatabase(context);
        database.Initialize(document);

        return 0;
    }
}
