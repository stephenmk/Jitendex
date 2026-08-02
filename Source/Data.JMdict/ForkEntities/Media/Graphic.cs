// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Graphic.cs, is part of Jitendex.
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jitendex.Data.JMdict.ForkEntities.Media;

[Table(nameof(Graphic))]
public sealed class Graphic
{
    [Key]
    public required int Id { get; init; }
    public required int LicenceId { get; set; }
    public required bool Cropped { get; set; }
    public required string PageUrl { get; set; }
    public required string FileUrl { get; set; }
    public required string Author { get; set; }
    public string? AuthorUrl { get; set; }
    public string? Title { get; set; }

    [ForeignKey(nameof(LicenceId))]
    public License License { get; set; } = null!;
}
