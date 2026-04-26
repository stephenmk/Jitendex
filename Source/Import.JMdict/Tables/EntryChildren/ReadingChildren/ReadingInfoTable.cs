// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, ReadingInfoTable.cs, is part of Jitendex.
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
using Jitendex.Data.JMdict.Entities.EntryChildren.ReadingChildren;
using Jitendex.Import.JMdict.TableRows;

namespace Jitendex.Import.JMdict.Tables.EntryChildren.ReadingChildren;

internal sealed class ReadingInfoTable : Table<ReadingInfoRow>
{
    protected override string Name { get; } = nameof(ReadingInfo);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(ReadingInfo.EntryId),
        nameof(ReadingInfo.ReadingOrder),
        nameof(ReadingInfo.Order),
        nameof(ReadingInfo.TagName),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(ReadingInfo.EntryId),
        nameof(ReadingInfo.ReadingOrder),
        nameof(ReadingInfo.Order),
    ];

    protected override object?[] ParameterValues(ReadingInfoRow row) =>
    [
        row.EntryId,
        row.ParentOrder,
        row.Order,
        row.TagName,
    ];
}
