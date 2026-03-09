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

internal sealed class ComponentGroupTable : Table<ComponentGroupElement>
{
    protected override string Name => nameof(ComponentGroup);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(ComponentGroup.UnicodeScalarValue),
        nameof(ComponentGroup.VariantTypeId),
        nameof(ComponentGroup.StyleId),
        nameof(ComponentGroup.IdAttribute),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(ComponentGroup.UnicodeScalarValue),
        nameof(ComponentGroup.VariantTypeId),
    ];

    protected override SqliteParameter[] Parameters(ComponentGroupElement group) =>
    [
        new("@0", group.UnicodeScalarValue),
        new("@1", group.VariantTypeId),
        new("@2", group.StyleId),
        new("@3", group.IdAttribute),
    ];
}
