// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Component.cs, is part of Jitendex.
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
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.KanjiVG.Entities;

[Table(nameof(Component))]
[PrimaryKey(nameof(UnicodeScalarValue), nameof(VariantTypeId), nameof(Order))]
public sealed class Component
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required int Order { get; init; }
    public required string IdAttribute { get; set; }
    public required int? ParentOrder { get; set; }
    public required int? CharacterId { get; set; }
    public required bool IsVariant { get; set; }
    public required bool IsPartial { get; set; }
    public required int? OriginalId { get; set; }
    public required int? Part { get; set; }
    public required int? Number { get; set; }
    public required bool IsTradForm { get; set; }
    public required bool IsRadicalForm { get; set; }
    public required int? PositionId { get; set; }
    public required int? RadicalId { get; set; }
    public required int? PhonId { get; set; }

    [ForeignKey($"{nameof(UnicodeScalarValue)}, {nameof(VariantTypeId)}")]
    public required ComponentGroup Group { get; init; }

    [ForeignKey($"{nameof(UnicodeScalarValue)}, {nameof(VariantTypeId)}, {nameof(ParentOrder)}")]
    public required Component? Parent { get; set; }

    [ForeignKey(nameof(CharacterId))]
    public required ComponentCharacter? Character { get; set; }

    [ForeignKey(nameof(OriginalId))]
    public required ComponentOriginal? Original { get; set; }

    [ForeignKey(nameof(PositionId))]
    public required ComponentPosition? Position { get; set; }

    [ForeignKey(nameof(RadicalId))]
    public required ComponentRadical? Radical { get; set; }

    [ForeignKey(nameof(PhonId))]
    public required ComponentPhon? Phon { get; set; }

    [InverseProperty(nameof(Parent))]
    public List<Component> Children { get; init; } = [];

    [InverseProperty(nameof(Stroke.Component))]
    public List<Stroke> Strokes { get; init; } = [];

    public string XmlIdAttribute() => Order == 1
        ? $"kvg:{Group.Variant.FileNameFormat()}"
        : $"kvg:{Group.Variant.FileNameFormat()}-g{Order - 1}";

    public int ComponentCount()
        => 1 + Children.Sum(static c => c.ComponentCount());

    public int StrokeCount()
        => Strokes.Count + Children.Sum(static c => c.StrokeCount());
}
