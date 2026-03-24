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
using Jitendex.Import.JMnedict.Models;

namespace Jitendex.Import.JMnedict.Tables;

internal sealed class RevisionTable : Table<DocumentRevision>
{
    protected override string Name => nameof(Revision);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Revision.SequenceId),
        nameof(Revision.Number),
        nameof(Revision.FileHeaderId),
        nameof(Revision.DiffJson),
    ];

    protected override IReadOnlyList<string> KeyColNames
        => throw new NotImplementedException($"The primary key for table {nameof(Revision)} is auto-incremented.");

    protected override object?[] ParameterValues(DocumentRevision revision) =>
    [
        revision.SequenceId,
        revision.Number,
        revision.FileHeaderId,
        revision.DiffJson,
    ];
}
