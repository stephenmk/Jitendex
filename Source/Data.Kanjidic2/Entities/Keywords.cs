// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Keywords.cs, is part of Jitendex.
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Jitendex.Data.Kanjidic2.Entities.GroupItems;
using Jitendex.Data.Kanjidic2.Entities.SubgroupItems;

namespace Jitendex.Data.Kanjidic2.Entities;

public interface IKeyword
{
    string Name { get; init; }
    int OriginFileId { get; init; }
    FileHeader OriginFile { get; init; }
}

[Table(nameof(CodepointType))]
public sealed class CodepointType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Codepoint.Type))]
    public List<Codepoint> Codepoints { get; init; } = [];
}

[Table(nameof(DictionaryType))]
public sealed class DictionaryType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Dictionary.Type))]
    public List<Dictionary> Dictionaries { get; init; } = [];
}

[Table(nameof(QueryCodeType))]
public sealed class QueryCodeType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(QueryCode.Type))]
    public List<QueryCode> QueryCodes { get; init; } = [];
}

[Table(nameof(MisclassificationType))]
public sealed class MisclassificationType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(QueryCode.MisclassificationType))]
    public List<QueryCode> QueryCodes { get; init; } = [];
}

[Table(nameof(RadicalType))]
public sealed class RadicalType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Radical.Type))]
    public List<Radical> Radicals { get; init; } = [];
}

[Table(nameof(ReadingType))]
public sealed class ReadingType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Reading.Type))]
    public List<Reading> Readings { get; init; } = [];
}

[Table(nameof(VariantType))]
public sealed class VariantType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Variant.Type))]
    public List<Variant> Variants { get; init; } = [];
}
