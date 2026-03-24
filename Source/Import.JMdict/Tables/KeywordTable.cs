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
using Jitendex.Data.JMdict.Entities;
using Jitendex.Import.JMdict.Models;

namespace Jitendex.Import.JMdict.Tables;

internal sealed class KeywordTable<T> : Table<T> where T : IKeywordElement
{
    protected override string Name { get; } = ElementNameToEntityName(typeof(T).Name);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(IKeyword.Name),
        nameof(IKeyword.OriginFileId),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(IKeyword.Name)
    ];

    protected override object?[] ParameterValues(T keyword) =>
    [
        keyword.Name,
        keyword.OriginFileId,
    ];

#pragma warning disable format

    private static string ElementNameToEntityName(string elementName) => elementName switch
    {
        nameof(ReadingInfoTagElement)     => nameof(ReadingInfoTag),
        nameof(KanjiFormInfoTagElement)   => nameof(KanjiFormInfoTag),
        nameof(PartOfSpeechTagElement)    => nameof(PartOfSpeechTag),
        nameof(FieldTagElement)           => nameof(FieldTag),
        nameof(MiscTagElement)            => nameof(MiscTag),
        nameof(DialectTagElement)         => nameof(DialectTag),
        nameof(GlossTypeTagElement)       => nameof(GlossTypeTag),
        nameof(CrossReferenceTypeElement) => nameof(CrossReferenceType),
        nameof(LanguageSourceTypeElement) => nameof(LanguageSourceType),
        nameof(PriorityTagElement)        => nameof(PriorityTag),
        nameof(LanguageElement)           => nameof(Language),
        _ => throw new ArgumentOutOfRangeException(nameof(elementName), $"Value: `{elementName}`")
    };

#pragma warning restore format
}
