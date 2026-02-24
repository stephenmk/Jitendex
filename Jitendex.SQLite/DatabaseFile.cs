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

using static Jitendex.SQLite.DatabaseFile;

namespace Jitendex.SQLite;

public enum DatabaseFile
{
    JMdict,
    JMdictAnalysis,
    JMnedict,
    Kanjidic2,
    Tatoeba,
    KanjiVG,
    ChiseIds,
}

internal static class DatabaseFileExtensions
{
    public static string ToFilename(this DatabaseFile databaseFile)
        => databaseFile switch
        {
            JMdict => "jmdict.db",
            JMdictAnalysis => "jmdict_analysis.db",
            JMnedict => "jmnedict.db",
            Kanjidic2 => "kanjidic2.db",
            Tatoeba => "tatoeba.db",
            KanjiVG => "kanjivg.db",
            ChiseIds => "chise_ids.db",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseFile))
        };
}
