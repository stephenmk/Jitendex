// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, KanjiFormReferenceTable.cs, is part of Jitendex.
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
using Jitendex.Data.JMdict.ForkEntities.References;
using Jitendex.Forks.JMdict.TableRows;

namespace Jitendex.Forks.JMdict.Tables.References;

internal sealed class KanjiFormReferenceTable : Table<KanjiFormReferenceRow>
{
    protected override string Name { get; } = nameof(KanjiFormReference);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(KanjiFormReference.EntryId),
        nameof(KanjiFormReference.SenseOrder),
        nameof(KanjiFormReference.CrossReferenceOrder),
        nameof(KanjiFormReference.RefEntryId),
        nameof(KanjiFormReference.RefKanjiFormOrder),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(KanjiFormReference.EntryId),
        nameof(KanjiFormReference.SenseOrder),
        nameof(KanjiFormReference.CrossReferenceOrder),
        nameof(KanjiFormReference.RefEntryId),
    ];

    protected override object?[] ParameterValues(KanjiFormReferenceRow row) =>
    [
        row.EntryId,
        row.SenseOrder,
        row.CrossReferenceOrder,
        row.RefEntryId,
        row.RefKanjiFormOrder,
    ];
}
