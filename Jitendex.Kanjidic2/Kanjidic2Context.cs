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

using Microsoft.EntityFrameworkCore;
using Jitendex.SQLite;
using Jitendex.Kanjidic2.Entities;
using Jitendex.Kanjidic2.Entities.SubgroupItems;

namespace Jitendex.Kanjidic2;

public class Kanjidic2Context : SqliteContext
{
    public DbSet<FileHeader> FileHeaders { get; set; } = null!;
    public DbSet<Sequence> Sequences { get; set; } = null!;
    public DbSet<Entry> Entries { get; set; } = null!;

    #region Keywords
    public DbSet<CodepointType> CodepointTypes { get; set; } = null!;
    public DbSet<DictionaryType> DictionaryTypes { get; set; } = null!;
    public DbSet<QueryCodeType> QueryCodeTypes { get; set; } = null!;
    public DbSet<MisclassificationType> MisclassificationTypes { get; set; } = null!;
    public DbSet<RadicalType> RadicalTypes { get; set; } = null!;
    public DbSet<ReadingType> ReadingType { get; set; } = null!;
    public DbSet<VariantType> VariantTypes { get; set; } = null!;
    #endregion

    #region Subgroup Items
    public DbSet<Meaning> Meanings { get; set; } = null!;
    public DbSet<Reading> Readings { get; set; } = null!;
    #endregion

    public Kanjidic2Context() : base("kanjidic2.db") { }
}
