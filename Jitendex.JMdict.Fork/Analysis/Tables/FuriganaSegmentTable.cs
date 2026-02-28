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
using Jitendex.JMdict.Fork.Entities.EntryItems.Furigana;

namespace Jitendex.JMdict.Fork.Analysis.Services.Tables;

internal sealed class FuriganaSegmentTable : Table<FuriganaSegmentRow>
{
    protected override string Name => nameof(FuriganaSegment);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(FuriganaSegment.EntryId),
        nameof(FuriganaSegment.ReadingOrder),
        nameof(FuriganaSegment.KanjiFormOrder),
        nameof(FuriganaSegment.Order),
        nameof(FuriganaSegment.BaseText),
        nameof(FuriganaSegment.Furigana),
        nameof(FuriganaSegment.TypeName),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(FuriganaSegment.EntryId),
        nameof(FuriganaSegment.ReadingOrder),
        nameof(FuriganaSegment.KanjiFormOrder),
        nameof(FuriganaSegment.Order),
    ];

    protected override SqliteParameter[] Parameters(FuriganaSegmentRow segment) =>
    [
        new("@0", segment.EntryId),
        new("@1", segment.ReadingOrder),
        new("@2", segment.KanjiFormOrder),
        new("@3", segment.Order),
        new("@4", segment.BaseText),
        new("@5", segment.Furigana.Nullable()),
        new("@6", segment.TypeName.Nullable()),
    ];
}
