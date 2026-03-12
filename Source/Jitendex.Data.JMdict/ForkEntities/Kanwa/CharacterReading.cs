/*
Copyright (c) 2025-2026 Stephen Kraus
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

namespace Jitendex.Data.JMdict.ForkEntities.Kanwa;

[Table(nameof(CharacterReading))]
[PrimaryKey(nameof(Id))]
[Index(nameof(CharacterValue), nameof(TypeId), nameof(Text), nameof(Okurigana), IsUnique = true)]
public sealed class CharacterReading
{
    public int Id { get; init; }
    public required int CharacterValue { get; init; }
    public required CharacterReadingTypeId TypeId { get; init; }
    public required string Text { get; init; }
    public required string? Okurigana { get; init; }
    public required bool IsPrefix { get; init; }
    public required bool IsSuffix { get; init; }

    [ForeignKey(nameof(CharacterValue))]
    public Character Character { get; init; } = null!;

    [ForeignKey(nameof(TypeId))]
    public CharacterReadingType Type { get; init; } = null!;

    [InverseProperty(nameof(DerivedCharacterReading.Source))]
    public ICollection<DerivedCharacterReading> DerivedReadings { get; init; } = [];
}
