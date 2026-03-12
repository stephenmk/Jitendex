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
using Jitendex.Forks.JMdict.Models;
using Jitendex.Data.JMdict.ForkEntities.References;

namespace Jitendex.Forks.JMdict.Tables.References;

internal sealed class AmbiguousReferenceTable : Table<AmbiguousReferenceRow>
{
    protected override string Name => nameof(AmbiguousReference);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(AmbiguousReference.EntryId),
        nameof(AmbiguousReference.SenseOrder),
        nameof(AmbiguousReference.CrossReferenceOrder),
    ];

    protected override IReadOnlyList<string> KeyColNames => ColumnNames;

    protected override SqliteParameter[] Parameters(AmbiguousReferenceRow row) =>
    [
        new("@0", row.EntryId),
        new("@1", row.SenseOrder),
        new("@2", row.CrossReferenceOrder),
    ];
}
