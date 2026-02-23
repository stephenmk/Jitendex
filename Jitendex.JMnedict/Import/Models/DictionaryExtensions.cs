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

namespace Jitendex.JMnedict.Import.Models;

internal static class DictionaryExtentions
{
    public static int NextOrder<T>(this Dictionary<(int, int), T> dictionary, int parentKey)
    {
        int i = 0;
        while (dictionary.ContainsKey((parentKey, i)))
        {
            i++;
        }
        return i;
    }

    public static int NextOrder<T>(this Dictionary<(int, int, int), T> dictionary, (int, int) parentKey)
    {
        int i = 0;
        while (dictionary.ContainsKey((parentKey.Item1, parentKey.Item2, i)))
        {
            i++;
        }
        return i;
    }

    public static IEnumerable<int> EntryIds<T>(this Dictionary<(int, int), T> dictionary)
        => dictionary.Keys.Select(static k => k.Item1);

    public static IEnumerable<int> EntryIds<T>(this Dictionary<(int, int, int), T> dictionary)
        => dictionary.Keys.Select(static k => k.Item1);
}
