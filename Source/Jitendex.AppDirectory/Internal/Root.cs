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

namespace Jitendex.AppDirectory.Internal;

internal static class Root
{
    private readonly static string AppDirectoryName = "jitendex";

    public static DirectoryInfo Get(string path)
    {
        var appDirPath = Path.Join(path, AppDirectoryName);
        DirectoryInfo directory = new(appDirPath);
        if (!directory.Exists)
        {
            directory.Create();
        }
        return directory;
    }
}
