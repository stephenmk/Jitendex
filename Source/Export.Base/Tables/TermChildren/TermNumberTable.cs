// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, TermNumberTable.cs, is part of Jitendex.
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
using Jitendex.Data.Export.Entities.TermChildren;
using Jitendex.Export.Base.TableRows;

namespace Jitendex.Export.Base.Tables.TermChildren;

internal sealed class TermNumberTable : Table<TermNumberRow>
{
    protected override string Name { get; } = nameof(TermNumber);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(TermNumber.HeadwordId),
        nameof(TermNumber.TermGroupId),
        nameof(TermNumber.Value),
        nameof(TermNumber.Total),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(TermNumber.HeadwordId),
        nameof(TermNumber.TermGroupId),
    ];

    protected override object?[] ParameterValues(TermNumberRow row) =>
    [
        row.HeadwordId,
        row.TermGroupId,
        row.Value,
        row.Total,
    ];
}
