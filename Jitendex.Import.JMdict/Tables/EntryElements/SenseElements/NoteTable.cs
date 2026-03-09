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
using Jitendex.Data.JMdict.Entities.EntryItems.SenseItems;
using Jitendex.Import.JMdict.Models;

namespace Jitendex.Import.JMdict.Tables.EntryElements.SenseElements;

internal sealed class NoteTable : Table<NoteElement>
{
    protected override string Name => nameof(Note);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Note.EntryId),
        nameof(Note.SenseOrder),
        nameof(Note.Order),
        nameof(Note.Text),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Note.EntryId),
        nameof(Note.SenseOrder),
        nameof(Note.Order),
    ];

    protected override SqliteParameter[] Parameters(NoteElement note) =>
    [
        new("@0", note.EntryId),
        new("@1", note.ParentOrder),
        new("@2", note.Order),
        new("@3", note.Text),
    ];
}
