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
using Jitendex.Data.JMnedict.Entities.EntryItems.TranslationItems;
using Jitendex.Import.JMnedict.Models;

namespace Jitendex.Import.JMnedict.Tables.EntryElements.TranslationElements;

internal sealed class CrossReferenceTable : Table<CrossReferenceElement>
{
    protected override string Name => nameof(CrossReference);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(CrossReference.EntryId),
        nameof(CrossReference.TranslationOrder),
        nameof(CrossReference.Order),
        nameof(CrossReference.Text),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(CrossReference.EntryId),
        nameof(CrossReference.TranslationOrder),
        nameof(CrossReference.Order),
    ];

    protected override SqliteParameter[] Parameters(CrossReferenceElement xref) =>
    [
        new("@0", xref.EntryId),
        new("@1", xref.ParentOrder),
        new("@2", xref.Order),
        new("@3", xref.Text),
    ];
}
