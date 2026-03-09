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
using Jitendex.Data.Tatoeba.Entities;
using Jitendex.Import.Tatoeba.Models;

namespace Jitendex.Import.Tatoeba.Tables;

internal sealed class FileHeaderTable : Table<DocumentHeader>
{
    protected override string Name => nameof(FileHeader);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(FileHeader.Date)
    ];

    protected override IReadOnlyList<string> KeyColNames
        => throw new NotImplementedException();

    protected override SqliteParameter[] Parameters(DocumentHeader header) =>
    [
        new("@0", header.Date)
    ];
}
