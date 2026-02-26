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
using Jitendex.KanjiVG.Entities;
using Jitendex.SQLite;

namespace Jitendex.KanjiVG;

public class KanjiVGContext() : SqliteContext(DatabaseFile.KanjiVG)
{
    public DbSet<Kanji> Kanjis { get; set; } = null!;

    #region Keywords
    public DbSet<VariantType> VariantTypes { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<ComponentGroupStyle> ComponentGroupStyles { get; set; } = null!;
    public DbSet<StrokeNumberGroupStyle> StrokeNumberGroupStyles { get; set; } = null!;
    public DbSet<ComponentCharacter> ComponentCharacters { get; set; } = null!;
    public DbSet<ComponentOriginal> ComponentOriginals { get; set; } = null!;
    public DbSet<ComponentPosition> ComponentPositions { get; set; } = null!;
    public DbSet<ComponentRadical> ComponentRadicals { get; set; } = null!;
    public DbSet<ComponentPhon> ComponentPhons { get; set; } = null!;
    public DbSet<StrokeType> StrokeTypes { get; set; } = null!;
    #endregion
}
