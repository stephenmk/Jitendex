// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DatabaseFile.cs, is part of Jitendex.
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

using static Jitendex.Data.DatabaseFile;

namespace Jitendex.Data;

public enum DatabaseFile
{
    ChiseIds,
    Export,
    Home,
    JMdict,
    JMdictFork,
    JMnedict,
    Kanjidic2,
    KanjiVG,
    Tatoeba,
    TatoebaFork,
}

internal static class DatabaseFileExtensions
{
    #pragma warning disable format
    public static string ToFilename(this DatabaseFile databaseFile)
        => databaseFile switch
        {
            ChiseIds    => "chise_ids.db",
            Export      => "export.db",
            Home        => "home.db",
            JMdict      => "jmdict.db",
            JMdictFork  => "jmdict_fork.db",
            JMnedict    => "jmnedict.db",
            Kanjidic2   => "kanjidic2.db",
            KanjiVG     => "kanjivg.db",
            Tatoeba     => "tatoeba.db",
            TatoebaFork => "tatoeba_fork.db",
            _           => throw new ArgumentOutOfRangeException(nameof(databaseFile))
        };
    #pragma warning restore format
}
