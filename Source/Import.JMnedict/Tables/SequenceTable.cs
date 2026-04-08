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
using Jitendex.Data.JMnedict.Entities;
using Jitendex.Import.JMnedict.TableRows;

namespace Jitendex.Import.JMnedict.Tables;

internal sealed class SequenceTable : Table<DocumentSequence>
{
    protected override string Name { get; } = nameof(Sequence);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Sequence.Id),
        nameof(Sequence.OriginFileId),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Sequence.Id)
    ];

    protected override object?[] ParameterValues(DocumentSequence row) =>
    [
        row.Id,
        row.FileHeaderId,
    ];
}
