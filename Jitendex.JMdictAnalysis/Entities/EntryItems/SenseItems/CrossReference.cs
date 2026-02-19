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

namespace Jitendex.JMdictAnalysis.Entities.EntryItems.SenseItems;

[Table(nameof(CrossReference))]
[PrimaryKey(nameof(EntryId), nameof(SenseOrder), nameof(Order))]
public sealed class CrossReference
{
    public required int EntryId { get; init; }
    public required int SenseOrder { get; init; }
    public required int Order { get; init; }
    public required string TypeName { get; set; }
    public required string Text { get; set; }

    public int? RefEntryId { get; set; }
    public int? RefReadingOrder { get; set; }
    public int? RefKanjiFormOrder { get; set; }
    public int? RefSenseOrder { get; set; }
    public bool? IsAmbiguous { get; set; }

    [ForeignKey($"{nameof(EntryId)}, {nameof(SenseOrder)}")]
    public Sense Sense { get; init; } = null!;

    [ForeignKey(nameof(TypeName))]
    public CrossReferenceType Type { get; set; } = null!;

    [ForeignKey($"{nameof(RefEntryId)}, {nameof(RefReadingOrder)}")]
    public Reading? ReferencedReading { get; set; }

    [ForeignKey($"{nameof(RefEntryId)}, {nameof(RefKanjiFormOrder)}")]
    public KanjiForm? ReferencedKanjiForm { get; set; }

    [ForeignKey($"{nameof(RefEntryId)}, {nameof(RefSenseOrder)}")]
    public Sense? ReferencedSense { get; set; }

    /// <summary>
    /// Stable and unique identifier for this reference in the raw data.
    /// </summary>
    public string ToExportKey() => $"{EntryId}・{SenseOrder + 1}・{Text}";
}
