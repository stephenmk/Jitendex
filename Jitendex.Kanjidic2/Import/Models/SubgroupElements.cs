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

namespace Jitendex.Kanjidic2.Import.Models;

internal interface ISubgroupElement
{
    int EntryId { get; init; }
    int GroupOrder { get; init; }
    int ReadingMeaningOrder { get; init; }
    int Order { get; init; }
}

internal sealed record MeaningElement(int EntryId, int GroupOrder, int ReadingMeaningOrder, int Order, string Text) : ISubgroupElement;
internal sealed record ReadingElement(int EntryId, int GroupOrder, int ReadingMeaningOrder, int Order, string Text, string TypeName) : ISubgroupElement;

internal static class SubgroupElementExtensions
{
    public static (int, int, int, int) Key(this ISubgroupElement element)
        => (element.EntryId, element.GroupOrder, element.ReadingMeaningOrder, element.Order);
}
