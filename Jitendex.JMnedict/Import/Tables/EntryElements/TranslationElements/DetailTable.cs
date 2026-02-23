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

internal sealed class DetailTable : Table<DetailElement>
{
    protected override string Name => nameof(Detail);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Detail.EntryId),
        nameof(Detail.TranslationOrder),
        nameof(Detail.Order),
        nameof(Detail.Text),
        nameof(Detail.LanguageName),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Detail.EntryId),
        nameof(Detail.TranslationOrder),
        nameof(Detail.Order),
    ];

    protected override SqliteParameter[] Parameters(DetailElement detail) =>
    [
        new("@0", detail.EntryId),
        new("@1", detail.ParentOrder),
        new("@2", detail.Order),
        new("@3", detail.Text),
        new("@4", detail.LanguageName.Nullable()),
    ];
}
