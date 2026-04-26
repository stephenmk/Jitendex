// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, TokenTable.cs, is part of Jitendex.
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
using Jitendex.Data.Tatoeba.Entities;
using Jitendex.Import.Tatoeba.TableRows;

namespace Jitendex.Import.Tatoeba.Tables;

internal sealed class TokenTable : Table<TokenRow>
{
    protected override string Name { get; } = nameof(Token);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(Token.ExampleId),
        nameof(Token.SegmentationOrder),
        nameof(Token.Order),
        nameof(Token.Headword),
        nameof(Token.Reading),
        nameof(Token.EntryId),
        nameof(Token.SenseNumber),
        nameof(Token.SentenceForm),
        nameof(Token.IsPriority),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(Token.ExampleId),
        nameof(Token.SegmentationOrder),
        nameof(Token.Order),
    ];

    protected override object?[] ParameterValues(TokenRow row) =>
    [
        row.ExampleId,
        row.SegmentationOrder,
        row.Order,
        row.Headword,
        row.Reading,
        row.EntryId,
        row.SenseNumber,
        row.SentenceForm,
        row.IsPriority,
    ];
}
