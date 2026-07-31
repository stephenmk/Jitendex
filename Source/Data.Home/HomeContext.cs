// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, HomeContext.cs, is part of Jitendex.
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

using Jitendex.Data.Home.Entities.Attribution;
using Jitendex.Data.Home.Entities.Imagery;
using Jitendex.Data.Home.Entities.JMdict;
using Jitendex.Data.Home.Entities.Kanwa;
using Jitendex.Data.Home.Entities.Sound;
using Jitendex.Data.Home.Entities.Tatoeba;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.Home;

public class HomeContext() : SqliteContext(DatabaseFile.Home)
{
    #region Attribution
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<License> GraphicLicenses { get; set; } = null!;
    #endregion

    #region Imagery
    public DbSet<Graphic> Graphics { get; set; } = null!;
    #endregion

    #region JMdict
    public DbSet<Patch> JMdictPatches { get; set; } = null!;
    public DbSet<PatchGraphic> JMdictPatchGraphics { get; set; } = null!;
    public DbSet<PatchApproval> JMdictPatchApprovals { get; set; } = null!;
    public DbSet<PatchRecall> JMdictPatchRecalls { get; set; } = null!;
    public DbSet<TrademarkGloss> TrademarkGlosses { get; set; } = null!;
    #endregion

    #region Kanwa
    public DbSet<Character> Characters { get; set; } = null!;
    public DbSet<CharacterVariant> Variants { get; set; } = null!;
    public DbSet<VariantType> VariantTypes { get; set; } = null!;
    public DbSet<CharacterReading> CharacterReadings { get; set; } = null!;
    public DbSet<CharacterReadingType> CharacterReadingTypes { get; set; } = null!;
    public DbSet<Compound> Compounds { get; set; } = null!;
    public DbSet<CompoundReading> CompoundReadings { get; set; } = null!;
    public DbSet<CompoundReadingType> CompoundReadingTypes { get; set; } = null!;
    #endregion

    #region Sound
    public DbSet<KanjiAliveAudio> KanjiAliveAudios { get; set; } = null!;
    #endregion

    #region Tatoeba
    public DbSet<Example> Examples { get; set; } = null!;
    public DbSet<ExampleFurigana> ExampleFurigana { get; set; } = null!;
    #endregion
}
