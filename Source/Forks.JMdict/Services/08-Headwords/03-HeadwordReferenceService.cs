// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 03-HeadwordReferenceService.cs, is part of Jitendex.
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

using System.Collections.Frozen;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.TableRows;
using Jitendex.Forks.JMdict.Tables.Headwords;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.JMdict.Services.Headwords;

internal partial class HeadwordReferenceService
(
    ILogger<HeadwordReferenceService> logger,
    JMdictForkContext context,
    HeadwordReferenceTable table
)
{
    public void Write()
    {
        var kanjiKeyToHeadword = context.HeadwordSenses
            .Where(static h => h.Headword.ReadingOrder != null)
            .Where(static h => h.Headword.KanjiFormOrder != null)
            .Select(static h => new
            {
                h.EntryId,
                h.Headword.ReadingOrder,
                h.Headword.KanjiFormOrder,
                h.SenseOrder,
                h.HeadwordOrder,
                HeadwordSenseOrder = h.Order,
            })
            .ToFrozenDictionary
            (
                static x => (x.EntryId, x.ReadingOrder!.Value, x.KanjiFormOrder!.Value, x.SenseOrder),
                static x => (x.HeadwordOrder, x.HeadwordSenseOrder)
            );

        var readingKeyToHeadword = context.HeadwordSenses
            .Where(static h => h.Headword.ReadingOrder != null)
            .OrderBy(static h => h.EntryId)
            .ThenBy(static h => h.HeadwordOrder)
            .GroupBy(static h => new { h.EntryId, h.Headword.ReadingOrder, h.SenseOrder })
            .Select(static group => new
            {
                group.Key.EntryId,
                group.Key.ReadingOrder,
                group.Key.SenseOrder,
                Value = group
                    .Select(static s => new
                    {
                        s.HeadwordOrder,
                        HeadwordSenseOrder = s.Order,
                    })
                    .First(),
            })
            .ToFrozenDictionary
            (
                static x => (x.EntryId, x.ReadingOrder!.Value, x.SenseOrder),
                static x => (x.Value.HeadwordOrder, x.Value.HeadwordSenseOrder)
            );

        var references = context.EntryReferences
            .Select(static r => new
            {
                r.EntryId,
                r.SenseOrder,
                r.CrossReferenceOrder,
                r.RefEntryId,
                r.RefSenseOrder,
                RefReadingOrder = r.ReadingReference != null
                    ? (int?)r.ReadingReference.RefReadingOrder
                    : null,
                RefKanjiFormOrder = r.KanjiFormReference != null
                    ? (int?)r.KanjiFormReference.RefKanjiFormOrder
                    : null,
            });

        var rows = new List<HeadwordReferenceRow>();

        foreach (var reference in references)
        {
            if (reference.RefReadingOrder is null)
            {
                continue;
            }
            if (reference.RefKanjiFormOrder.HasValue)
            {
                var kanjiKey = (
                    reference.RefEntryId,
                    reference.RefReadingOrder.Value,
                    reference.RefKanjiFormOrder.Value,
                    reference.RefSenseOrder
                );
                if (kanjiKeyToHeadword.TryGetValue(kanjiKey, out var val))
                {
                    rows.Add(new(
                        reference.EntryId,
                        reference.SenseOrder,
                        reference.CrossReferenceOrder,
                        reference.RefEntryId,
                        val.HeadwordOrder,
                        val.HeadwordSenseOrder
                    ));
                }
                else
                {
                    LogMissingEntryByKanji(kanjiKey.RefEntryId, kanjiKey.Item2, kanjiKey.Item3, kanjiKey.RefSenseOrder);
                }
                continue;
            }
            var readingKey = (reference.RefEntryId, reference.RefReadingOrder.Value, reference.RefSenseOrder);
            if (readingKeyToHeadword.TryGetValue(readingKey, out var value))
            {
                rows.Add(new(
                    reference.EntryId,
                    reference.SenseOrder,
                    reference.CrossReferenceOrder,
                    reference.RefEntryId,
                    value.HeadwordOrder,
                    value.HeadwordSenseOrder
                ));
            }
            else
            {
                LogMissingEntryByReading(readingKey.RefEntryId, readingKey.Value, readingKey.RefSenseOrder);
            }
        }

        table.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning,
    "No entry found with ID {EntryId}, reading order {ReadingOrder}, kanji form order {KanjiFormOrder}, sense order {SenseOrder}")]
    partial void LogMissingEntryByKanji(int entryId, int readingOrder, int kanjiFormOrder, int senseOrder);

    [LoggerMessage(LogLevel.Warning,
    "No entry found with ID {EntryId}, reading order {ReadingOrder}, sense order {SenseOrder}")]
    partial void LogMissingEntryByReading(int entryId, int readingOrder, int senseOrder);
}
