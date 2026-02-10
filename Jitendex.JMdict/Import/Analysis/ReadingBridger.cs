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

namespace Jitendex.JMdict.Import.Analysis;

internal partial class ReadingBridger
{
    private readonly ILogger<ReadingBridger> _logger;
    private readonly JmdictContext _context;

    public ReadingBridger(ILogger<ReadingBridger> logger, JmdictContext context) =>
        (_logger, _context) =
        (@logger, @context);

    private static readonly KanjiFormBridgeTable KanjiFormBridgeTable = new();
    private readonly record struct ReadingData(int Order, string Text, bool NoKanji, bool IsHidden, ImmutableArray<int> Restrictions);
    private readonly record struct KanjiFormData(int Order, string Text);

    public void BridgeReadingsToKanjiForms()
    {
        var bridges = GetBridges();
        // TODO: check for excessive pairings, e.g. キモ可愛；きも可愛【キモかわ；きもかわ】
        // Need to include method for normalizing katakana to hiragana.
        KanjiFormBridgeTable.InsertItems(_context, bridges);
    }

    private List<KanjiFormBridgeElement> GetBridges()
    {
        var entries = _context.Entries
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
                        Restrictions: r.Restrictions
                            .Where(static x => x.KanjiFormOrder != null)
                            .Select(static x => (int)x.KanjiFormOrder!)
                            .ToImmutableArray()
                    )),
                KanjiForms = e.KanjiForms
                    .Where(static k => k.Infos.All(static i => i.TagName != "sK"))
                    .Select(static k => new KanjiFormData(k.Order, k.Text))
                    .ToImmutableArray(),
            });

        var bridges = new List<KanjiFormBridgeElement>(250_000);

        foreach (var entry in entries)
        {
            var usedKanjiFormOrders = new HashSet<int>(entry.KanjiForms.Length);
            foreach (var reading in entry.Readings)
            {
                CheckForRedundancies(entry.Id, entry.KanjiForms.Length, reading);
                if (entry.KanjiForms.Length == 0 || reading.NoKanji || reading.IsHidden)
                {
                    continue;
                }
                var kanjiFormOrders = reading.Restrictions.Length > 0
                    ? reading.Restrictions
                    : entry.KanjiForms.Select(static k => k.Order).ToImmutableArray();
                foreach (var order in kanjiFormOrders)
                {
                    usedKanjiFormOrders.Add(order);
                    bridges.Add(new(entry.Id, reading.Order, order));
                }
            }
            if (usedKanjiFormOrders.Count != entry.KanjiForms.Length)
            {
                LogOrphanKanjiForms(entry.Id);
            }
        }

        return bridges;
    }

    private void CheckForRedundancies(int entryId, int visibleKanjiFormCount, in ReadingData reading)
    {
        // A reading shouldn't have both [NoKanji] and restrictions.
        int count0 = (reading.NoKanji ? 1 : 0) + (reading.Restrictions.Length > 0 ? 1 : 0);

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
    protected partial void LogRedundantRestrictions(int entryId, string reading);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} reading `{Reading}` contains a restriction to an invalid kanji form")]
    protected partial void LogInvalidRestriction(int entryId, string reading);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a visible kanji form without a corresponding reading")]
    protected partial void LogOrphanKanjiForms(int entryId);
}
