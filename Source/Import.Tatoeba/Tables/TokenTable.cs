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
using Jitendex.Data.Tatoeba.Entities;
using Jitendex.Import.Tatoeba.Models;

namespace Jitendex.Import.Tatoeba.Tables;

internal sealed class TokenTable : Table<TokenElement>
{
    protected override string Name => nameof(Token);

    protected override IReadOnlyList<string> ColumnNames =>
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

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Token.ExampleId),
        nameof(Token.SegmentationOrder),
        nameof(Token.Order),
    ];

    protected override object?[] ParameterValues(TokenElement token) =>
    [
        token.ExampleId,
        token.SegmentationOrder,
        token.Order,
        token.Headword,
        token.Reading,
        token.EntryId,
        token.SenseNumber,
        token.SentenceForm,
        token.IsPriority,
    ];
}
