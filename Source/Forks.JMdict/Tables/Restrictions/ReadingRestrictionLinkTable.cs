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

using Jitendex.Data;
using Jitendex.Forks.JMdict.Models;
using Jitendex.Data.JMdict.ForkEntities.Links;

namespace Jitendex.Forks.JMdict.Tables.Restrictions;

internal sealed class ReadingRestrictionLinkTable : Table<ReadingRestrictionLinkRow>
{
    protected override string Name => nameof(ReadingRestrictionLink);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(ReadingRestrictionLink.EntryId),
        nameof(ReadingRestrictionLink.SenseOrder),
        nameof(ReadingRestrictionLink.RestrictionOrder),
        nameof(ReadingRestrictionLink.ReadingOrder),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(ReadingRestrictionLink.EntryId),
        nameof(ReadingRestrictionLink.SenseOrder),
        nameof(ReadingRestrictionLink.RestrictionOrder),
    ];

    protected override object?[] ParameterValues(ReadingRestrictionLinkRow row) =>
    [
        row.EntryId,
        row.SenseOrder,
        row.RestrictionOrder,
        row.ReadingOrder,
    ];
}
