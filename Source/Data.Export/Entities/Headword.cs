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
using Jitendex.Data.Export.Entities.HeadwordItems;

namespace Jitendex.Data.Export.Entities;

[Table(nameof(Headword))]
[PrimaryKey(nameof(Id))]
public sealed class Headword
{
    public required int Id { get; init; }
    public required string Surface { get; init; }
    public required string? Reading { get; init; }
    public required int Score { get; init; }

    #region HeadwordItems

    [InverseProperty(nameof(HeadwordNumber.Headword))]
    public HeadwordNumber Number { get; set; } = null!;

    [InverseProperty(nameof(HeadwordRedirect.Headword))]
    public HeadwordRedirect? Redirect { get; set; }

    [InverseProperty(nameof(HeadwordRedirect.RedirectHeadword))]
    public ICollection<HeadwordRedirect> ReverseRedirects { get; init; } = [];

    [InverseProperty(nameof(HeadwordRule.Headword))]
    public ICollection<HeadwordRule> Rules { get; init; } = [];

    [InverseProperty(nameof(HeadwordTag.Headword))]
    public List<HeadwordTag> Tags { get; init; } = [];

    public string Glossary { get; set; } = null!;

    #endregion
}
