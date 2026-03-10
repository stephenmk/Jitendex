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

namespace Jitendex.JMdict.Fork.Entities.EntryItems.SenseItems;

[Table(nameof(ReadingRestrictionLink))]
[PrimaryKey(nameof(EntryId), nameof(SenseOrder), nameof(RestrictionOrder))]
public sealed class ReadingRestrictionLink
{
    public required int EntryId { get; init; }
    public required int SenseOrder { get; init; }
    public required int RestrictionOrder { get; init; }
    public required int ReadingOrder { get; set; }

    [ForeignKey($"{nameof(EntryId)}, {nameof(SenseOrder)}, {nameof(RestrictionOrder)}")]
    public ReadingRestriction Source { get; init; } = null!;

    [ForeignKey($"{nameof(EntryId)}, {nameof(ReadingOrder)}")]
    public Reading Reading { get; set; } = null!;
}
