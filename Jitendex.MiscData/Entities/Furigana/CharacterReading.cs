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
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.MiscData.Entities.Furigana;

[Table(nameof(CharacterReading))]
[PrimaryKey(nameof(CharacterValue), nameof(Text))]
public sealed class CharacterReading
{
    public required int CharacterValue { get; init; }
    public required string Text { get; init; }
    public required bool IsPrefix { get; init; }
    public required bool IsSuffix { get; init; }
    public required string? Okurigana { get; init; }
    public required int TypeId { get; init; }

    [ForeignKey(nameof(CharacterValue))]
    public Character Character { get; init; } = null!;

    [ForeignKey(nameof(TypeId))]
    public CharacterReadingType Type { get; init; } = null!;

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IsSuffix)
        {
            sb.Append('-');
        }

        sb.Append(Text);

        if (Okurigana is not null)
        {
            sb.Append($".{Okurigana}");
        }

        if (IsPrefix)
        {
            sb.Append('-');
        }

        return sb.ToString();
    }
}
