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
using Jitendex.MiscData.ImportExport.Models;

namespace Jitendex.MiscData.ImportExport.Tables.Furigana;

internal sealed class CharacterReadingTable : Table<CharacterReadingRow>
{
    protected override string Name => nameof(CharacterReading);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(CharacterReading.CharacterValue),
        nameof(CharacterReading.Text),
        nameof(CharacterReading.IsPrefix),
        nameof(CharacterReading.IsSuffix),
        nameof(CharacterReading.Okurigana),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(CharacterReading.CharacterValue),
        nameof(CharacterReading.Text),
    ];

    protected override SqliteParameter[] Parameters(CharacterReadingRow row) =>
    [
        new("@0", row.CharacterValue),
        new("@1", row.Text),
        new("@2", row.IsPrefix),
        new("@3", row.IsSuffix),
        new("@4", row.Okurigana.Nullable()),
    ];
}
