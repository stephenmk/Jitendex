// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, StrokeNumberGroup.cs, is part of Jitendex.
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

[Table(nameof(StrokeNumberGroup))]
[PrimaryKey(nameof(UnicodeScalarValue), nameof(VariantTypeId))]
public sealed class StrokeNumberGroup
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required string IdAttribute { get; set; }
    public required int StyleId { get; set; }

    [ForeignKey($"{nameof(UnicodeScalarValue)}, {nameof(VariantTypeId)}")]
    public required Variant Variant { get; init; }

    [ForeignKey(nameof(StyleId))]
    public required StrokeNumberGroupStyle Style { get; set; }

    [InverseProperty(nameof(StrokeNumber.Group))]
    public List<StrokeNumber> StrokeNumbers { get; init; } = [];

    public string XmlIdAttribute()
        => $"kvg:StrokeNumbers_{Variant.FileNameFormat()}";
}
