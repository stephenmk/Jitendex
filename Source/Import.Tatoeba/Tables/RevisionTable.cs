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
using Jitendex.Data.Tatoeba.Entities;
using Jitendex.Import.Tatoeba.TableRows;

namespace Jitendex.Import.Tatoeba.Tables;

internal sealed class RevisionTable : Table<RevisionRow>
{
    protected override string Name { get; } = nameof(Revision);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Revision.SequenceId),
        nameof(Revision.Number),
        nameof(Revision.FileHeaderId),
        nameof(Revision.IsPriority),
        nameof(Revision.DiffJson),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Revision.SequenceId),
        nameof(Revision.Number),
    ];

    protected override object?[] ParameterValues(RevisionRow row) =>
    [
        row.SequenceId,
        row.Number,
        row.FileHeaderId,
        row.IsPriority,
        row.DiffJson,
    ];
}
