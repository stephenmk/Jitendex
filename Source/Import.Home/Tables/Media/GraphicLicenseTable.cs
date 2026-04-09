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

using Jitendex.Data;
using Jitendex.Data.Home.Entities.Media;
using Jitendex.Import.Home.TableRows;

namespace Jitendex.Import.Home.Tables.Media;

internal sealed class GraphicLicenseTable : Table<GraphicLicenseRow>
{
    protected override string Name { get; } = nameof(GraphicLicense);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(GraphicLicense.Id),
        nameof(GraphicLicense.Name),
        nameof(GraphicLicense.InfoUrl),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(GraphicLicense.Id)
    ];

    protected override object?[] ParameterValues(GraphicLicenseRow row) =>
    [
        row.Id,
        row.Name,
        row.InfoUrl,
    ];
}
