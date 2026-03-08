/*
Copyright (c) 2026 Stephen Kraus
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

using Microsoft.Data.Sqlite;
using Jitendex.SQLite;
using Jitendex.MiscData.Entities.Furigana;
using Jitendex.MiscData.ImportExport.Furigana.Models;

namespace Jitendex.MiscData.ImportExport.Furigana.Tables;

internal sealed class CharacterReadingTypeTable : Table<CharacterReadingTypeRow>
{
    protected override string Name => nameof(CharacterReadingType);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(CharacterReadingType.Id),
        nameof(CharacterReadingType.Name),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(CharacterReadingType.Id)
    ];

    protected override SqliteParameter[] Parameters(CharacterReadingTypeRow row) =>
    [
        new("@0", row.Id),
        new("@1", row.Name),
    ];
}
