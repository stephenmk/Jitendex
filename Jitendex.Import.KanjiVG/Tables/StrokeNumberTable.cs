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
using Jitendex.Data;
using Jitendex.Data.KanjiVG.Entities;
using Jitendex.Import.KanjiVG.Models;

namespace Jitendex.Import.KanjiVG.Tables;

internal sealed class StrokeNumberTable : Table<StrokeNumberElement>
{
    protected override string Name => nameof(StrokeNumber);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(StrokeNumber.UnicodeScalarValue),
        nameof(StrokeNumber.VariantTypeId),
        nameof(StrokeNumber.Order),
        nameof(StrokeNumber.Number),
        nameof(StrokeNumber.TransformAttribute),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(StrokeNumber.UnicodeScalarValue),
        nameof(StrokeNumber.VariantTypeId),
        nameof(StrokeNumber.Order),
    ];

    protected override SqliteParameter[] Parameters(StrokeNumberElement strokeNumber) =>
    [
        new("@0", strokeNumber.UnicodeScalarValue),
        new("@1", strokeNumber.VariantTypeId),
        new("@2", strokeNumber.Order),
        new("@3", strokeNumber.Number),
        new("@4", strokeNumber.TransformAttribute),
    ];
}
