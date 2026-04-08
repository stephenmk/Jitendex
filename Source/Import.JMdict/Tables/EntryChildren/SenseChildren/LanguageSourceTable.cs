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
using Jitendex.Data.JMdict.Entities.EntryChildren.SenseChildren;
using Jitendex.Import.JMdict.TableRows;

namespace Jitendex.Import.JMdict.Tables.EntryChildren.SenseChildren;

internal sealed class LanguageSourceTable : Table<LanguageSourceRow>
{
    protected override string Name { get; } = nameof(LanguageSource);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(LanguageSource.EntryId),
        nameof(LanguageSource.SenseOrder),
        nameof(LanguageSource.Order),
        nameof(LanguageSource.Text),
        nameof(LanguageSource.LanguageCode),
        nameof(LanguageSource.TypeName),
        nameof(LanguageSource.IsWasei),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(LanguageSource.EntryId),
        nameof(LanguageSource.SenseOrder),
        nameof(LanguageSource.Order),
    ];

    protected override object?[] ParameterValues(LanguageSourceRow row) =>
    [
        row.EntryId,
        row.ParentOrder,
        row.Order,
        row.Text,
        row.LanguageCode,
        row.TypeName,
        row.IsWasei,
    ];
}
