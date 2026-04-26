// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, StrokeCount.cs, is part of Jitendex.
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
using Jitendex.Data.Kanjidic2.Entities.Groups;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.Kanjidic2.Entities.GroupItems;

[PrimaryKey(nameof(UnicodeScalarValue), nameof(GroupOrder), nameof(Order))]
public sealed class StrokeCount
{
    public required int UnicodeScalarValue { get; init; }
    public required int GroupOrder { get; init; }
    public required int Order { get; init; }
    public required int Value { get; set; }

    [ForeignKey($"{nameof(UnicodeScalarValue)}, {nameof(GroupOrder)}")]
    public MiscGroup Group { get; init; } = null!;
}
