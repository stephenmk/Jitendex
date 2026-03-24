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

using Jitendex.Data;
using Jitendex.Data.KanjiVG.Entities;
using Jitendex.Import.KanjiVG.Models;

namespace Jitendex.Import.KanjiVG.Tables;

internal sealed class StrokeTable : Table<StrokeElement>
{
    protected override string Name => nameof(Stroke);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Stroke.UnicodeScalarValue),
        nameof(Stroke.VariantTypeId),
        nameof(Stroke.Order),
        nameof(Stroke.IdAttribute),
        nameof(Stroke.ComponentOrder),
        nameof(Stroke.TypeId),
        nameof(Stroke.PathData),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Stroke.UnicodeScalarValue),
        nameof(Stroke.VariantTypeId),
        nameof(Stroke.Order),
    ];

    protected override object?[] ParameterValues(StrokeElement stroke) =>
    [
        stroke.UnicodeScalarValue,
        stroke.VariantTypeId,
        stroke.Order,
        stroke.IdAttribute,
        stroke.ComponentOrder,
        stroke.TypeId,
        stroke.PathData,
    ];
}
