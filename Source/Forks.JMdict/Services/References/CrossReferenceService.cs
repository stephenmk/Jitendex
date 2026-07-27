// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, CrossReferenceService.cs, is part of Jitendex.
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

using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.TableRows;
using Jitendex.Forks.JMdict.Tables.References;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.JMdict.Services.References;

internal partial class CrossReferenceService
(
    ILogger<CrossReferenceService> logger,
    JMdictForkContext context,

    EntryReferenceTable entryReferenceTable,
    ReadingReferenceTable readingReferenceTable,
    KanjiFormReferenceTable kanjiFormReferenceTable
)
{
    public void Write()
    {
        WriteEntryReferences();
        WriteReadingReferences();
    }

    private void WriteEntryReferences()
    {
        var references = context.CrossReferences
            .Where(static x => x.Sequence != null)
            .Where(static x => x.Corpus == null)
            .Select(static x => new
            {
                x.EntryId,
                x.SenseOrder,
                x.Order,
                Sequence = x.Sequence!.Value,
                x.SenseNumber,
            });

        var rows = new List<EntryReferenceRow>();
        foreach (var x in references)
        {
            var senseOrder = x.SenseNumber.HasValue
                ? x.SenseNumber.Value - 1
                : 0;
            rows.Add(new(x.EntryId, x.SenseOrder, x.Order, x.Sequence, senseOrder));
        }
        entryReferenceTable.InsertItems(context, rows);
    }

    private void WriteReadingReferences()
    {
        var references = context.EntryReferences
            .Where(static r => r.Source.Reading != null)
            .Select(static x => new
            {
                x.EntryId,
                x.SenseOrder,
                x.CrossReferenceOrder,
                x.RefEntryId,
                Reading = x.Source.Reading!,
                Readings = x.Sense.Entry.Readings
                    .Select(static r => new { r.Order, r.Text })
            });

        var rows = new List<ReadingReferenceRow>();
        foreach (var x in references)
        {
            int? order = null;
            foreach (var r in x.Readings)
            {
                if (string.Equals(x.Reading, r.Text, StringComparison.Ordinal))
                {
                    order = r.Order;
                    break;
                }
            }
            if (!order.HasValue)
            {
                // TODO: Log
                continue;
            }
            rows.Add(new(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, order.Value));
        }
        readingReferenceTable.InsertItems(context, rows);
    }

    private void WriteKanjiFormReferences()
    {
        var references = context.EntryReferences
            .Where(static r => r.Source.KanjiForm != null)
            .Select(static x => new
            {
                x.EntryId,
                x.SenseOrder,
                x.CrossReferenceOrder,
                x.RefEntryId,
                KanjiForm = x.Source.KanjiForm!,
                KanjiForms = x.Sense.Entry.KanjiForms
                    .Select(static k => new { k.Order, k.Text })
            });

        var rows = new List<KanjiFormReferenceRow>();
        foreach (var x in references)
        {
            int? order = null;
            foreach (var k in x.KanjiForms)
            {
                if (string.Equals(x.KanjiForm, k.Text, StringComparison.Ordinal))
                {
                    order = k.Order;
                    break;
                }
            }
            if (!order.HasValue)
            {
                // TODO: Log
                continue;
            }
            rows.Add(new(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, order.Value));
        }
        kanjiFormReferenceTable.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` could refer to {Count} possible entries: {EntryIds}")]
    partial void LogAmbiguousReference(string cacheKey, int count, int[] entryIds);

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` refers to an entry that does not exist.")]
    partial void LogImpossibleReference(string cacheKey);

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` is invalid either because it points to itself or to an invalid sense number")]
    partial void LogBizarreReference(string cacheKey);

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` could not be assigned to a reading")]
    partial void LogMissingReading(string cacheKey);

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` could not be assigned to a kanji form")]
    partial void LogMissingKanjiForm(string cacheKey);

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` refers to a reading that is search-only")]
    partial void LogReferenceToSearchOnlyReading(string cacheKey);

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` refers to an invalid reading / kanji-form pair")]
    partial void LogInvalidPair(string cacheKey);
}
