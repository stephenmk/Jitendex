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
using Jitendex.Data.KanjiVG.Entities;
using Jitendex.KanjiVG.Import.Models;

namespace Jitendex.KanjiVG.Import.Tables;

internal sealed class StrokeNumberGroupTable : Table<StrokeNumberGroupElement>
{
    protected override string Name => nameof(StrokeNumberGroup);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(StrokeNumberGroup.UnicodeScalarValue),
        nameof(StrokeNumberGroup.VariantTypeId),
        nameof(StrokeNumberGroup.StyleId),
        nameof(StrokeNumberGroup.IdAttribute),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(StrokeNumberGroup.UnicodeScalarValue),
        nameof(StrokeNumberGroup.VariantTypeId),
    ];

    protected override SqliteParameter[] Parameters(StrokeNumberGroupElement group) =>
    [
        new("@0", group.UnicodeScalarValue),
        new("@1", group.VariantTypeId),
        new("@2", group.StyleId),
        new("@3", group.IdAttribute),
    ];
}
