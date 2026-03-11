/*
Copyright (c) 2025 Stephen Kraus
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

using Jitendex.AppDirectory.Internal;
using static Jitendex.AppDirectory.DataSubdirectory;

namespace Jitendex.AppDirectory;

public static class DataDirectory
{
    public static DirectoryInfo Get(DataSubdirectory subdir)
        => DataHomeRoot.GetDirectories(subdir.Name()) switch
        {
            var subdirectories and not [] => subdirectories.First(),
            _ => throw new DirectoryNotFoundException($"No data directory '{subdir.Name()}' found in '{DataHomeRoot.FullName}'")
        };

    private static DirectoryInfo DataHomeRoot
        => Root.Get(EnvironmentPaths.DataHomePath);

    private static string Name(this DataSubdirectory subdir)
        => subdir switch
        {
            ChiseIdsDirectory     => "chise-ids",
            EdrdgArchiveDirectory => "edrdg-dictionary-archive",
            JitendexDataDirectory => "jitendex-data",
            KanjiVGDirectory      => "kanjivg",
            _ => throw new ArgumentOutOfRangeException(nameof(subdir))
        };
}

public enum DataSubdirectory : byte
{
    ChiseIdsDirectory,
    EdrdgArchiveDirectory,
    JitendexDataDirectory,
    KanjiVGDirectory,
}
