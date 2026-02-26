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

namespace Jitendex.KanjiVG.Entities;

[Table(nameof(Stroke))]
[PrimaryKey(nameof(UnicodeScalarValue), nameof(VariantTypeId), nameof(Order))]
public sealed class Stroke
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required int Order { get; init; }
    public required string IdAttribute { get; set; }
    public required int ComponentOrder { get; set; }
    public required int? TypeId { get; set; }
    public required string PathData { get; set; }

    [ForeignKey($"{nameof(UnicodeScalarValue)}, {nameof(VariantTypeId)}, {nameof(ComponentOrder)}")]
    public required Component Component { get; set; }

    [ForeignKey(nameof(TypeId))]
    public required StrokeType? Type { get; set; }

    public string XmlIdAttribute()
        => $"kvg:{Component.Group.Variant.FileNameFormat()}-s{Order}";
}
