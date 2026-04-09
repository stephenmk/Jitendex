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
using Jitendex.Data.Export.Entities.TermChildren;

namespace Jitendex.Data.Export.Entities;

[Table(nameof(Term))]
[PrimaryKey(nameof(HeadwordId), nameof(GroupId))]
public sealed class Term
{
    public required int HeadwordId { get; init; }
    public required int GroupId { get; init; }
    public required int Score { get; set; }

    [ForeignKey(nameof(HeadwordId))]
    public Headword Headword { get; init; } = null!;

    [ForeignKey(nameof(GroupId))]
    public TermGroup Group { get; init; } = null!;

    [InverseProperty(nameof(TermNumber.Term))]
    public TermNumber Number { get; set; } = null!;

    [InverseProperty(nameof(TermGlossary.Term))]
    public TermGlossary? Glossary { get; set; }

    [InverseProperty(nameof(TermRedirect.Term))]
    public TermRedirect? Redirect { get; set; }

    [InverseProperty(nameof(TermRedirect.RedirectTerm))]
    public ICollection<TermRedirect> ReverseRedirects { get; init; } = [];

    [InverseProperty(nameof(TermRule.Term))]
    public ICollection<TermRule> Rules { get; init; } = [];

    [InverseProperty(nameof(TermTag.Term))]
    public List<TermTag> Tags { get; init; } = [];
}
