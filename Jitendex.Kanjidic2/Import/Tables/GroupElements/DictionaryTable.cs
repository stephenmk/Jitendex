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
using Jitendex.Data.Kanjidic2.Entities.GroupItems;
using Jitendex.Kanjidic2.Import.Models;

namespace Jitendex.Kanjidic2.Import.Tables.GroupElements;

internal sealed class DictionaryTable : Table<DictionaryElement>
{
    protected override string Name => nameof(Dictionary);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Dictionary.UnicodeScalarValue),
        nameof(Dictionary.GroupOrder),
        nameof(Dictionary.Order),
        nameof(Dictionary.Text),
        nameof(Dictionary.TypeName),
        nameof(Dictionary.Volume),
        nameof(Dictionary.Page),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Dictionary.UnicodeScalarValue),
        nameof(Dictionary.GroupOrder),
        nameof(Dictionary.Order),
    ];

    protected override SqliteParameter[] Parameters(DictionaryElement dictionary) =>
    [
        new("@0", dictionary.EntryId),
        new("@1", dictionary.GroupOrder),
        new("@2", dictionary.Order),
        new("@3", dictionary.Text),
        new("@4", dictionary.TypeName),
        new("@5", dictionary.Volume.Nullable()),
        new("@6", dictionary.Page.Nullable()),
    ];
}
