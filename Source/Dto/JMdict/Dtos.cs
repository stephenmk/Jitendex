// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Dtos.cs, is part of Jitendex.
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

namespace Jitendex.Dto.JMdict;

public sealed record SequenceDto
{
    public required int Id { get; init; }
    public required DateOnly CreatedDate { get; init; }
    public EntryDto? Entry { get; init; }
    public List<RevisionDto> Revisions { get; init; } = [];
}

public sealed record EntryDto
{
    public List<KanjiFormDto> KanjiForms { get; init; } = [];
    public List<ReadingDto> Readings { get; init; } = [];
    public List<SenseDto> Senses { get; init; } = [];
}

public sealed record KanjiFormDto
{
    public required string Text { get; init; }
    public List<string> Infos { get; init; } = [];
    public List<string> Priorities { get; init; } = [];
}

public sealed record ReadingDto
{
    public required string Text { get; init; }
    public required bool NoKanji { get; init; }
    public List<string> Infos { get; init; } = [];
    public List<string> Priorities { get; init; } = [];
    public List<string> Restrictions { get; init; } = [];
}

public sealed record SenseDto
{
    public List<string> KanjiFormRestrictions { get; init; } = [];
    public List<string> ReadingRestrictions { get; init; } = [];
    public List<string> PartsOfSpeech { get; init; } = [];
    public List<string> Fields { get; init; } = [];
    public List<string> Miscs { get; init; } = [];
    public List<string> Dialects { get; init; } = [];
    public List<LanguageSourceDto> LanguageSources { get; init; } = [];
    public List<string> Notes { get; init; } = [];
    public List<GlossDto> Glosses { get; init; } = [];
    public List<CrossReferenceDto> CrossReferences { get; init; } = [];
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
