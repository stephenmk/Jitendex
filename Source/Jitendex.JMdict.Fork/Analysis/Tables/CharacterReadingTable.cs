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

using Microsoft.Data.Sqlite;
using Jitendex.Data;
using Jitendex.JMdict.Fork.Entities.EntryItems.Furigana;

namespace Jitendex.JMdict.Fork.Analysis.Tables;

internal sealed class CharacterReadingTable : Table<CharacterReadingRow>
{
    protected override string Name => nameof(CharacterReading);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(CharacterReading.CharacterValue),
        nameof(CharacterReading.TypeId),
        nameof(CharacterReading.Text),
        nameof(CharacterReading.Okurigana),
        nameof(CharacterReading.IsPrefix),
        nameof(CharacterReading.IsSuffix),
    ];

    protected override IReadOnlyList<string> KeyColNames
        => throw new NotImplementedException($"The primary key for table {Name} is auto-incremented.");

    protected override SqliteParameter[] Parameters(CharacterReadingRow row) =>
    [
        new("@0", row.CharacterValue),
        new("@1", row.TypeId),
        new("@2", row.Text),
        new("@3", row.Okurigana.Nullable()),
        new("@4", row.IsPrefix),
        new("@5", row.IsSuffix),
    ];
}
