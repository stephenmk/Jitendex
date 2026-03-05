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

namespace Jitendex.MiscData.Entities.JMdict;

[Table(nameof(CrossReferenceSequence))]
[PrimaryKey(nameof(EntryId), nameof(SenseNumber), nameof(Text))]
public sealed class CrossReferenceSequence
{
    public required int EntryId { get; init; }
    public required int SenseNumber { get; init; }
    public required string Text { get; init; }
    public int? RefEntryId { get; set; }

    /// <summary>
    /// Dictionary key in the JSON file.
    /// </summary>
    public string ToExportKey()
        => $"{EntryId}・{SenseNumber}・{Text}";
}
