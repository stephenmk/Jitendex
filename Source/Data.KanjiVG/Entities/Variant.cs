// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Variant.cs, is part of Jitendex.
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

[Table(nameof(Variant))]
[PrimaryKey(nameof(UnicodeScalarValue), nameof(TypeId))]
public sealed class Variant
{
    public required int UnicodeScalarValue { get; init; }
    public required int TypeId { get; init; }
    public required int CommentId { get; set; }
    public ComponentGroup ComponentGroup { get; init; } = null!;
    public StrokeNumberGroup StrokeNumberGroup { get; init; } = null!;

    [ForeignKey(nameof(UnicodeScalarValue))]
    public Kanji Kanji { get; init; } = null!;

    [ForeignKey(nameof(TypeId))]
    public VariantType Type { get; init; } = null!;

    [ForeignKey(nameof(CommentId))]
    public Comment Comment { get; set; } = null!;

    public string FileNameFormat()
        => $"{UnicodeScalarValue:x5}{Type.FileNameFormat()}";

    public string FileName() => $"{FileNameFormat()}.svg";
}
