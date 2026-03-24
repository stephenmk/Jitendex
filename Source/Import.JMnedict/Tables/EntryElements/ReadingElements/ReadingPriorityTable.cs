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

using Jitendex.Data;
using Jitendex.Data.JMnedict.Entities.EntryItems.ReadingItems;
using Jitendex.Import.JMnedict.Models;

namespace Jitendex.Import.JMnedict.Tables.EntryElements.ReadingElements;

internal sealed class ReadingPriorityTable : Table<ReadingPriorityElement>
{
    protected override string Name { get; } = nameof(ReadingPriority);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(ReadingPriority.EntryId),
        nameof(ReadingPriority.ReadingOrder),
        nameof(ReadingPriority.Order),
        nameof(ReadingPriority.TagName),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(ReadingPriority.EntryId),
        nameof(ReadingPriority.ReadingOrder),
        nameof(ReadingPriority.Order),
    ];

    protected override object?[] ParameterValues(ReadingPriorityElement priority) =>
    [
        priority.EntryId,
        priority.ParentOrder,
        priority.Order,
        priority.TagName,
    ];
}
