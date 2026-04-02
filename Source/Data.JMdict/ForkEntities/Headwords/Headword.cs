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
using Jitendex.Data.JMdict.Entities;

namespace Jitendex.Data.JMdict.ForkEntities.Headwords;

[Table(nameof(Headword))]
[PrimaryKey(nameof(EntryId), nameof(Order))]
[Index(nameof(Surface), nameof(Reading), nameof(EntryId), IsUnique = true)]
public sealed class Headword
{
    public required int EntryId { get; set; }
    public required int Order { get; set; }
    public required int Score { get; set; }
    public required string Surface { get; set; }
    public required string? Reading { get; set; }
    public required int? ReadingOrder { get; set; }
    public required int? KanjiFormOrder { get; set; }

    [ForeignKey(nameof(EntryId))]
    public Entry Entry { get; init; } = null!;

    [InverseProperty(nameof(HeadwordNumber.Headword))]
    public HeadwordNumber Number { get; set; } = null!;

    [InverseProperty(nameof(HeadwordRedirect.Headword))]
    public HeadwordRedirect Redirect { get; set; } = null!;

    [InverseProperty(nameof(HeadwordRedirect.RedirectHeadword))]
    public ICollection<HeadwordRedirect> ReverseRedirects { get; init; } = [];

    [InverseProperty(nameof(HeadwordRule.Headword))]
    public ICollection<HeadwordRule> Rule { get; init; } = [];

    [InverseProperty(nameof(HeadwordSense.Headword))]
    public ICollection<HeadwordSense> Senses { get; init; } = [];

    [InverseProperty(nameof(HeadwordTag.Headword))]
    public ICollection<HeadwordTag> Tags { get; init; } = [];
}
