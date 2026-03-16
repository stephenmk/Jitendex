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
using Jitendex.Data.JMdict.Entities.EntryItems.SenseItems;
using Jitendex.Import.JMdict.Models;

namespace Jitendex.Import.JMdict.Tables.EntryElements.SenseElements;

internal sealed class GlossTypeTable : Table<GlossTypeElement>
{
    protected override string Name => nameof(GlossType);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(GlossType.EntryId),
        nameof(GlossType.SenseOrder),
        nameof(GlossType.GlossOrder),
        nameof(GlossType.TagName),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(GlossType.EntryId),
        nameof(GlossType.SenseOrder),
        nameof(GlossType.GlossOrder),
    ];

    protected override SqliteParameter[] Parameters(GlossTypeElement name) =>
    [
        new("@0", name.EntryId),
        new("@1", name.ParentOrder),
        new("@2", name.Order),
        new("@3", name.TagName),
    ];
}
