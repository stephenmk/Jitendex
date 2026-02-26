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
using Jitendex.KanjiVG.Entities;
using Jitendex.KanjiVG.Import.Models;

namespace Jitendex.KanjiVG.Import.Tables;

internal sealed class ComponentTable : Table<ComponentElement>
{
    protected override string Name => nameof(Component);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Component.UnicodeScalarValue),
        nameof(Component.VariantTypeId),
        nameof(Component.Order),
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

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Component.UnicodeScalarValue),
        nameof(Component.VariantTypeId),
        nameof(Component.Order),
    ];

    protected override SqliteParameter[] Parameters(ComponentElement component) =>
    [
        new("@0", component.UnicodeScalarValue),
        new("@1", component.VariantTypeId),
        new("@2", component.Order),
        new("@3", component.ParentOrder.Nullable()),
        new("@4", component.CharacterId.Nullable()),
        new("@5", component.IsVariant),
        new("@6", component.IsPartial),
        new("@7", component.OriginalId.Nullable()),
        new("@8", component.Part.Nullable()),
        new("@9", component.Number.Nullable()),
        new("@A", component.IsTradForm),
        new("@B", component.IsRadicalForm),
        new("@C", component.PositionId.Nullable()),
        new("@D", component.RadicalId.Nullable()),
        new("@E", component.PhonId.Nullable()),
    ];
}
