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

using Jitendex.Data.Export.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.Export;

public sealed class ExportContext() : SqliteContext(DatabaseFile.Export)
{
    public DbSet<Headword> Headwords { get; set; } = null!;
    public DbSet<Term> Terms { get; set; } = null!;
    public DbSet<JMdictEntry> JMdictEntries { get; set; } = null!;
}
