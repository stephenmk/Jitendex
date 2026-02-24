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

namespace Jitendex.SQLite.EntityFrameworkCore;

internal static class CommandTextExtensions
{
    /// <remarks>
    /// See: https://sqlite.org/withoutrowid.html
    /// </remarks>
    public static string WithoutRowId(this string commandText)
    {
        if (!commandText.StartsWith("CREATE TABLE", StringComparison.Ordinal))
        {
            return commandText;
        }

        if (commandText.Contains("AUTOINCREMENT", StringComparison.Ordinal))
        {
            return commandText;
        }

        int endCreateTableIndex = commandText.IndexOf(");", StringComparison.Ordinal);

        if (endCreateTableIndex < 0)
        {
            return commandText;
        }

        // Insert " WITHOUT ROWID" between the ")" and the ";"
        return commandText.Insert(endCreateTableIndex + 1, " WITHOUT ROWID");
    }
}
