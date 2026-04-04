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

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Jitendex.Data.JMdict.Entities.EntryItems;
using Jitendex.Data.JMdict.ForkEntities.Headwords;

namespace Jitendex.Data.JMdict.ForkEntities.Furigana;

[Table(nameof(ReadingKanjiFormBridge))]
[PrimaryKey(nameof(EntryId), nameof(ReadingOrder), nameof(KanjiFormOrder))]
public sealed class ReadingKanjiFormBridge
{
    public required int EntryId { get; init; }
    public required int ReadingOrder { get; init; }
    public required int KanjiFormOrder { get; init; }

    [ForeignKey($"{nameof(EntryId)}, {nameof(ReadingOrder)}")]
    public Reading Reading { get; init; } = null!;

    [ForeignKey($"{nameof(EntryId)}, {nameof(KanjiFormOrder)}")]
    public KanjiForm KanjiForm { get; init; } = null!;

    [InverseProperty(nameof(FuriganaSegment.KanjiFormBridge))]
    public List<FuriganaSegment> FuriganaSegments { get; init; } = [];

    [InverseProperty(nameof(Headword.ReadingKanjiFormBridge))]
    public Headword Headword { get; init; } = null!;
}
