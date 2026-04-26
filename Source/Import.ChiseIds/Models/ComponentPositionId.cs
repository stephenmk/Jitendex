// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, ComponentPositionId.cs, is part of Jitendex.
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

using static Jitendex.Import.ChiseIds.Models.ComponentPositionId;

namespace Jitendex.Import.ChiseIds.Models;

internal enum ComponentPositionId : byte
{
    LeftHalf,
    RightHalf,
    TopHalf,
    BottomHalf,
    Left,
    VerticalCenter,
    Right,
    Top,
    HorizontalCenter,
    Bottom,
    FullSurrounding,
    FullSurrounded,
    AboveSurrounding,
    BelowSurrounded,
    BelowSurrounding,
    AboveSurrounded,
    LeftSurrounding,
    RightSurrounded,
    RightSurrounding,
    LeftSurrounded,
    UpperLeftSurrounding,
    LowerRightSurrounded,
    UpperRightSurrounding,
    LowerLeftSurrounded,
    LowerLeftSurrounding,
    UpperRightSurrounded,
    LowerRightSurrounding,
    UpperLeftSurrounded,
    Overlaying,
    Overlaid,
    UpperLeftAndRightSurrounding,
    LowerLeftAndRightSurrounded,
    LowerLeftAndRightSurrounding,
    UpperLeftAndRightSurrounded,
    UpperAndLowerSurrounding,
    LeftAndRightSurrounded,
}

internal static class ComponentPositionIdExtensions
{
    public static string ToName(this ComponentPositionId id) => id switch
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
