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

namespace Jitendex.Dto.JMnedict;

public sealed record SequenceDto
{
    public required int Id { get; init; }
    public required DateOnly CreatedDate { get; init; }
    public EntryDto? Entry { get; init; }
    public ImmutableArray<RevisionDto> Revisions { get; init; } = [];
}

public sealed record EntryDto
{
    public ImmutableArray<KanjiFormDto> KanjiForms { get; init; } = [];
    public ImmutableArray<ReadingDto> Readings { get; init; } = [];
    public ImmutableArray<TranslationDto> Translations { get; init; } = [];
}

public sealed record KanjiFormDto
{
    public required string Text { get; init; }
    public ImmutableArray<string> Infos { get; init; } = [];
    public ImmutableArray<string> Priorities { get; init; } = [];
}

public sealed record ReadingDto
{
    public required string Text { get; init; }
    public ImmutableArray<string> Infos { get; init; } = [];
    public ImmutableArray<string> Priorities { get; init; } = [];
    public ImmutableArray<string> Restrictions { get; init; } = [];
}

public sealed record TranslationDto
{
    public ImmutableArray<string> NameTypes { get; init; } = [];
    public ImmutableArray<string> Details { get; init; } = [];
    public ImmutableArray<string> CrossReferences { get; init; } = [];
}

public sealed record RevisionDto
(
    int Number,
    DateOnly Date,
    string DiffJson
);
