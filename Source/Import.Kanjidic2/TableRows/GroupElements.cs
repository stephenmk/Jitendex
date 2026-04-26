// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, GroupElements.cs, is part of Jitendex.
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

namespace Jitendex.Import.Kanjidic2.TableRows;

internal interface IGroupElement
{
    int EntryId { get; init; }
    int GroupOrder { get; init; }
    int Order { get; init; }
}

internal sealed record CodepointElement(int EntryId, int GroupOrder, int Order, string Text, string TypeName) : IGroupElement;
internal sealed record DictionaryElement(int EntryId, int GroupOrder, int Order, string Text, string TypeName, int? Volume, int? Page) : IGroupElement;
internal sealed record NanoriElement(int EntryId, int GroupOrder, int Order, string Text) : IGroupElement;
internal sealed record QueryCodeElement(int EntryId, int GroupOrder, int Order, string Text, string TypeName, string? Misclassification) : IGroupElement;
internal sealed record RadicalElement(int EntryId, int GroupOrder, int Order, int Number, string TypeName) : IGroupElement;
internal sealed record RadicalNameElement(int EntryId, int GroupOrder, int Order, string Text) : IGroupElement;
internal sealed record StrokeCountElement(int EntryId, int GroupOrder, int Order, int Value) : IGroupElement;
internal sealed record VariantElement(int EntryId, int GroupOrder, int Order, string Text, string TypeName) : IGroupElement;

internal sealed record ReadingMeaningElement(int EntryId, int GroupOrder, int Order) : IGroupElement
{
    public bool IsKokuji { get; set; } = false;
    public bool IsGhost { get; set; } = false;
}

internal static class GroupElementExtensions
{
    public static (int, int, int) Key(this IGroupElement element)
        => (element.EntryId, element.GroupOrder, element.Order);

    public static Rune ToRune(this IGroupElement element)
        => new(element.EntryId);
}
