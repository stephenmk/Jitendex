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

internal sealed class ComponentTable : Table<ComponentElement>
{
    protected override string Name { get; } = nameof(Component);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Component.UnicodeScalarValue),
        nameof(Component.VariantTypeId),
        nameof(Component.Order),
        nameof(Component.IdAttribute),
        nameof(Component.ParentOrder),
        nameof(Component.CharacterId),
        nameof(Component.IsVariant),
        nameof(Component.IsPartial),
        nameof(Component.OriginalId),
        nameof(Component.Part),
        nameof(Component.Number),
        nameof(Component.IsTradForm),
        nameof(Component.IsRadicalForm),
        nameof(Component.PositionId),
        nameof(Component.RadicalId),
        nameof(Component.PhonId),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Component.UnicodeScalarValue),
        nameof(Component.VariantTypeId),
        nameof(Component.Order),
    ];

    protected override object?[] ParameterValues(ComponentElement component) =>
    [
        component.UnicodeScalarValue,
        component.VariantTypeId,
        component.Order,
        component.IdAttribute,
        component.ParentOrder,
        component.CharacterId,
        component.IsVariant,
        component.IsPartial,
        component.OriginalId,
        component.Part,
        component.Number,
        component.IsTradForm,
        component.IsRadicalForm,
        component.PositionId,
        component.RadicalId,
        component.PhonId,
    ];
}
