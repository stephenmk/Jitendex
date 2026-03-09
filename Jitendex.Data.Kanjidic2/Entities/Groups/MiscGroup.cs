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
using Jitendex.Data.Kanjidic2.Entities.GroupItems;

namespace Jitendex.Data.Kanjidic2.Entities.Groups;

[PrimaryKey(nameof(UnicodeScalarValue), nameof(Order))]
public sealed class MiscGroup
{
    public required int UnicodeScalarValue { get; init; }
    public required int Order { get; init; }
    public int? Grade { get; set; }
    public int? Frequency { get; set; }
    public int? JlptLevel { get; set; }

    [ForeignKey(nameof(UnicodeScalarValue))]
    public Entry Entry { get; init; } = null!;

    [InverseProperty(nameof(RadicalName.Group))]
    public List<RadicalName> RadicalNames { get; init; } = [];

    [InverseProperty(nameof(StrokeCount.Group))]
    public List<StrokeCount> StrokeCounts { get; init; } = [];

    [InverseProperty(nameof(Variant.Group))]
    public List<Variant> Variants { get; init; } = [];
}
