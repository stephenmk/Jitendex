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
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.Models;
using Jitendex.Forks.JMdict.Tables.References;

namespace Jitendex.Forks.JMdict.Services.References;

internal partial class CrossReferenceService
(
    ILogger<CrossReferenceService> logger,
    JMdictForkContext context,
    CrossReferenceCacheService cacheService,
    CrossReferenceTextParser parser,

    AmbiguousReferenceTable ambiguousReferenceTable,
    EntryReferenceTable entryReferenceTable,
    ReadingReferenceTable readingReferenceTable,
    KanjiFormReferenceTable kanjiFormReferenceTable
)
{
    private sealed record ReferenceText
    (
        string Text1,
        string? Text2
    );

    private sealed record EntryData
    (
        int Id,
        int SenseCount,
        ImmutableArray<string> Readings,
        ImmutableArray<string> KanjiForms,
        FrozenSet<int> HiddenReadingIndices
    );

    private readonly record struct KanjiFormKey
    (
        int EntryId,
        int KanjiFormOrder
    );

    private sealed record CrossReferenceData
    (
        int EntryId,
        int SenseOrder,
        int Order,
        string Text
    )
    {
        public string CacheKey => $"{EntryId}・{SenseOrder + 1}・{Text}";
    }

    private sealed class Data
    {
        public required FrozenDictionary<string, int?> EntryIdCache { get; init; }
        public required FrozenDictionary<KanjiFormKey, ImmutableArray<int>> KanjiFormToReadings { get; init; }
        public required IReadOnlyDictionary<ReferenceText, ImmutableArray<EntryData>> ReferenceTextToEntries { get; init; }
    }

    private sealed class Rows
    {
        public List<EntryReferenceRow> EntryRefs { get; init; } = new(50_000);
        public List<ReadingReferenceRow> ReadingRefs { get; init; } = new(50_000);
        public List<KanjiFormReferenceRow> KanjiFormRefs { get; init; } = new(50_000);
        public List<AmbiguousReferenceRow> AmbiguousRefs { get; init; } = new(5_000);
    }

    public void Write()
    {
        var data = new Data
        {
            EntryIdCache = cacheService.Load(),
            ReferenceTextToEntries = GetReferenceTextToEntries(),
            KanjiFormToReadings = GetKanjiFormToReadings(),
        };

        var rows = new Rows();

        var xrefs = context.CrossReferences
            .Select(static x => new CrossReferenceData(x.EntryId, x.SenseOrder, x.Order, x.Text));

        foreach (var xref in xrefs)
        {
            Solve(data, rows, xref);
        }

        ambiguousReferenceTable.InsertItems(context, rows.AmbiguousRefs);
        entryReferenceTable.InsertItems(context, rows.EntryRefs);
        readingReferenceTable.InsertItems(context, rows.ReadingRefs);
        kanjiFormReferenceTable.InsertItems(context, rows.KanjiFormRefs);

        cacheService.Export();
    }

    private void Solve(Data data, Rows rows, CrossReferenceData xref)
    {
        var parsedRef = parser.Parse(xref.Text);

        if (parsedRef is null)
        {
            return;
        }

        var potentialEntries = GetPotentialEntries(xref, parsedRef, data);

        int? entryId = null;

        if (potentialEntries.Length == 1)
        {
            entryId = potentialEntries[0].Id;
        }
        else if (!potentialEntries.IsEmpty)
        {
            var potentialEntryIds = potentialEntries.Length < 100
                ? stackalloc int[potentialEntries.Length]
                : new int[potentialEntries.Length];

            for (int i = 0; i < potentialEntries.Length; i++)
                potentialEntryIds[i] = potentialEntries[i].Id;

            entryId = FindIdInCache(xref.CacheKey, potentialEntryIds, data);
        }

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
            : data.KanjiFormToReadings.TryGetValue(new(entry.Id, kanjiFormOrder.Value), out var readingOrders)
            ? readingOrders.First()
            : null;

        LogReferenceInconsistencies(xref, parsedRef, entry, readingOrder, kanjiFormOrder, data);

        if (potentialEntries.Length > 1)
        {
            rows.AmbiguousRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Order));
        }
        if (entryId.HasValue)
        {
            rows.EntryRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Order, entryId.Value, parsedRef.SenseNumber - 1));
            if (readingOrder.HasValue)
            {
                rows.ReadingRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Order, entryId.Value, readingOrder.Value));
            }
            if (kanjiFormOrder.HasValue)
            {
                rows.KanjiFormRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Order, entryId.Value, kanjiFormOrder.Value));
            }
        }
    }

    private FrozenDictionary<KanjiFormKey, ImmutableArray<int>> GetKanjiFormToReadings()
        => context.ReadingKanjiFormBridges
            .GroupBy(static x => new { x.EntryId, x.KanjiFormOrder })
            .Select(static group => new
            {
                Key = new KanjiFormKey(group.Key.EntryId, group.Key.KanjiFormOrder),
                Value = group
                    .OrderBy(static bridge => bridge.ReadingOrder)
                    .Select(static bridge => bridge.ReadingOrder)
                    .ToImmutableArray()
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

    private int? FindIdInCache(string key, ReadOnlySpan<int> potentialEntryIds, Data data)
    {
        int? entryId;
        if (!data.EntryIdCache.TryGetValue(key, out var cachedId))
        {
            entryId = null;
        }
        else if (cachedId is null)
        {
            entryId = null;
        }
        else if (!potentialEntryIds.Contains(cachedId.Value))
        {
            entryId = null;
        }
        else
        {
            entryId = cachedId;
        }

        if (entryId is null)
        {
            LogAmbiguousReference(key, potentialEntryIds.Length, potentialEntryIds.ToArray());
        }

        return entryId;
    }

    private ImmutableArray<EntryData> GetPotentialEntries(CrossReferenceData xref, ParsedReferenceText parsed, Data data)
    {
        var key = new ReferenceText(parsed.Text1, parsed.Text2);

        if (!data.ReferenceTextToEntries.TryGetValue(key, out var entryInfos))
        {
            LogImpossibleReference(xref.CacheKey);
            return [];
        }

        var validEntries = new EntryData[entryInfos.Length];
        int count = 0;
        foreach (var entryInfo in entryInfos)
        {
            if (entryInfo.Id != xref.EntryId && entryInfo.SenseCount >= parsed.SenseNumber)
            {
                validEntries[count++] = entryInfo;
            }
        }

        if (count == 0)
        {
            LogBizarreReference(xref.CacheKey);
        }

        return ImmutableArray.Create(validEntries.AsSpan(0, count));
    }

    private IReadOnlyDictionary<ReferenceText, ImmutableArray<EntryData>> GetReferenceTextToEntries()
    {
        var dict = new Dictionary<ReferenceText, ImmutableArray<EntryData>>(1_000_000);

        var entryQuery = context.Entries
            .AsSplitQuery()
            .Select(static e => new EntryData
            (
                e.Id,
                SenseCount: e.Senses.Count,
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
                    // Average array length will be 1.05
                    dict[referenceText] = values.Add(entry);
                }
                else
                {
                    dict.Add(referenceText, [entry]);
                }
            }
        }

        return dict;
    }

    private static IEnumerable<ReferenceText> GetReferenceTexts(
        ImmutableArray<string> readings,
        ImmutableArray<string> kanjiForms)
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

    private void LogReferenceInconsistencies(
        CrossReferenceData xref,
        ParsedReferenceText parsed,
        EntryData? entry,
        int? readingOrder,
        int? kanjiFormOrder,
        Data data)
    {
        if (entry is null)
        {
            return;
        }
        else if (readingOrder is null)
        {
            LogMissingReading(xref.CacheKey);
        }
        else if (entry.HiddenReadingIndices.Contains(readingOrder.Value))
        {
            LogReferenceToSearchOnlyReading(xref.CacheKey);
        }
        else if (kanjiFormOrder is null && parsed.Text2 is not null)
        {
            LogMissingKanjiForm(xref.CacheKey);
        }
        else if (kanjiFormOrder.HasValue &&
                data.KanjiFormToReadings.TryGetValue(new(entry.Id, kanjiFormOrder.Value), out var readings) &&
                readings.Contains(readingOrder.Value) is false)
        {
            LogInvalidPair(xref.CacheKey);
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
