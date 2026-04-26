// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, TermRedirect.cs, is part of Jitendex.
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

namespace Jitendex.Data.Export.Entities.TermChildren;

[Table(nameof(TermRedirect))]
[PrimaryKey(nameof(HeadwordId), nameof(TermGroupId))]
public sealed class TermRedirect
{
    public required int HeadwordId { get; init; }
    public required int TermGroupId { get; init; }
    public required int RedirectHeadwordId { get; set; }
    public required int RedirectTermGroup { get; set; }

    [ForeignKey($"{nameof(HeadwordId)}, {nameof(TermGroupId)}")]
    public Term Term { get; init; } = null!;

    [ForeignKey($"{nameof(RedirectHeadwordId)}, {nameof(RedirectTermGroup)}")]
    public Term RedirectTerm { get; set; } = null!;
}
