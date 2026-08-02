// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, LicenseTable.cs, is part of Jitendex.
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
using Jitendex.Data.JMdict.ForkEntities.Media;
using Jitendex.Process.JMdict.TableRows;

namespace Jitendex.Process.JMdict.Tables.Media;

internal sealed class LicenseTable : Table<LicenseRow>
{
    protected override string Name { get; } = nameof(License);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(License.Id),
        nameof(License.Name),
        nameof(License.InfoUrl),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(License.Id)
    ];

    protected override object?[] ParameterValues(LicenseRow row) =>
    [
        row.Id,
        row.Name,
        row.InfoUrl,
    ];
}
