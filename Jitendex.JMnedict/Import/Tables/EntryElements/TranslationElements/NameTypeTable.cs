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

using Microsoft.Data.Sqlite;
using Jitendex.SQLite;
using Jitendex.JMnedict.Entities.EntryItems.TranslationItems;
using Jitendex.JMnedict.Import.Models;

namespace Jitendex.JMnedict.Import.Tables.EntryElements.TranslationElements;

internal sealed class NameTypeTable : Table<NameTypeElement>
{
    protected override string Name => nameof(NameType);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(NameType.EntryId),
        nameof(NameType.TranslationOrder),
        nameof(NameType.Order),
        nameof(NameType.TagName),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(NameType.EntryId),
        nameof(NameType.TranslationOrder),
        nameof(NameType.Order),
    ];

    protected override SqliteParameter[] Parameters(NameTypeElement nameType) =>
    [
        new("@0", nameType.EntryId),
        new("@1", nameType.ParentOrder),
        new("@2", nameType.Order),
        new("@3", nameType.TagName),
    ];
}
