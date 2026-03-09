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

using Jitendex.AppDirectory;
using static Jitendex.AppDirectory.DataSubdirectory;

namespace Jitendex.EdrdgDictionaryArchive.Internal;

internal sealed record FileRequest
{
    public DateOnly Date { get; init; }
    public DictionaryFile File { get; init; }
    public DirectoryInfo ArchiveDirectory { get; init; }

    public FileRequest(DateOnly date, EdrdgArchiveServiceOptions options)
    {
        Date = date;
        File = options.File;
        ArchiveDirectory = options.ArchiveDirectory ?? DataHome.Get(EdrdgArchiveDirectory);
    }
}
