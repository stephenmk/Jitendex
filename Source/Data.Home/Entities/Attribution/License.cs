// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, License.cs, is part of Jitendex.
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
using Jitendex.Data.Home.Entities.Imagery;

namespace Jitendex.Data.Home.Entities.Attribution;

[Table(nameof(License))]
public sealed class License
{
    [Key]
    public required LicenseId Id { get; init; }
    public required string Name { get; set; }
    public required string InfoUrl { get; set; }

    [InverseProperty(nameof(Graphic.License))]
    public ICollection<Graphic> Graphics { get; init; } = [];
}
