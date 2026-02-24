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
using Jitendex.SQLite;
using Jitendex.Tatoeba.Entities;
using Jitendex.Tatoeba.Import.Models;

namespace Jitendex.Tatoeba.Import.Tables;

internal sealed class RevisionTable : Table<DocumentRevision>
{
    protected override string Name => nameof(Revision);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Revision.SequenceId),
        nameof(Revision.Number),
        nameof(Revision.FileHeaderId),
        nameof(Revision.IsPriority),
        nameof(Revision.DiffJson),
    ];

    protected override IReadOnlyList<string> KeyColNames
        => throw new NotImplementedException($"The primary key for table {nameof(Revision)} is auto-incremented.");

    protected override SqliteParameter[] Parameters(DocumentRevision revision) =>
    [
        new("@0", revision.SequenceId),
        new("@1", revision.Number),
        new("@2", revision.FileHeaderId),
        new("@3", revision.IsPriority),
        new("@4", revision.DiffJson),
    ];
}
