// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, RestrictionTable.cs, is part of Jitendex.
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

internal sealed class RestrictionTable : Table<RestrictionRow>
{
    protected override string Name { get; } = nameof(Restriction);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Restriction.EntryId),
        nameof(Restriction.ReadingOrder),
        nameof(Restriction.Order),
        nameof(Restriction.KanjiFormText),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Restriction.EntryId),
        nameof(Restriction.ReadingOrder),
        nameof(Restriction.Order),
    ];

    protected override object?[] ParameterValues(RestrictionRow row) =>
    [
        row.EntryId,
        row.ParentOrder,
        row.Order,
        row.KanjiFormText,
    ];
}
