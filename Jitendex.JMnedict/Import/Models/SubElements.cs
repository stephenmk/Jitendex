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

namespace Jitendex.JMnedict.Import.Models;

internal interface ISubElement
{
    public int EntryId { get; init; }
    public int ParentOrder { get; init; }
    public int Order { get; init; }
}

internal sealed record KanjiFormInfoElement
    (int EntryId, int ParentOrder, int Order, string TagName) : ISubElement;

internal sealed record KanjiFormPriorityElement
    (int EntryId, int ParentOrder, int Order, string TagName) : ISubElement;

internal sealed record ReadingInfoElement
    (int EntryId, int ParentOrder, int Order, string TagName) : ISubElement;

internal sealed record ReadingPriorityElement
    (int EntryId, int ParentOrder, int Order, string TagName) : ISubElement;

internal sealed record RestrictionElement
    (int EntryId, int ParentOrder, int Order, string KanjiFormText) : ISubElement;

internal sealed record CrossReferenceElement
    (int EntryId, int ParentOrder, int Order, string Text) : ISubElement;

internal sealed record DetailElement
    (int EntryId, int ParentOrder, int Order, string Text) : ISubElement;

internal sealed record NameTypeElement
    (int EntryId, int ParentOrder, int Order, string TagName) : ISubElement;

internal static class SubElementExtensions
{
    public static (int, int, int) Key(this ISubElement x)
        => (x.EntryId, x.ParentOrder, x.Order);
}
