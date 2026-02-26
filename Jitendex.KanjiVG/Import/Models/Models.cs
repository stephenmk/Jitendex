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

namespace Jitendex.KanjiVG.Import.Models;

internal sealed record EntryElement
(
    int UnicodeScalarValue
);

internal sealed record VariantElement
{
    public required int UnicodeScalarValue { get; init; }
    public required int TypeId { get; init; }
    public (int, int) Key() => (UnicodeScalarValue, TypeId);
}

internal sealed record ComponentGroupElement
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required int StyleId { get; init; }
    public required string IdAttribute { get; init; }
    public (int, int) Key() => (UnicodeScalarValue, VariantTypeId);
}

internal sealed record StrokeNumberGroupElement
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required int StyleId { get; init; }
    public required string IdAttribute { get; init; }
    public (int, int) Key() => (UnicodeScalarValue, VariantTypeId);
}

internal sealed record VariantCommentElement
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required int Order { get; init; }
    public required int CommentId { get; init; }
    public (int, int, int) Key() => (UnicodeScalarValue, VariantTypeId, Order);
}

internal sealed record ComponentElement
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required int Order { get; init; }
    public required int? ParentOrder { get; init; }
    public required string IdAttribute { get; init; }
    public required int? CharacterId { get; init; }
    public required bool IsVariant { get; init; }
    public required bool IsPartial { get; init; }
    public required int? OriginalId { get; init; }
    public required int? Part { get; init; }
    public required int? Number { get; init; }
    public required bool IsTradForm { get; init; }
    public required bool IsRadicalForm { get; init; }
    public required int? PositionId { get; init; }
    public required int? RadicalId { get; init; }
    public required int? PhonId { get; init; }
    public (int, int, int) Key() => (UnicodeScalarValue, VariantTypeId, Order);
}

internal sealed record StrokeElement
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required int Order { get; init; }
    public required int ComponentOrder { get; init; }
    public required string IdAttribute { get; init; }
    public required int TypeId { get; init; }
    public required string PathData { get; init; }
    public (int, int, int) Key() => (UnicodeScalarValue, VariantTypeId, Order);
}

internal sealed record StrokeNumberElement
{
    public required int UnicodeScalarValue { get; init; }
    public required int VariantTypeId { get; init; }
    public required int Order { get; init; }
    public required string Number { get; init; }
    public required string TransformAttribute { get; init; }
    public (int, int, int) Key() => (UnicodeScalarValue, VariantTypeId, Order);
}
