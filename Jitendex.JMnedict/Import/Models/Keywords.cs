/*
Copyright (c) 2026 Stephen Kraus
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

namespace Jitendex.JMnedict.Import.Models;

internal interface IKeywordElement
{
    string Name { get; init; }
    DateOnly Date { get; init; }
}

internal sealed record PriorityTagElement(string Name, DateOnly Date) : IKeywordElement;
internal sealed record ReadingInfoTagElement(string Name, DateOnly Date) : IKeywordElement;
internal sealed record KanjiFormInfoTagElement(string Name, DateOnly Date) : IKeywordElement;
internal sealed record NameTypeTagElement(string Name, DateOnly Date) : IKeywordElement;
internal sealed record DetailLanguageElement(string Name, DateOnly Date) : IKeywordElement;
internal sealed record CrossReferenceTypeElement(string Name, DateOnly Date) : IKeywordElement;
