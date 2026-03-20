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

using Microsoft.Data.Sqlite;
using Jitendex.Data;
using Jitendex.Data.JMnedict.Entities;
using Jitendex.Import.JMnedict.Models;

namespace Jitendex.Import.JMnedict.Tables;

internal sealed class SequenceTable : Table<DocumentSequence>
{
    protected override string Name => nameof(Sequence);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Sequence.Id),
        nameof(Sequence.OriginFileId),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Sequence.Id)
    ];

    protected override SqliteParameter[] Parameters(DocumentSequence sequence) =>
    [
        new("@0", sequence.Id),
        new("@1", sequence.FileHeaderId),
    ];
}
