// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Patch.cs, is part of Jitendex.
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
using Jitendex.Data.Home.Entities.Attribution;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.Home.Entities.JMdict;

[Table(nameof(Patch))]
[PrimaryKey(nameof(Id))]
public sealed class Patch
{
    public required int Id { get; init; }
    public required int SequenceId { get; init; }
    public required DateOnly SequenceDate { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int AuthorId { get; init; }
    public required string AuthorComment { get; init; }
    public required int? PreviousPatchId { get; init; }
    public required byte[]? JsonDiff { get; init; }

    [ForeignKey(nameof(AuthorId))]
    public User Author { get; init; } = null!;

    [ForeignKey(nameof(PreviousPatchId))]
    public Patch? PreviousPatch { get; init; }

    [InverseProperty(nameof(PatchGraphic.Patch))]
    public List<PatchGraphic> GraphicPatches { get; init; } = [];

    [InverseProperty(nameof(PatchApproval.Patch))]
    public ICollection<PatchApproval> Approvals { get; init; } = [];

    [InverseProperty(nameof(PatchRecall.Patch))]
    public ICollection<PatchRecall> Recalls { get; init; } = [];
}
