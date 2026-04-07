/*
Copyright (c) 2026 Stephen Kraus
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

namespace Jitendex.Data.Export.Entities.GlossaryItems.SenseGroupItems;

[Table(nameof(Sense))]
[PrimaryKey(nameof(HeadwordId), nameof(GroupOrder), nameof(Order))]
public sealed class SenseGroupTag
{
    public required int HeadwordId { get; init; }
    public required int GroupOrder { get; init; }
    public required int Order { get; init; }
    public required string Class { get; set; }
    public required string Code { get; set; }
    public required string DisplayText { get; set; }
    public required string Description { get; set; }

    [ForeignKey($"{nameof(HeadwordId)}, {nameof(GroupOrder)}")]
    public SenseGroup Group { get; init; } = null!;
}
