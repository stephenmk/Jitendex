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
using Jitendex.JMdict.Fork.Analysis.Models;
using Jitendex.Data.JMdict.Entities.EntryItems.References;

namespace Jitendex.JMdict.Fork.Analysis.Tables.References;

internal sealed class EntryReferenceTable : Table<EntryReferenceRow>
{
    protected override string Name => nameof(EntryReference);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(EntryReference.EntryId),
        nameof(EntryReference.SenseOrder),
        nameof(EntryReference.CrossReferenceOrder),
        nameof(EntryReference.RefEntryId),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(EntryReference.EntryId),
        nameof(EntryReference.SenseOrder),
        nameof(EntryReference.CrossReferenceOrder),
    ];

    protected override SqliteParameter[] Parameters(EntryReferenceRow row) =>
    [
        new("@0", row.EntryId),
        new("@1", row.SenseOrder),
        new("@2", row.CrossReferenceOrder),
        new("@3", row.RefEntryId),
    ];
}
