// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, JMdictPatchTable.cs, is part of Jitendex.
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

using Jitendex.Data;
using Jitendex.Data.Home.Entities.JMdict;
using Jitendex.Import.Home.TableRows;

namespace Jitendex.Import.Home.Tables.JMdict;

internal sealed class JMdictPatchTable : Table<JMdictPatchRow>
{
    protected override string Name { get; } = nameof(Patch);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Patch.Id),
        nameof(Patch.SequenceId),
        nameof(Patch.SequenceDate),
        nameof(Patch.CreatedAt),
        nameof(Patch.AuthorId),
        nameof(Patch.AuthorComment),
        nameof(Patch.PreviousPatchId),
        nameof(Patch.JsonDiff),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Patch.Id)
    ];

    protected override object?[] ParameterValues(JMdictPatchRow row) =>
    [
        row.Id,
        row.SequenceId,
        row.SequenceDate,
        row.CreatedAt,
        row.AuthorId,
        row.AuthorComment,
        row.PreviousPatchId,
        row.Json,
    ];
}
