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

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Jitendex.JMdict.Entities.EntryItems.SenseItems;

namespace Jitendex.JMdict.Entities.EntryItems;

[Table(nameof(Sense))]
[PrimaryKey(nameof(EntryId), nameof(Order))]
public sealed class Sense
{
    public required int EntryId { get; init; }
    public required int Order { get; init; }
    public string? Note { get; set; }

    public List<KanjiFormRestriction> KanjiFormRestrictions { get; init; } = [];
    public List<ReadingRestriction> ReadingRestrictions { get; init; } = [];

    public List<PartOfSpeech> PartsOfSpeech { get; init; } = [];
    public List<Field> Fields { get; init; } = [];
    public List<Misc> Miscs { get; init; } = [];
    public List<Dialect> Dialects { get; init; } = [];

    public List<Gloss> Glosses { get; init; } = [];
    public List<LanguageSource> LanguageSources { get; init; } = [];

    [InverseProperty(nameof(CrossReference.Sense))]
    public List<CrossReference> CrossReferences { get; init; } = [];

    [InverseProperty(nameof(CrossReference.ReferencedSense))]
    public List<CrossReference> ReverseCrossReferences { get; init; } = [];

    [ForeignKey(nameof(EntryId))]
    public Entry Entry { get; init; } = null!;
}
