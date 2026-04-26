// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, HeadwordRedirect.cs, is part of Jitendex.
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

namespace Jitendex.Data.JMdict.ForkEntities.Headwords;

[Table(nameof(HeadwordRedirect))]
[PrimaryKey(nameof(EntryId), nameof(HeadwordOrder))]
public sealed class HeadwordRedirect
{
    public required int EntryId { get; init; }
    public required int HeadwordOrder { get; init; }
    public required int RedirectHeadwordOrder { get; set; }

    [ForeignKey($"{nameof(EntryId)}, {nameof(HeadwordOrder)}")]
    public Headword Headword { get; init; } = null!;

    [ForeignKey($"{nameof(EntryId)}, {nameof(RedirectHeadwordOrder)}")]
    public Headword RedirectHeadword { get; set; } = null!;
}
