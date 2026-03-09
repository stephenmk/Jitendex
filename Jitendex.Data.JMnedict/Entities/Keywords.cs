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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Jitendex.Data.JMnedict.Entities.EntryItems.KanjiFormItems;
using Jitendex.Data.JMnedict.Entities.EntryItems.ReadingItems;
using Jitendex.Data.JMnedict.Entities.EntryItems.TranslationItems;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Jitendex.Data.JMnedict.Entities;

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

[Table(nameof(NameTypeTag))]
public sealed class NameTypeTag : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(NameType.Tag))]
    public List<NameType> NameTypes { get; init; } = [];
}

[Table(nameof(DetailLanguage))]
public sealed class DetailLanguage : IKeyword
{
    [Key]
    public required string Name { get; init; }
    public required int OriginFileId { get; init; }

    [ForeignKey(nameof(OriginFileId))]
    public FileHeader OriginFile { get; init; } = null!;

    [InverseProperty(nameof(Detail.Language))]
    public List<Detail> Details { get; init; } = [];
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
