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
using Jitendex.MiscData.Entities.JMdict;
using Jitendex.MiscData.ImportExport.Models;

namespace Jitendex.MiscData.ImportExport.Tables.JMdict;

internal sealed class CrossReferenceSequenceTable : Table<CrossReferenceSequenceRow>
{
    protected override string Name => nameof(CrossReferenceSequence);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(CrossReferenceSequence.EntryId),
        nameof(CrossReferenceSequence.SenseNumber),
        nameof(CrossReferenceSequence.RefKanjiFormText),
        nameof(CrossReferenceSequence.RefReadingText),
        nameof(CrossReferenceSequence.RefSenseNumber),
        nameof(CrossReferenceSequence.RefEntryId),
    ];

    protected override IReadOnlyList<string> KeyColNames
        => throw new NotImplementedException($"The primary key for table {nameof(CrossReferenceSequence)} is auto-incremented.");

    protected override SqliteParameter[] Parameters(CrossReferenceSequenceRow row) =>
    [
        new("@0", row.EntryId),
        new("@1", row.SenseNumber),
        new("@2", row.RefKanjiFormText.Nullable()),
        new("@3", row.RefReadingText.Nullable()),
        new("@4", row.RefSenseNumber.Nullable()),
        new("@5", row.RefEntryId.Nullable()),
    ];
}
