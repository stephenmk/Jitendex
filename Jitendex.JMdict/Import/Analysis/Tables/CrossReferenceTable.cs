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
using Jitendex.JMdict.Entities.EntryItems.SenseItems;

namespace Jitendex.JMdict.Import.Analysis.Tables;

internal sealed class CrossReferenceTable : Table<CrossReferenceUpdate>
{
    protected override string Name => nameof(CrossReference);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(CrossReference.EntryId),
        nameof(CrossReference.SenseOrder),
        nameof(CrossReference.Order),
        nameof(CrossReference.RefEntryId),
        nameof(CrossReference.RefReadingOrder),
        nameof(CrossReference.RefKanjiFormOrder),
        nameof(CrossReference.RefSenseOrder),
        nameof(CrossReference.IsAmbiguous),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(CrossReference.EntryId),
        nameof(CrossReference.SenseOrder),
        nameof(CrossReference.Order),
    ];

    protected override SqliteParameter[] Parameters(CrossReferenceUpdate xref) =>
    [
        new("@0", xref.EntryId),
        new("@1", xref.SenseOrder),
        new("@2", xref.Order),
        new("@3", xref.RefEntryId.Nullable()),
        new("@4", xref.RefReadingOrder.Nullable()),
        new("@5", xref.RefKanjiFormOrder.Nullable()),
        new("@6", xref.RefSenseOrder.Nullable()),
        new("@7", xref.IsAmbiguous.Nullable()),
    ];
}
