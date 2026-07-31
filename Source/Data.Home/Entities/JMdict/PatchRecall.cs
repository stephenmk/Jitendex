// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, PatchRecall.cs, is part of Jitendex.
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

[Table(nameof(PatchRecall))]
[PrimaryKey(nameof(PatchId), nameof(RecallerId), nameof(CreatedAt))]
public sealed class PatchRecall
{
    public required int PatchId { get; init; }
    public required int RecallerId { get; init; }
    public required DateTime CreatedAt { get; init; }

    [ForeignKey(nameof(PatchId))]
    public Patch Patch { get; init; } = null!;

    [ForeignKey(nameof(RecallerId))]
    public User Recaller { get; init; } = null!;
}
