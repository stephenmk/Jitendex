// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, TermRuleTable.cs, is part of Jitendex.
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
using Jitendex.Process.Base.TableRows;

namespace Jitendex.Process.Base.Tables.TermChildren;

internal sealed class TermRuleTable : Table<TermRuleRow>
{
    protected override string Name { get; } = nameof(TermRule);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(TermRule.HeadwordId),
        nameof(TermRule.TermGroupId),
        nameof(TermRule.Name),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(TermRule.HeadwordId),
        nameof(TermRule.TermGroupId),
    ];

    protected override object?[] ParameterValues(TermRuleRow row) =>
    [
        row.HeadwordId,
        row.TermGroupId,
        row.Name,
    ];
}
