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

using Microsoft.Data.Sqlite;
using Jitendex.Data;
using Jitendex.Forks.Tatoeba.Models;
using Jitendex.Data.Tatoeba.ForkEntities;

namespace Jitendex.Forks.Tatoeba.Tables;

internal sealed class EntryLinkTable : Table<EntryLinkRow>
{
    protected override string Name => nameof(EntryLink);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(EntryLink.ExampleId),
        nameof(EntryLink.SegmentationOrder),
        nameof(EntryLink.TokenOrder),
        nameof(EntryLink.EntryId),
        nameof(EntryLink.SenseOrder),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(EntryLink.ExampleId),
        nameof(EntryLink.SegmentationOrder),
        nameof(EntryLink.TokenOrder),
        nameof(EntryLink.EntryId),
    ];

    protected override SqliteParameter[] Parameters(EntryLinkRow row) =>
    [
        new("@0", row.ExampleId),
        new("@1", row.SegmentationOrder),
        new("@2", row.TokenOrder),
        new("@3", row.EntryId),
        new("@4", row.SenseOrder.Nullable()),
    ];
}
