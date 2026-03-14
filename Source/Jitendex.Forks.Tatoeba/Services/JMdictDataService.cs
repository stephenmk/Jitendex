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

using Jitendex.Data.JMdict;
using Jitendex.Forks.Tatoeba.Models;
using System.Collections.Immutable;
using System.Collections.Frozen;

namespace Jitendex.Forks.Tatoeba.Services;

internal partial class JMdictDataService(JMdictForkContext jmdictContext)
{
    public JMdictData Load()
    {
        var readingToEntryIds = jmdictContext.Readings
            .GroupBy(static r => r.Text)
            .Select(static group => new
            {
                group.Key,
                Values = group
                    .Select(static r => r.EntryId)
                    .ToImmutableArray()
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Values);

        var kanjiFormToEntryIds = jmdictContext.KanjiForms
            .GroupBy(static k => k.Text)
            .Select(static group => new
            {
                group.Key,
                Values = group
                    .Select(static k => k.EntryId)
                    .ToImmutableArray()
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Values);

        var entryIdToSenseCount = jmdictContext.Entries
            .Select(static e => new
            {
                Key = e.Id,
                Value = e.Senses.Count,
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        return new JMdictData
        {
            ReadingToEntryIds = readingToEntryIds,
            KanjiFormToEntryIds = kanjiFormToEntryIds,
            EntryIdToSenseCount = entryIdToSenseCount,
        };
    }
}