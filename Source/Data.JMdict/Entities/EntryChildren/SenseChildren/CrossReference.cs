// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, CrossReference.cs, is part of Jitendex.
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
using Jitendex.Data.JMdict.ForkEntities.References;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.JMdict.Entities.EntryChildren.SenseChildren;

[Table(nameof(CrossReference))]
[PrimaryKey(nameof(EntryId), nameof(SenseOrder), nameof(Order))]
public sealed class CrossReference
{
    public required int EntryId { get; init; }
    public required int SenseOrder { get; init; }
    public required int Order { get; init; }
    public required string TypeName { get; set; }
    public int? Sequence { get; set; }
    public string? Corpus { get; set; }
    public int? SenseNumber { get; set; }
    public string? KanjiForm { get; set; }
    public string? Reading { get; set; }
    public required string Text { get; set; }

    [ForeignKey($"{nameof(EntryId)}, {nameof(SenseOrder)}")]
    public Sense Sense { get; init; } = null!;

    [ForeignKey(nameof(TypeName))]
    public CrossReferenceType Type { get; set; } = null!;

    #region Fork Properties

    [InverseProperty(nameof(EntryReference.Source))]
    public EntryReference? ReferencedEntry { get; set; }

    #endregion
}
