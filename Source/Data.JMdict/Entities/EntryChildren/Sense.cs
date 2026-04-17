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
using Jitendex.Data.JMdict.Entities.EntryChildren.SenseChildren;
using Jitendex.Data.JMdict.ForkEntities.Headwords;
using Jitendex.Data.JMdict.ForkEntities.Media;
using Jitendex.Data.JMdict.ForkEntities.References;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.JMdict.Entities.EntryChildren;

[Table(nameof(Sense))]
[PrimaryKey(nameof(EntryId), nameof(Order))]
public sealed class Sense
{
    public required int EntryId { get; init; }
    public required int Order { get; init; }

    [ForeignKey(nameof(EntryId))]
    public Entry Entry { get; init; } = null!;

    public List<KanjiFormRestriction> KanjiFormRestrictions { get; init; } = [];
    public List<ReadingRestriction> ReadingRestrictions { get; init; } = [];

    public List<PartOfSpeech> PartsOfSpeech { get; init; } = [];
    public List<Field> Fields { get; init; } = [];
    public List<Misc> Miscs { get; init; } = [];
    public List<Dialect> Dialects { get; init; } = [];

    public List<Note> Notes { get; init; } = [];
    public List<Gloss> Glosses { get; init; } = [];
    public List<LanguageSource> LanguageSources { get; init; } = [];
    public List<CrossReference> CrossReferences { get; init; } = [];

    #region Fork Properties

    [InverseProperty(nameof(EntryReference.Sense))]
    public List<EntryReference> ReverseReferences { get; init; } = [];

    [InverseProperty(nameof(SenseGraphic.Sense))]
    public List<SenseGraphic> Graphics { get; init; } = [];

    [InverseProperty(nameof(HeadwordSense.Sense))]
    public List<HeadwordSense> HeadwordSenses { get; init; } = [];

    [InverseProperty(nameof(HeadwordReference.Sense))]
    public List<HeadwordReference> HeadwordReferences { get; init; } = [];

    #endregion
}
