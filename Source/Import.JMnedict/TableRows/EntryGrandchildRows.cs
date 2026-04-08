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

namespace Jitendex.Import.JMnedict.TableRows;

internal interface IEntryGrandchildRow
{
    public int EntryId { get; init; }
    public int ParentOrder { get; init; }
    public int Order { get; init; }
}

internal sealed record KanjiFormInfoRow
    (int EntryId, int ParentOrder, int Order, string TagName) : IEntryGrandchildRow;

internal sealed record KanjiFormPriorityRow
    (int EntryId, int ParentOrder, int Order, string TagName) : IEntryGrandchildRow;

internal sealed record ReadingInfoRow
    (int EntryId, int ParentOrder, int Order, string TagName) : IEntryGrandchildRow;

internal sealed record ReadingPriorityRow
    (int EntryId, int ParentOrder, int Order, string TagName) : IEntryGrandchildRow;

internal sealed record RestrictionRow
    (int EntryId, int ParentOrder, int Order, string KanjiFormText) : IEntryGrandchildRow;

internal sealed record CrossReferenceRow
    (int EntryId, int ParentOrder, int Order, string Text) : IEntryGrandchildRow;

internal sealed record DetailRow
    (int EntryId, int ParentOrder, int Order, string Text, string? LanguageName) : IEntryGrandchildRow;

internal sealed record NameTypeRow
    (int EntryId, int ParentOrder, int Order, string TagName) : IEntryGrandchildRow;

internal static class EntryGrandchildRowExtensions
{
    public static (int, int, int) Key(this IEntryGrandchildRow x)
        => (x.EntryId, x.ParentOrder, x.Order);
}
