// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Elements.cs, is part of Jitendex.
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

using System.Text;

namespace Jitendex.Import.ChiseIds.Models;

internal sealed record CodepointElement
{
    public required string Id { get; init; }
    public required int? UnicodeScalarValue { get; init; }
    public required string? SequenceText { get; init; }
    public required string? AltSequenceText { get; init; }

    public string ToCharacter() => UnicodeScalarValue.HasValue
        ? new Rune(UnicodeScalarValue.Value).ToString()
        : Id;
}

internal sealed record ComponentElement
{
    public required string CodepointId { get; init; }
    public required int PositionId { get; init; }
}

internal sealed record ComponentPositionElement
(
    int Id,
    string Name
);

internal sealed record DescriptionSequenceElement
(
    string Text
);

internal sealed record SequenceComponentElement
{
    public required string SequenceText { get; init; }
    public required string CodepointId { get; init; }
    public required int PositionId { get; init; }
}

internal sealed record UnicodeCharacterElement(int ScalarValue)
{
    public Rune Character() => new(ScalarValue);
}
