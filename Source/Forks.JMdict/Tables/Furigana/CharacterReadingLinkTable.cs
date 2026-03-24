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

using Jitendex.Data;
using Jitendex.Forks.JMdict.Models;
using Jitendex.Data.JMdict.ForkEntities.Furigana;

namespace Jitendex.Forks.JMdict.Tables.Furigana;

internal sealed class CharacterReadingLinkTable : Table<CharacterReadingLinkRow>
{
    protected override string Name { get; } = nameof(CharacterReadingLink);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(CharacterReadingLink.EntryId),
        nameof(CharacterReadingLink.ReadingOrder),
        nameof(CharacterReadingLink.KanjiFormOrder),
        nameof(CharacterReadingLink.FuriganaSegmentOrder),
        nameof(CharacterReadingLink.CharacterReadingId),
        nameof(CharacterReadingLink.DerivedReadingText),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(CharacterReadingLink.EntryId),
        nameof(CharacterReadingLink.ReadingOrder),
        nameof(CharacterReadingLink.KanjiFormOrder),
        nameof(CharacterReadingLink.FuriganaSegmentOrder),
    ];

    protected override object?[] ParameterValues(CharacterReadingLinkRow row) =>
    [
        row.EntryId,
        row.ReadingOrder,
        row.KanjiFormOrder,
        row.FuriganaSegmentOrder,
        row.CharacterReadingId,
        row.DerivedReadingText,
    ];
}
