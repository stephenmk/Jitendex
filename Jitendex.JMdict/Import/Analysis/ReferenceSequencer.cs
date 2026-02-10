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
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Jitendex.SupplementalData;
using Jitendex.SupplementalData.Entities.JMdict;
using Jitendex.SQLite;
using Jitendex.JMdict.Entities.EntryItems.SenseItems;

namespace Jitendex.JMdict.Import.Analysis;

internal partial class ReferenceSequencer
{
    private readonly ILogger<ReferenceSequencer> _logger;
    private readonly JmdictContext _jmdictContext;
    private readonly SupplementContext _supplementContext;
    private readonly CrossReferenceTextParser _parser;

    public ReferenceSequencer(ILogger<ReferenceSequencer> logger, JmdictContext jmdictContext, SupplementContext supplementContext, CrossReferenceTextParser parser) =>
        (_logger, _jmdictContext, _supplementContext, _parser) =
        (@logger, @jmdictContext, @supplementContext, @parser);

    private sealed record ReferenceText(string Text1, string? Text2);

    private sealed record EntryData(
        int Id,
        int SenseCount,
        ImmutableArray<string> Readings,
        ImmutableArray<string> KanjiForms,
        FrozenSet<int> HiddenReadingIndices);

    private sealed record SequencedRef(
        int SequenceId,
        int SenseOrder,
        string Text,
        int? RefSequenceId,
        int? RefReadingOrder,
        int? RefKanjiFormOrder,
        int? RefSenseOrder);

    public void FindCrossReferenceSequenceIds()
    {
        var sequencedRefs = GetSequencedRefs();
        WriteRefsToDatabase(sequencedRefs);
    }

    private List<SequencedRef> GetSequencedRefs()
    {
        var referenceTextToEntries = GetReferenceTextToEntries();

        var rawCrossReferences = _jmdictContext.CrossReferences
            .AsNoTracking()
            .ToList();

        var kanjiFormToReadings = _jmdictContext.KanjiFormBridges
            .GroupBy(static x => new { x.EntryId, x.KanjiFormOrder })
            .ToFrozenDictionary(
                static g => (g.Key.EntryId, g.Key.KanjiFormOrder),
                static g => g
                    .OrderBy(static bridge => bridge.ReadingOrder)
                    .Select(static bridge => bridge.ReadingOrder)
                    .ToImmutableArray());

        var sequenceIdCache = _supplementContext.CrossReferenceSequences
            .ToFrozenDictionary(
                static x => x.ToExportKey(),
                static x => x.RefSequenceId);

        var sequencedRefs = new List<SequencedRef>(rawCrossReferences.Count);

        foreach (var xref in rawCrossReferences)
        {
            var parsedRef = _parser.Parse(xref.Text);

            if (parsedRef is null)
            {
                sequencedRefs.Add(new(xref.EntryId, xref.SenseOrder, xref.Text, null, null, null, null));
                continue;
            }

            var potentialEntries = GetPotentialEntries(xref, parsedRef, referenceTextToEntries);
            var potentialEntryIds = potentialEntries.Select(static e => e.Id);

            var entryId = potentialEntries.Length == 0
                ? null
                : potentialEntries.Length == 1
                ? potentialEntries[0].Id
                : FindIdInCache(xref.ToExportKey(), potentialEntryIds.ToArray(), sequenceIdCache);

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
                : kanjiFormToReadings.TryGetValue((entry.Id, (int)kanjiFormOrder), out var readingOrders)
                ? readingOrders.First()
                : null;

            LogReferenceInconsistencies(xref, parsedRef, entry, readingOrder, kanjiFormOrder, kanjiFormToReadings);

            sequencedRefs.Add(new
            (
                SequenceId: xref.EntryId,
                SenseOrder: xref.SenseOrder,
                Text: xref.Text,
                RefSequenceId: entryId,
                RefReadingOrder: readingOrder,
                RefKanjiFormOrder: kanjiFormOrder,
                RefSenseOrder: parsedRef.SenseNumber - 1
            ));
        }

        return sequencedRefs;
    }

    private int? FindIdInCache(string key, int[] potentialEntryIds, FrozenDictionary<string, int?> xrefCache)
    {
        int? entryId;
        if (!xrefCache.TryGetValue(key, out var cachedId))
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

    private EntryData[] GetPotentialEntries(
        CrossReference xref,
        ParsedReferenceText parsed,
        IReadOnlyDictionary<ReferenceText, List<EntryData>> referenceTextToEntries)
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
        var entries = LoadEntryData();
        var dict = new Dictionary<ReferenceText, List<EntryData>>(entries.Count * 4);
        foreach (var entry in entries)
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

    private ImmutableList<EntryData> LoadEntryData() => _jmdictContext.Entries
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
        ))
        .ToImmutableList();

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

    private void LogReferenceInconsistencies(
        CrossReference xref,
        ParsedReferenceText parsed,
        EntryData? entry,
        int? readingOrder,
        int? kanjiFormOrder,
        FrozenDictionary<(int SequenceId, int KanjiFormOrder), ImmutableArray<int>> kanjiFormToReadings)
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

    private void WriteRefsToDatabase(List<SequencedRef> refs)
    {
        using var transaction = _supplementContext.Database.BeginTransaction();
        _supplementContext.CrossReferenceSequences.ExecuteDelete();

        using var command = _supplementContext.Database.GetDbConnection().CreateCommand();
        command.CommandText =
            $"""
            INSERT INTO "{nameof(CrossReferenceSequence)}"
            ( "{nameof(CrossReferenceSequence.SequenceId)}"
            , "{nameof(CrossReferenceSequence.SenseOrder)}"
            , "{nameof(CrossReferenceSequence.Text)}"
            , "{nameof(CrossReferenceSequence.RefSequenceId)}"
            , "{nameof(CrossReferenceSequence.RefReadingOrder)}"
            , "{nameof(CrossReferenceSequence.RefKanjiFormOrder)}"
            , "{nameof(CrossReferenceSequence.RefSenseOrder)}"
            ) VALUES (@0, @1, @2, @3, @4, @5, @6);
            """;

        foreach (var xref in refs)
        {
            command.Parameters.AddRange(new SqliteParameter[]
            {
                new("@0", xref.SequenceId),
                new("@1", xref.SenseOrder),
                new("@2", xref.Text),
                new("@3", xref.RefSequenceId.Nullable()),
                new("@4", xref.RefReadingOrder.Nullable()),
                new("@5", xref.RefKanjiFormOrder.Nullable()),
                new("@6", xref.RefSenseOrder.Nullable()),
            });
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }

        _supplementContext.SaveChanges();
        transaction.Commit();
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
