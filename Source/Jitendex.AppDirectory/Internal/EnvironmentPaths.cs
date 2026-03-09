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

using static System.Environment;

namespace Jitendex.AppDirectory.Internal;

internal static class EnvironmentPaths
{
    private static string HomePath
        => GetFolderPath(SpecialFolder.UserProfile);

    public static string DataHomePath
        => GetFolderPath(SpecialFolder.LocalApplicationData);

    public static string CacheHomePath
        => GetEnvironmentVariable("XDG_CACHE_HOME")
        ?? OSVersion.Platform switch
        {
            PlatformID.Unix
                => Path.Join(HomePath, ".cache"),
            _
                => Path.Join(DataHomePath, "cache"),
        };
}
