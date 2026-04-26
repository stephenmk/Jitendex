// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, MiscGroupTable.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using Jitendex.Data;
using Jitendex.Data.Kanjidic2.Entities.Groups;
using Jitendex.Import.Kanjidic2.TableRows;

namespace Jitendex.Import.Kanjidic2.Tables.Groups;

internal sealed class MiscGroupTable : Table<MiscGroupElement>
{
    protected override string Name { get; } = nameof(MiscGroup);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(MiscGroup.UnicodeScalarValue),
        nameof(MiscGroup.Order),
        nameof(MiscGroup.Grade),
        nameof(MiscGroup.Frequency),
        nameof(MiscGroup.JlptLevel),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(MiscGroup.UnicodeScalarValue),
        nameof(MiscGroup.Order),
    ];

    protected override object?[] ParameterValues(MiscGroupElement group) =>
    [
        group.EntryId,
        group.Order,
        group.Grade,
        group.Frequency,
        group.JlptLevel,
    ];
}
