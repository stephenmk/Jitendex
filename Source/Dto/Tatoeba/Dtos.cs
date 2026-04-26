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

namespace Jitendex.Dto.Tatoeba;

public sealed record RevisionDto
(
    int Number,
    DateOnly CreatedDate,
    bool IsPriority,
    string DiffJson
);

public sealed record SequenceDto
{
    public required int Id { get; init; }
    public required DateOnly CreatedDate { get; init; }
    public ExampleDto? Example { get; init; }
    public List<RevisionDto> Revisions { get; init; } = [];
}

public sealed record ExampleDto
{
    public required string Text { get; init; }
    public List<SegmentationDto> Segmentations { get; init; } = [];
}

public sealed record SegmentationDto
{
    public required TranslationDto Translation { get; init; }
    public List<TokenDto> Tokens { get; init; } = [];
}

public sealed record TranslationDto
(
    int Id,
    string Text
);

public sealed record TokenDto
(
    string Headword,
    string? Reading,
    int? EntryId,
    int? SenseNumber,
    string? SentenceForm,
    bool IsPriority
);
