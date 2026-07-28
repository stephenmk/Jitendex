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

namespace Jitendex.Forks.JMdict.Services.Links;

internal sealed partial class CrossReferenceService
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
        WriteKanjiFormReferences();
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
            if (context.Senses.Any(s => s.EntryId == x.Sequence && s.Order == senseOrder))
                rows.Add(new(x.EntryId, x.SenseOrder, x.Order, x.Sequence, senseOrder));
            else
                LogMissingSense(x.EntryId, x.SenseOrder, x.Order, x.Sequence, senseOrder);
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
            if (order.HasValue)
                rows.Add(new(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, order.Value));
            else
                LogMissingReading(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, x.Reading);
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
            if (order.HasValue)
                rows.Add(new(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, order.Value));
            else
                LogMissingKanjiForm(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, x.KanjiForm);
        }
        kanjiFormReferenceTable.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning,
    "Reference {EntryId}・{SenseOrder}・{Order}: could not find referenced sense {RefEntryId}・{RefSenseOrder}")]
    partial void LogMissingSense(int entryId, int senseOrder, int order, int refEntryId, int refSenseOrder);

    [LoggerMessage(LogLevel.Warning,
    "Reference {EntryId}・{KanjiFormOrder}・{Order}: could not find referenced kanji form {RefEntryId}・{RefKanjiFormText}")]
    partial void LogMissingKanjiForm(int entryId, int kanjiFormOrder, int order, int refEntryId, string refKanjiFormText);

    [LoggerMessage(LogLevel.Warning,
    "Reference {EntryId}・{ReadingOrder}・{Order}: could not find referenced reading {RefEntryId}・{RefReadingText}")]
    partial void LogMissingReading(int entryId, int readingOrder, int order, int refEntryId, string refReadingText);

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` refers to a reading that is search-only")]
    partial void LogReferenceToSearchOnlyReading(string cacheKey);

    [LoggerMessage(LogLevel.Warning,
    "Reference `{CacheKey}` refers to an invalid reading / kanji-form pair")]
    partial void LogInvalidPair(string cacheKey);
}
