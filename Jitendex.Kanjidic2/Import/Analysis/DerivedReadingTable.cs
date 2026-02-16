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
using Jitendex.SQLite;
using Jitendex.Kanjidic2.Entities.SubgroupItems;

namespace Jitendex.Kanjidic2.Import.Analysis;

internal sealed record DerivedReadingElement
(
    int EntryId,
    int GroupOrder,
    int ReadingMeaningOrder,
    int ReadingOrder,
    int Order,
    string Text,
    bool IsPrefix,
    bool IsSuffix,
    string TypeName
);

internal sealed class DerivedReadingTable : Table<DerivedReadingElement>
{
    protected override string Name => nameof(DerivedReading);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(DerivedReading.UnicodeScalarValue),
        nameof(DerivedReading.GroupOrder),
        nameof(DerivedReading.ReadingMeaningOrder),
        nameof(DerivedReading.ReadingOrder),
        nameof(DerivedReading.Order),
        nameof(DerivedReading.Text),
        nameof(DerivedReading.IsPrefix),
        nameof(DerivedReading.IsSuffix),
        nameof(DerivedReading.TypeName),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(DerivedReading.UnicodeScalarValue),
        nameof(DerivedReading.GroupOrder),
        nameof(DerivedReading.ReadingMeaningOrder),
        nameof(DerivedReading.ReadingOrder),
        nameof(DerivedReading.Order),
    ];

    protected override SqliteParameter[] Parameters(DerivedReadingElement reading) =>
    [
        new("@0", reading.EntryId),
        new("@1", reading.GroupOrder),
        new("@2", reading.ReadingMeaningOrder),
        new("@3", reading.ReadingOrder),
        new("@4", reading.Order),
        new("@5", reading.Text),
        new("@6", reading.IsPrefix),
        new("@7", reading.IsSuffix),
        new("@8", reading.TypeName),
    ];
}
