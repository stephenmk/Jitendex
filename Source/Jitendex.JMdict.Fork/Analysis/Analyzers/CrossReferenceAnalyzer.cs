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

using System.Collections.Frozen;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Jitendex.JMdict.Fork.Analysis.Models;
using Jitendex.JMdict.Fork.Analysis.Services;
using Jitendex.JMdict.Fork.Entities.EntryItems.SenseItems;
using Jitendex.JMdict.Fork.Analysis.Tables.References;

namespace Jitendex.JMdict.Fork.Analysis.Analyzers;

internal partial class CrossReferenceAnalyzer
(
    ILogger<CrossReferenceAnalyzer> logger,
    JMdictForkContext context,
    CrossReferenceCacheService cacheService,
    CrossReferenceTextParser parser,

    AmbiguityFlagTable ambiguityFlagTable,
    EntryReferenceTable entryReferenceTable,
    SenseReferenceTable senseReferenceTable,
    ReadingReferenceTable readingReferenceTable,
    KanjiFormReferenceTable kanjiFormReferenceTable
)
{
    private sealed record ReferenceText(string Text1, string? Text2);
    private sealed record EntryData
    (
        int Id,
        int SenseCount,
        ImmutableArray<string> Readings,
        ImmutableArray<string> KanjiForms,
        FrozenSet<int> HiddenReadingIndices
    );

    public void Analyze()
    {
        var entryIdCache = cacheService.Load();
        SolveSequences(entryIdCache);
        cacheService.Export();
    }

    private void SolveSequences(FrozenDictionary<string, int?> entryIdCache)
    {
        var referenceTextToEntries = GetReferenceTextToEntries();

        var kanjiFormToReadings = context.ReadingKanjiFormBridges
            .GroupBy(static x => new { x.EntryId, x.KanjiFormOrder })
            .Select(static group => new
            {
                group.Key,
                Value = group
                    .OrderBy(static bridge => bridge.ReadingOrder)
                    .Select(static bridge => bridge.ReadingOrder)
                    .ToImmutableArray()
            })
            .AsEnumerable()
            .ToFrozenDictionary
            (
                keySelector: static g => (g.Key.EntryId, g.Key.KanjiFormOrder),
                elementSelector: static g => g.Value
            );

        var entryRefs = new List<EntryReferenceRow>(50_000);
        var senseRefs = new List<SenseReferenceRow>(50_000);
        var readingRefs = new List<ReadingReferenceRow>(50_000);
        var kanjiFormRefs = new List<KanjiFormReferenceRow>(50_000);
        var ambiguityFlags = new List<AmbiguityFlagRow>(5_000);

        foreach (var xref in context.CrossReferences.AsNoTracking())
        {
            var parsedRef = parser.Parse(xref.Text);

            if (parsedRef is null)
            {
                continue;
            }

            var potentialEntries = GetPotentialEntries(xref, parsedRef, referenceTextToEntries);
            var potentialEntryIds = potentialEntries.Select(static e => e.Id);

            var entryId = potentialEntries.Length == 0
                ? null
                : potentialEntries.Length == 1
                ? potentialEntries[0].Id
                : FindIdInCache(xref.ToExportKey(), potentialEntryIds.ToArray(), entryIdCache);

            var entry = entryId is null ? null
                : potentialEntries.First(e => e.Id == entryId);

            int? kanjiFormOrder = entry is null ? null
                : entry.KanjiForms.IndexOf(parsedRef.Text1) is int order and not -1
                ? order
                : null;

            int? readingOrder = entry is null ? null
                : entry.Readings.IndexOf(parsedRef.Text1) is int order1 and not -1
                ? order1
                : parsedRef.Text2 is not null && entry.Readings.IndexOf(parsedRef.Text2) is int order2 and not -1
                ? order2
                : kanjiFormOrder is null
                ? null
                : kanjiFormToReadings.TryGetValue((entry.Id, kanjiFormOrder.Value), out var readingOrders)
                ? readingOrders.First()
                : null;

            LogReferenceInconsistencies(xref, parsedRef, entry, readingOrder, kanjiFormOrder, kanjiFormToReadings);

            if (potentialEntries.Length > 1)
            {
                ambiguityFlags.Add(new(xref.EntryId, xref.SenseOrder, xref.Order));
            }
            if (entryId.HasValue)
            {
                entryRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Order, entryId.Value));
                senseRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Order, entryId.Value, parsedRef.SenseNumber - 1));
                if (readingOrder.HasValue)
                {
                    readingRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Order, entryId.Value, readingOrder.Value));
                }
                if (kanjiFormOrder.HasValue)
                {
                    kanjiFormRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Order, entryId.Value, kanjiFormOrder.Value));
                }
            }
        }

        ambiguityFlagTable.InsertItems(context, ambiguityFlags);
        entryReferenceTable.InsertItems(context, entryRefs);
        senseReferenceTable.InsertItems(context, senseRefs);
        readingReferenceTable.InsertItems(context, readingRefs);
        kanjiFormReferenceTable.InsertItems(context, kanjiFormRefs);
    }

    private int? FindIdInCache(string key, int[] potentialEntryIds, FrozenDictionary<string, int?> entryIdCache)
    {
        int? entryId;
        if (!entryIdCache.TryGetValue(key, out var cachedId))
        {
            entryId = null;
        }
        else if (cachedId is null)
        {
            entryId = null;
        }
        else if (!potentialEntryIds.Contains((int)cachedId))
        {
            entryId = null;
        }
        else
        {
            entryId = cachedId;
        }

        if (entryId is null)
        {
            LogAmbiguousReference(key, potentialEntryIds.Length, potentialEntryIds);
        }

        return entryId;
    }

    private EntryData[] GetPotentialEntries
    (
        CrossReference xref,
        ParsedReferenceText parsed,
        FrozenDictionary<ReferenceText, List<EntryData>> referenceTextToEntries
    )
    {
        var key = new ReferenceText(parsed.Text1, parsed.Text2);

        if (!referenceTextToEntries.TryGetValue(key, out var entryInfos))
        {
            LogImpossibleReference(xref.ToExportKey());
            return [];
        }

        var possibleTargetEntries = entryInfos
            .Where(e => e.Id != xref.EntryId && e.SenseCount >= parsed.SenseNumber)
            .ToArray();

        if (possibleTargetEntries.Length == 0)
        {
            LogBizarreReference(xref.ToExportKey());
        }

        return possibleTargetEntries;
    }

    private FrozenDictionary<ReferenceText, List<EntryData>> GetReferenceTextToEntries()
    {
        var dict = new Dictionary<ReferenceText, List<EntryData>>(1_000_000);

        var entryQuery = context.Entries
            .AsSplitQuery()
            .Select(static e => new EntryData
            (
                e.Id,
                SenseCount: e.Senses.Count(),
                Readings: e.Readings
                    .OrderBy(static r => r.Order)
                    .Select(static r => r.Text)
                    .ToImmutableArray(),
                KanjiForms: e.KanjiForms
                    .OrderBy(static k => k.Order)
                    .Select(static k => k.Text)
                    .ToImmutableArray(),
                HiddenReadingIndices: e.Readings
                    .Where(static r => r.Infos.Any(static i => i.TagName == "sk"))
                    .Select(static r => r.Order)
                    .ToFrozenSet()
            ));

        foreach (var entry in entryQuery)
        {
            foreach (var referenceText in GetReferenceTexts(entry.Readings, entry.KanjiForms))
            {
                if (dict.TryGetValue(referenceText, out var values))
                {
                    values.Add(entry);
                }
                else
                {
                    dict.Add(referenceText, [entry]);
                }
            }
        }

        return dict.ToFrozenDictionary();
    }

    private static IEnumerable<ReferenceText> GetReferenceTexts(ImmutableArray<string> readings, ImmutableArray<string> kanjiForms)
    {
        foreach (var kanjiForm in kanjiForms)
        {
            foreach (var reading in readings)
            {
                yield return new ReferenceText(kanjiForm, reading);
            }

            // References in Jmdict sometimes display only the kanji form without a reading.
            yield return new ReferenceText(kanjiForm, null);
        }

        // It is also possible for references to only show the reading,
        // even if valid kanji forms are available.
        foreach (var reading in readings)
        {
            yield return new ReferenceText(reading, null);
        }
    }

    private void LogReferenceInconsistencies
    (
        CrossReference xref,
        ParsedReferenceText parsed,
        EntryData? entry,
        int? readingOrder,
        int? kanjiFormOrder,
        FrozenDictionary<(int SequenceId, int KanjiFormOrder), ImmutableArray<int>> kanjiFormToReadings
    )
    {
        if (entry is null)
        {
            return;
        }
        else if (readingOrder is null)
        {
            LogMissingReading(xref.ToExportKey());
        }
        else if (entry.HiddenReadingIndices.Contains((int)readingOrder))
        {
            LogReferenceToSearchOnlyReading(xref.ToExportKey());
        }
        else if (kanjiFormOrder is null && parsed.Text2 is not null)
        {
            LogMissingKanjiForm(xref.ToExportKey());
        }
        else if (kanjiFormOrder is not null &&
                kanjiFormToReadings.TryGetValue((entry.Id, (int)kanjiFormOrder), out var readings) &&
                !readings.Contains((int)readingOrder))
        {
            LogInvalidPair(xref.ToExportKey());
        }
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
