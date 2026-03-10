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

using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Jitendex.JapaneseTextUtils;
using Jitendex.JMdict.Fork.Analysis.Tables;

namespace Jitendex.JMdict.Fork.Analysis.Analyzers;

internal partial class KanjiFormBridgeAnalyzer
(
    ILogger<KanjiFormBridgeAnalyzer> logger,
    JMdictForkContext context,
    ReadingKanjiFormBridgeTable table
)
{
    private readonly record struct ReadingData
    (
        int Order,
        string Text,
        bool NoKanji,
        bool IsHidden,
        ImmutableArray<int> RestrictionOrders
    );

    public void Analyze()
    {
        var entries = context.Entries
            .AsSplitQuery()
            .Select(static e => new
            {
                e.Id,
                Readings = e.Readings
                    .Select(static r => new ReadingData
                    (
                        r.Order,
                        r.Text,
                        r.NoKanji,
                        IsHidden: r.Infos
                            .Select(static i => i.TagName)
                            .Any(static t => t == "sk"),
                        RestrictionOrders: r.Restrictions
                            .Where(static x => x.KanjiFormOrder != null)
                            .Select(static x => (int)x.KanjiFormOrder!)
                            .ToImmutableArray()
                    )),
                KanjiFormOrders = e.KanjiForms
                    .Where(static k => k.Infos.All(static i => i.TagName != "sK"))
                    .Select(static k => k.Order)
                    .ToImmutableArray(),
            });

        var bridges = new List<KanjiFormBridgeRow>(250_000);

        foreach (var entry in entries)
        {
            var entryUsedOrders = new HashSet<int>(entry.KanjiFormOrders.Length);
            var readingToUsedOrders = new Dictionary<string, HashSet<int>>();
            foreach (var reading in entry.Readings)
            {
                if (entry.KanjiFormOrders.Length == 0 || reading.NoKanji || reading.IsHidden)
                {
                    CheckForRestrictionRedundancies(entry.Id, entry.KanjiFormOrders.Length, reading);
                    continue;
                }
                var kanjiFormOrders = reading.RestrictionOrders.Length > 0
                    ? reading.RestrictionOrders
                    : entry.KanjiFormOrders;
                var normalizedReading = reading.Text.KatakanaToHiragana();
                if (!readingToUsedOrders.TryGetValue(normalizedReading, out var readingUsedOrders))
                {
                    readingUsedOrders = [];
                    readingToUsedOrders[normalizedReading] = readingUsedOrders;
                }
                foreach (var order in kanjiFormOrders)
                {
                    entryUsedOrders.Add(order);
                    if (!readingUsedOrders.Add(order))
                    {
                        LogRedundantReadings(entry.Id, normalizedReading);
                    }
                    bridges.Add(new(entry.Id, reading.Order, order));
                }
            }
            if (entryUsedOrders.Count != entry.KanjiFormOrders.Length)
            {
                LogOrphanKanjiForms(entry.Id);
            }
        }

        table.InsertItems(context, bridges);
    }

    private void CheckForRestrictionRedundancies(int entryId, int visibleKanjiFormCount, in ReadingData reading)
    {
        // A reading shouldn't have both [NoKanji] and restrictions.
        int count0 = (reading.NoKanji ? 1 : 0) + (reading.RestrictionOrders.Length > 0 ? 1 : 0);

        // If the reading is hidden, it shouldn't have [NoKanji] or restrictions.
        int count1 = (reading.IsHidden ? 1 : 0) + count0;

        // If there are no visible kanji forms, it shouldn't have [NoKanji] or restrictions
        int count2 = (visibleKanjiFormCount == 0 ? 1 : 0) + count0;

        if (count0 > 1 || count1 > 1 || count2 > 1)
        {
            LogRedundantRestrictions(entryId, reading.Text);
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} reading `{Reading}` contains redundant restrictions")]
    partial void LogRedundantRestrictions(int entryId, string reading);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a visible kanji form without a corresponding reading")]
    partial void LogOrphanKanjiForms(int entryId);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains redundant reading {Reading}")]
    partial void LogRedundantReadings(int entryId, string reading);
}
