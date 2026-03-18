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
using Jitendex.Forks.JMdict.Models;
using Jitendex.Data.JMdict.ForkEntities.Furigana;

namespace Jitendex.Forks.JMdict.Tables.Furigana;

internal sealed class CompoundReadingLinkTable : Table<CompoundReadingLinkRow>
{
    protected override string Name => nameof(CompoundReadingLink);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(CompoundReadingLink.EntryId),
        nameof(CompoundReadingLink.ReadingOrder),
        nameof(CompoundReadingLink.KanjiFormOrder),
        nameof(CompoundReadingLink.FuriganaSegmentOrder),
        nameof(CompoundReadingLink.CompoundId),
        nameof(CompoundReadingLink.ReadingText),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(CompoundReadingLink.EntryId),
        nameof(CompoundReadingLink.ReadingOrder),
        nameof(CompoundReadingLink.KanjiFormOrder),
        nameof(CompoundReadingLink.FuriganaSegmentOrder),
    ];

    protected override SqliteParameter[] Parameters(CompoundReadingLinkRow row) =>
    [
        new("@0", row.EntryId),
        new("@1", row.ReadingOrder),
        new("@2", row.KanjiFormOrder),
        new("@3", row.FuriganaSegmentOrder),
        new("@4", row.CompoundId),
        new("@5", row.ReadingText),
    ];
}
