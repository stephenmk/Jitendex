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
using Jitendex.MiscData.Entities;
using Jitendex.MiscData.Entities.Furigana;
using Jitendex.MiscData.Entities.JMdict;

namespace Jitendex.MiscData;

public class MiscDataContext() : SqliteContext(DatabaseFile.MiscData)
{
    public DbSet<User> Users { get; set; } = null!;

    #region Furigana
    public DbSet<Character> Characters { get; set; } = null!;
    public DbSet<CharacterReading> CharacterReadings { get; set; } = null!;
    public DbSet<CharacterReadingType> CharacterReadingTypes { get; set; } = null!;
    public DbSet<Compound> Compounds { get; set; } = null!;
    public DbSet<CompoundReading> CompoundReadings { get; set; } = null!;
    #endregion

    #region JMdict
    public DbSet<CrossReferenceSequence> CrossReferenceSequences { get; set; } = null!;
    public DbSet<JMdictPatch> JMdictPatches { get; set; } = null!;
    public DbSet<JMdictPatchApproval> JMdictPatchApprovals { get; set; } = null!;
    #endregion
}
