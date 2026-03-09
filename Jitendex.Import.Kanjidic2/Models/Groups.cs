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

using System.Text;

namespace Jitendex.Import.Kanjidic2.Models;

internal sealed record EntryElement
{
    public required int Id { get; set; }
    public Rune ToRune() => new(Id);
}

internal interface IGroup
{
    int EntryId { get; init; }
    int Order { get; init; }
}

internal sealed record CodepointGroupElement(int EntryId, int Order) : IGroup;
internal sealed record DictionaryGroupElement(int EntryId, int Order) : IGroup;
internal sealed record QueryCodeGroupElement(int EntryId, int Order) : IGroup;
internal sealed record RadicalGroupElement(int EntryId, int Order) : IGroup;
internal sealed record ReadingMeaningGroupElement(int EntryId, int Order) : IGroup;

internal sealed record MiscGroupElement(int EntryId, int Order) : IGroup
{
    public int? Grade { get; set; }
    public int? Frequency { get; set; }
    public int? JlptLevel { get; set; }
}

internal static class GroupExtensions
{
    public static (int, int) Key(this IGroup group)
        => (group.EntryId, group.Order);

    public static Rune ToRune(this IGroup group)
        => new(group.EntryId);
}