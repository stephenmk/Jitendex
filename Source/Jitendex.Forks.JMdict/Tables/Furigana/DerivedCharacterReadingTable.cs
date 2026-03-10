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
using Jitendex.Forks.JMdict.Models;
using Jitendex.Data.JMdict.Entities.EntryItems.Furigana;

namespace Jitendex.Forks.JMdict.Tables.Furigana;

internal sealed class DerivedCharacterReadingTable : Table<DerivedCharacterReadingRow>
{
    protected override string Name => nameof(DerivedCharacterReading);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(DerivedCharacterReading.ReadingId),
        nameof(DerivedCharacterReading.Text),
        nameof(DerivedCharacterReading.IsPrefix),
        nameof(DerivedCharacterReading.IsSuffix),
        nameof(DerivedCharacterReading.TypeId),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(DerivedCharacterReading.ReadingId),
        nameof(DerivedCharacterReading.Text),
    ];

    protected override SqliteParameter[] Parameters(DerivedCharacterReadingRow row) =>
    [
        new("@0", row.ReadingId),
        new("@1", row.Text),
        new("@2", row.IsPrefix),
        new("@3", row.IsSuffix),
        new("@4", row.TypeId),
    ];
}
