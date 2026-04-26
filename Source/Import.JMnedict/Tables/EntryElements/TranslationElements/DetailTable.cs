// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DetailTable.cs, is part of Jitendex.
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
using Jitendex.Data.JMnedict.Entities.EntryChildren.TranslationChildren;
using Jitendex.Import.JMnedict.TableRows;

namespace Jitendex.Import.JMnedict.Tables.EntryElements.TranslationElements;

internal sealed class DetailTable : Table<DetailRow>
{
    protected override string Name { get; } = nameof(Detail);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Detail.EntryId),
        nameof(Detail.TranslationOrder),
        nameof(Detail.Order),
        nameof(Detail.Text),
        nameof(Detail.LanguageName),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Detail.EntryId),
        nameof(Detail.TranslationOrder),
        nameof(Detail.Order),
    ];

    protected override object?[] ParameterValues(DetailRow row) =>
    [
        row.EntryId,
        row.ParentOrder,
        row.Order,
        row.Text,
        row.LanguageName,
    ];
}
