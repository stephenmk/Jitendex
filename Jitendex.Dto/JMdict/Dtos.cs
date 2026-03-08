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

namespace Jitendex.Dto.JMdict;

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
    public ImmutableArray<SenseDto> Senses { get; init; } = [];
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
    public required bool NoKanji { get; init; }
    public ImmutableArray<string> Infos { get; init; } = [];
    public ImmutableArray<string> Priorities { get; init; } = [];
    public ImmutableArray<string> Restrictions { get; init; } = [];
}

public sealed record SenseDto
{
    public ImmutableArray<string> KanjiFormRestrictions { get; init; } = [];
    public ImmutableArray<string> ReadingRestrictions { get; init; } = [];
    public ImmutableArray<string> PartsOfSpeech { get; init; } = [];
    public ImmutableArray<string> Fields { get; init; } = [];
    public ImmutableArray<string> Miscs { get; init; } = [];
    public ImmutableArray<string> Dialects { get; init; } = [];
    public ImmutableArray<LanguageSourceDto> LanguageSources { get; init; } = [];
    public ImmutableArray<string> Notes { get; init; } = [];
    public ImmutableArray<GlossDto> Glosses { get; init; } = [];
    public ImmutableArray<CrossReferenceDto> CrossReferences { get; init; } = [];
}

public sealed record RevisionDto
(
    DateOnly Date,
    string DiffJson
);

public sealed record LanguageSourceDto
(
    string? Text,
    string LanguageCode,
    string TypeName,
    bool IsWasei
);

public sealed record GlossDto
(
    string? TypeName,
    string Text
);

public sealed record CrossReferenceDto
(
    string TypeName,
    string Text
);
