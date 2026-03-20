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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Jitendex.Data.JMdict.Entities.EntryItems.KanjiFormItems;
using Jitendex.Data.JMdict.Entities.EntryItems.ReadingItems;
using Jitendex.Data.JMdict.Entities.EntryItems.SenseItems;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Jitendex.Data.JMdict.Entities;

public interface IKeyword
{
    string Name { get; init; }
    int OriginFileId { get; init; }
    FileHeader OriginFile { get; init; }
}

[Table(nameof(ReadingInfoTag))]
public sealed class ReadingInfoTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(ReadingInfo.Tag))]
    public List<ReadingInfo> Infos { get; init; } = [];
}

[Table(nameof(KanjiFormInfoTag))]
public sealed class KanjiFormInfoTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(KanjiFormInfo.Tag))]
    public List<KanjiFormInfo> Infos { get; init; } = [];
}

[Table(nameof(PartOfSpeechTag))]
public sealed class PartOfSpeechTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(PartOfSpeech.Tag))]
    public List<PartOfSpeech> PartsOfSpeech { get; init; } = [];
}

[Table(nameof(FieldTag))]
public sealed class FieldTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Field.Tag))]
    public List<Field> Fields { get; init; } = [];
}

[Table(nameof(MiscTag))]
public sealed class MiscTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Misc.Tag))]
    public List<Misc> Miscs { get; init; } = [];
}

[Table(nameof(DialectTag))]
public sealed class DialectTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Dialect.Tag))]
    public List<Dialect> Dialects { get; init; } = [];
}

[Table(nameof(GlossTypeTag))]
public sealed class GlossTypeTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(GlossType.Tag))]
    public List<GlossType> Types { get; init; } = [];
}

[Table(nameof(CrossReferenceType))]
public sealed class CrossReferenceType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(CrossReference.Type))]
    public List<CrossReference> CrossReferences { get; init; } = [];
}

[Table(nameof(LanguageSourceType))]
public sealed class LanguageSourceType : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(LanguageSource.Type))]
    public List<LanguageSource> LanguageSources { get; init; } = [];
}

[Table(nameof(PriorityTag))]
public sealed class PriorityTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(KanjiFormPriority.Tag))]
    public List<KanjiFormPriority> KanjiFormPriorities { get; init; } = [];

    [InverseProperty(nameof(ReadingPriority.Tag))]
    public List<ReadingPriority> ReadingPriorities { get; init; } = [];
}

[Table(nameof(Language))]
public sealed class Language : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(LanguageSource.Language))]
    public List<LanguageSource> LanguageSources { get; init; } = [];
}
