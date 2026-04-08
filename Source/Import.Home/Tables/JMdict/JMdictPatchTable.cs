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
using Jitendex.Data.Home.Entities.JMdict;
using Jitendex.Import.Home.RowModels;

namespace Jitendex.Import.Home.Tables.JMdict;

internal sealed class JMdictPatchTable : Table<JMdictPatchRow>
{
    protected override string Name { get; } = nameof(JMdictPatch);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(JMdictPatch.Id),
        nameof(JMdictPatch.SequenceId),
        nameof(JMdictPatch.SequenceDate),
        nameof(JMdictPatch.CreatedAt),
        nameof(JMdictPatch.AuthorId),
        nameof(JMdictPatch.AuthorComment),
        nameof(JMdictPatch.PreviousPatchId),
        nameof(JMdictPatch.Json),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(JMdictPatch.Id)
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
