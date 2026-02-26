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

[Table(nameof(Variant))]
[PrimaryKey(nameof(UnicodeScalarValue), nameof(TypeId))]
public sealed class Variant
{
    public required int UnicodeScalarValue { get; init; }
    public required int TypeId { get; init; }

    public ComponentGroup ComponentGroup { get; init; } = null!;
    public StrokeNumberGroup StrokeNumberGroup { get; init; } = null!;

    [ForeignKey(nameof(UnicodeScalarValue))]
    public Entry Entry { get; init; } = null!;

    [ForeignKey(nameof(TypeId))]
    public VariantType Type { get; init; } = null!;

    [InverseProperty(nameof(VariantComment.Variant))]
    public List<VariantComment> Comments { get; set; } = [];

    public string FileNameFormat()
        => $"{UnicodeScalarValue:x5}{Type.FileNameFormat()}";

    public string FileName() => $"{FileNameFormat()}.svg";
}
