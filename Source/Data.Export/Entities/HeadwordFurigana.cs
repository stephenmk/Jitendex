// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, HeadwordFurigana.cs, is part of Jitendex.
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

namespace Jitendex.Data.Export.Entities;

[Table(nameof(HeadwordFurigana))]
[PrimaryKey(nameof(HeadwordId), nameof(Order))]
public sealed class HeadwordFurigana
{
    public required int HeadwordId { get; init; }
    public required int Order { get; init; }
    public required string BaseText { get; set; }
    public required string? RubyText { get; set; }

    [ForeignKey(nameof(HeadwordId))]
    public Headword Headword { get; init; } = null!;
}
