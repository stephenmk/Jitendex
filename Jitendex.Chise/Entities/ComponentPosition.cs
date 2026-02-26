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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Jitendex.Chise.Entities.ComponentPositionId;

namespace Jitendex.Chise.Entities;

[Table(nameof(ComponentPosition))]
public class ComponentPosition
{
    [Key]
    public required ComponentPositionId Id { get; init; }
    public string Name { get => IdToName(Id); }

    [InverseProperty(nameof(Component.Position))]
    public List<Component> Components { get; } = [];

    private static string IdToName(ComponentPositionId id) => id switch
    {
        LeftHalf => "Left Half",
        RightHalf => "Right Half",
        TopHalf => "Top Half",
        BottomHalf => "Bottom Half",
        Left => "Left",
        VerticalCenter => "Vertical Center",
        Right => "Right",
        Top => "Top",
        HorizontalCenter => "Horizontal Center",
        Bottom => "Bottom",
        FullSurrounding => "Full Surrounding",
        FullSurrounded => "Full Surrounded",
        AboveSurrounding => "Above Surrounding",
        BelowSurrounded => "Below Surrounded",
        BelowSurrounding => "Below Surrounding",
        AboveSurrounded => "Above Surrounded",
        LeftSurrounding => "Left Surrounding",
        RightSurrounded => "Right Surrounded",
        RightSurrounding => "Right Surrounding",
        LeftSurrounded => "Left Surrounded",
        UpperLeftSurrounding => "Upper-Left Surrounding",
        LowerRightSurrounded => "Lower-Right Surrounded",
        UpperRightSurrounding => "Upper-Right Surrounding",
        LowerLeftSurrounded => "Lower-Left Surrounded",
        LowerLeftSurrounding => "Lower-Left Surrounding",
        UpperRightSurrounded => "Upper-Right Surrounded",
        LowerRightSurrounding => "Lower-Right Surrounding",
        UpperLeftSurrounded => "Upper-Left Surrounded",
        Overlaying => "Overlaying",
        Overlaid => "Overlaid",
        UpperLeftAndRightSurrounding => "Upper-Left And Upper-Right Surrounding",
        LowerLeftAndRightSurrounded => "Lower-Left and Lower-Right Surrounded",
        LowerLeftAndRightSurrounding => "Lower-Left and Lower-Right Surrounding",
        UpperLeftAndRightSurrounded => "Upper-Left and Upper-Right Surrounded",
        UpperAndLowerSurrounding => "Upper and Lower Surrounding",
        LeftAndRightSurrounded => "Left and Right Surrounded",
        _ => throw new ArgumentOutOfRangeException(nameof(id)),
    };
}
