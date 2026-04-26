// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, ReadingMeaningGroup.cs, is part of Jitendex.
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
using Jitendex.Data.Kanjidic2.Entities.GroupItems;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.Kanjidic2.Entities.Groups;

[PrimaryKey(nameof(UnicodeScalarValue), nameof(Order))]
public sealed class ReadingMeaningGroup
{
    public required int UnicodeScalarValue { get; init; }
    public required int Order { get; init; }

    [ForeignKey(nameof(UnicodeScalarValue))]
    public Entry Entry { get; init; } = null!;

    [InverseProperty(nameof(ReadingMeaning.Group))]
    public List<ReadingMeaning> ReadingMeanings { get; init; } = [];

    [InverseProperty(nameof(Nanori.Group))]
    public List<Nanori> Nanoris { get; init; } = [];
}
