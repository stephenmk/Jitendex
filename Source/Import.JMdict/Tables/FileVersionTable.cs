// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, FileVersionTable.cs, is part of Jitendex.
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
using Jitendex.Data.JMdict.Entities;
using Jitendex.Import.JMdict.TableRows;

namespace Jitendex.Import.JMdict.Tables;

internal sealed class FileVersionTable : Table<VersionRow>
{
    protected override string Name { get; } = nameof(FileVersion);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(FileVersion.Number)
    ];

    protected override ImmutableArray<string> KeyColNames
        => throw new InvalidOperationException($"The primary key for table {Name} is auto-incremented.");

    protected override object?[] ParameterValues(VersionRow row) =>
    [
        row.Number
    ];
}
