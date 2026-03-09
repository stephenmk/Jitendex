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

namespace Jitendex.Dto.Kanjidic2;

public sealed record SequenceDto
{
    public required int Id { get; init; }
    public required DateOnly CreatedDate { get; init; }
    public ImmutableArray<RevisionDto> Revisions { get; init; } = [];
    public EntryDto? Entry { get; init; }
}

public sealed record RevisionDto
(
    int Number,
    DateOnly CreatedDate,
    string DiffJson
);

public sealed record EntryDto
{
    public ImmutableArray<CodepointGroupDto> CodepointGroups { get; init; } = [];
    public ImmutableArray<DictionaryGroupDto> DictionaryGroups { get; init; } = [];
    public ImmutableArray<MiscGroupDto> MiscGroups { get; init; } = [];
    public ImmutableArray<QueryCodeGroupDto> QueryCodeGroups { get; init; } = [];
    public ImmutableArray<RadicalGroupDto> RadicalGroups { get; init; } = [];
    public ImmutableArray<ReadingMeaningGroupDto> ReadingMeaningGroups { get; init; } = [];
}

public sealed record CodepointGroupDto
{
    public ImmutableArray<CodepointDto> Codepoints { get; init; } = [];
}

public sealed record DictionaryGroupDto
{
    public ImmutableArray<DictionaryDto> Dictionaries { get; init; } = [];
}

public sealed record MiscGroupDto
{
    public required int? Grade { get; init; }
    public required int? Frequency { get; init; }
    public required int? JlptLevel { get; init; }
    public ImmutableArray<string> RadicalNames { get; init; } = [];
    public ImmutableArray<int> StrokeCounts { get; init; } = [];
    public ImmutableArray<VariantDto> Variants { get; init; } = [];
}

public sealed record QueryCodeGroupDto
{
    public ImmutableArray<QueryCodeDto> QueryCodes { get; init; } = [];
}

public sealed record RadicalGroupDto
{
    public ImmutableArray<RadicalDto> Radicals { get; init; } = [];
}

public sealed record ReadingMeaningGroupDto
{
    public ImmutableArray<ReadingMeaningDto> ReadingMeanings { get; init; } = [];
    public ImmutableArray<string> Nanoris { get; init; } = [];
}

public sealed record ReadingMeaningDto
{
    public required bool IsKokuji { get; init; }
    public required bool IsGhost { get; init; }
    public ImmutableArray<string> Meanings { get; init; } = [];
    public ImmutableArray<ReadingDto> Readings { get; init; } = [];
}

public sealed record CodepointDto
(
    string Text,
    string TypeName
);

public sealed record DictionaryDto
(
    string Text,
    string TypeName,
    int? Volume,
    int? Page
);

public sealed record QueryCodeDto
(
    string Text,
    string TypeName,
    string? Misclassification
);

public sealed record RadicalDto
(
    int Number,
    string TypeName
);

public sealed record VariantDto
(
    string Text,
    string TypeName
);

public sealed record ReadingDto
(
    string Text,
    string TypeName
);
