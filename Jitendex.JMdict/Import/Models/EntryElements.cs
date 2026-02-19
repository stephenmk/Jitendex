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

namespace Jitendex.JMdict.Import.Models;

internal sealed record EntryElement
{
    public required int Id { get; set; }

    public bool IsJmdictEntry() => Id switch
    {
        >= 1_000_000 and <= 3_000_000 => true,
        _ => false,
    };
}

internal interface IEntryElement
{
    public int EntryId { get; init; }
    public int Order { get; init; }
}

internal sealed record KanjiFormElement : IEntryElement
{
    public required int EntryId { get; init; }
    public required int Order { get; init; }
    public required string Text { get; set; }
}

internal sealed record ReadingElement : IEntryElement
{
    public required int EntryId { get; init; }
    public required int Order { get; init; }
    public required string Text { get; set; }
    public required bool NoKanji { get; set; }
}

internal sealed record SenseElement : IEntryElement
{
    public required int EntryId { get; init; }
    public required int Order { get; init; }
}

internal static class EntryElementExtensions
{
    public static (int, int) Key(this IEntryElement element)
        => (element.EntryId, element.Order);
}
