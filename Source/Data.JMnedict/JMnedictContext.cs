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

using Microsoft.EntityFrameworkCore;
using Jitendex.Data.JMnedict.Entities;
using Jitendex.Data.JMnedict.Entities.EntryChildren;
using Jitendex.Data.JMnedict.Entities.EntryChildren.KanjiFormChildren;
using Jitendex.Data.JMnedict.Entities.EntryChildren.ReadingChildren;
using Jitendex.Data.JMnedict.Entities.EntryChildren.TranslationChildren;

namespace Jitendex.Data.JMnedict;

public class JMnedictContext() : SqliteContext(DatabaseFile.JMnedict)
{
    public DbSet<FileHeader> FileHeaders { get; set; } = null!;
    public DbSet<Sequence> Sequences { get; set; } = null!;
    public DbSet<Revision> Revisions { get; set; } = null!;
    public DbSet<Entry> Entries { get; set; } = null!;

    #region Entry Items
    public DbSet<KanjiForm> KanjiForms { get; set; } = null!;
    public DbSet<Reading> Readings { get; set; } = null!;
    public DbSet<Translation> Translations { get; set; } = null!;
    #endregion

    #region Kanji Form Items
    public DbSet<KanjiFormInfo> KanjiFormInfos { get; set; } = null!;
    public DbSet<KanjiFormPriority> KanjiFormPriorities { get; set; } = null!;
    #endregion

    #region Reading Items
    public DbSet<ReadingInfo> ReadingInfos { get; set; } = null!;
    public DbSet<ReadingPriority> ReadingPriorities { get; set; } = null!;
    public DbSet<Restriction> Restrictions { get; set; } = null!;
    #endregion

    #region Translation Items
    public DbSet<CrossReference> CrossReferences { get; set; } = null!;
    public DbSet<Detail> Details { get; set; } = null!;
    public DbSet<NameType> NameTypes { get; set; } = null!;
    #endregion

    #region Keywords
    public DbSet<PriorityTag> PriorityTags { get; set; } = null!;
    public DbSet<ReadingInfoTag> ReadingInfoTags { get; set; } = null!;
    public DbSet<KanjiFormInfoTag> KanjiFormInfoTags { get; set; } = null!;
    public DbSet<NameTypeTag> NameTypeTags { get; set; } = null!;
    #endregion
}
