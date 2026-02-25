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
using Jitendex.JMdict.Entities;
using Jitendex.JMdict.Import.Models;

namespace Jitendex.JMdict.Import.Tables;

internal sealed class KeywordTable<T> : Table<T> where T : IKeywordElement
{
    protected override string Name => ElementNameToEntityName(typeof(T).Name);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(IKeyword.Name),
        nameof(IKeyword.OriginFileId),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(IKeyword.Name)
    ];

    protected override SqliteParameter[] Parameters(T keyword) =>
    [
        new("@0", keyword.Name),
        new("@1", keyword.OriginFileId),
    ];

    private static string ElementNameToEntityName(string elementName) => elementName switch
    {
        nameof(ReadingInfoTagElement) => nameof(ReadingInfoTag),
        nameof(KanjiFormInfoTagElement) => nameof(KanjiFormInfoTag),
        nameof(PartOfSpeechTagElement) => nameof(PartOfSpeechTag),
        nameof(FieldTagElement) => nameof(FieldTag),
        nameof(MiscTagElement) => nameof(MiscTag),
        nameof(DialectTagElement) => nameof(DialectTag),
        nameof(GlossTypeElement) => nameof(GlossType),
        nameof(CrossReferenceTypeElement) => nameof(CrossReferenceType),
        nameof(LanguageSourceTypeElement) => nameof(LanguageSourceType),
        nameof(PriorityTagElement) => nameof(PriorityTag),
        nameof(LanguageElement) => nameof(Language),
        _ => throw new ArgumentOutOfRangeException(nameof(elementName), $"Value: `{elementName}`")
    };
}
