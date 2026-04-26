// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, CompoundReadingLink.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using System.ComponentModel.DataAnnotations.Schema;
using Jitendex.Data.JMdict.ForkEntities.Kanwa;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.JMdict.ForkEntities.Furigana;

[Table(nameof(CompoundReadingLink))]
[PrimaryKey(nameof(EntryId), nameof(ReadingOrder), nameof(KanjiFormOrder), nameof(FuriganaSegmentOrder))]
public sealed class CompoundReadingLink
{
    public required int EntryId { get; init; }
    public required int ReadingOrder { get; init; }
    public required int KanjiFormOrder { get; init; }
    public required int FuriganaSegmentOrder { get; init; }

    public required int CompoundId { get; set; }
    public required string ReadingText { get; set; }

    [ForeignKey($"{nameof(EntryId)}, {nameof(ReadingOrder)}, {nameof(KanjiFormOrder)}, {nameof(FuriganaSegmentOrder)}")]
    public FuriganaSegment FuriganaSegment { get; init; } = null!;

    [ForeignKey($"{nameof(CompoundId)}, {nameof(ReadingText)}")]
    public CompoundReading Reading { get; set; } = null!;
}
