// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, EntryChildrenRows.cs, is part of Jitendex.
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

namespace Jitendex.Import.JMdict.TableRows;

internal interface IEntryChildRow
{
    int EntryId { get; init; }
    int Order { get; init; }
}

internal sealed record KanjiFormRow : IEntryChildRow
{
    public required int EntryId { get; init; }
    public required int Order { get; init; }
    public required string Text { get; set; }
}

internal sealed record ReadingRow : IEntryChildRow
{
    public required int EntryId { get; init; }
    public required int Order { get; init; }
    public required string Text { get; set; }
    public required bool NoKanji { get; set; }
}

internal sealed record SenseRow : IEntryChildRow
{
    public required int EntryId { get; init; }
    public required int Order { get; init; }
}

internal static class EntryChildRowExtensions
{
    public static (int, int) Key(this IEntryChildRow element)
        => (element.EntryId, element.Order);
}
