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

namespace Jitendex.Import.JMdict.TableRows;

internal interface IKeywordRow
{
    string Name { get; init; }
    int OriginFileId { get; init; }
}

internal sealed record ReadingInfoTagRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record KanjiFormInfoTagRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record PartOfSpeechTagRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record FieldTagRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record MiscTagRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record DialectTagRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record GlossTypeTagRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record CrossReferenceTypeRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record LanguageSourceTypeRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record PriorityTagRow(string Name, int OriginFileId) : IKeywordRow;
internal sealed record LanguageRow(string Name, int OriginFileId) : IKeywordRow;
