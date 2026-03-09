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

internal sealed class VariantTable : Table<VariantElement>
{
    protected override string Name => nameof(Variant);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Variant.UnicodeScalarValue),
        nameof(Variant.TypeId),
        nameof(Variant.CommentId),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Variant.UnicodeScalarValue),
        nameof(Variant.TypeId),
    ];

    protected override SqliteParameter[] Parameters(VariantElement variant) =>
    [
        new("@0", variant.UnicodeScalarValue),
        new("@1", variant.TypeId),
        new("@2", variant.CommentId),
    ];
}
