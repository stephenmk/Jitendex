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
using Microsoft.Extensions.Logging;
using Jitendex.JapaneseTextUtils;
using Jitendex.Data.Tatoeba;
using Jitendex.Forks.Tatoeba.Models;
using Jitendex.Forks.Tatoeba.Tables;

namespace Jitendex.Forks.Tatoeba.Services;

internal partial class EntryLinkService
(
    ILogger<EntryLinkService> logger,
    TatoebaForkContext tatoebaContext,
    JMdictDataService dataService,
    EntryLinkTable table
)
{
    private sealed record TokenData
    (
        int ExampleId,
        int SegmentationOrder,
        int Order,
        string Headword,
        string? Reading,
        int? EntryId,
        int? SenseNumber,
        bool IsPriority
    );

    public void Write()
    {
        var data = dataService.Load();

        var tokens = tatoebaContext.Tokens
            .Select(static x => new TokenData
            (
                x.ExampleId,
                x.SegmentationOrder,
                x.Order,
                x.Headword,
                x.Reading,
                x.EntryId,
                x.SenseNumber,
                x.IsPriority
            ));

        var rows = new List<EntryLinkRow>(4_000_000);

        foreach (var token in tokens)
        {
            if (token.EntryId is null)
            {
                rows.AddRange(GetImplicitLinks(token, data));
            }
            else if (GetExplicitLink(token, data) is EntryLinkRow row)
            {
                rows.Add(row);
            }
        }

        table.InsertItems(tatoebaContext, rows);
    }

    private IEnumerable<EntryLinkRow> GetImplicitLinks(TokenData token, JMdictData data)
    {
        ImmutableArray<int> entryIds;
        if (token.Reading is not null)
        {
            entryIds = data.BridgeToEntryIds.TryGetValue((token.Headword, token.Reading), out var ids)
                ? ids
                : [];
        }
        else if (token.Headword.IsAllKana())
        {
            entryIds = data.ReadingToEntryIds.TryGetValue(token.Headword, out var ids)
                ? ids
                : [];
        }
        else
        {
            entryIds = data.KanjiFormToEntryIds.TryGetValue(token.Headword, out var ids)
                ? ids
                : [];
        }

        if (token.SenseNumber is not null)
        {
            int i = 0;
            var validList = entryIds.Length < 100
                ? stackalloc int[entryIds.Length]
                : new int[entryIds.Length];
            foreach (var entryId in entryIds)
            {
                if (data.EntryIdToSenseCount.TryGetValue(entryId, out var count) && token.SenseNumber.Value <= count)
                {
                    validList[i++] = entryId;
                }
            }
            entryIds = ImmutableArray.Create(validList[..i]);
        }

        if (entryIds.IsEmpty && token.IsPriority)
        {
            LogNoMatches(token.ExampleId, token.SegmentationOrder, token.Order);
        }

        foreach (var entryId in entryIds)
        {
            int? senseOrder = token.SenseNumber.HasValue
                ? token.SenseNumber.Value - 1
                : data.EntryIdToSenseCount.TryGetValue(entryId, out int count) && count is 1
                ? 0
                : null;
            yield return new EntryLinkRow(token.ExampleId, token.SegmentationOrder, token.Order, entryId, senseOrder);
        }
    }

    private EntryLinkRow? GetExplicitLink(TokenData token, JMdictData data)
    {
        int entryId = token.EntryId!.Value;
        bool match;

        if (token.Reading is not null)
        {
            match =
                data.KanjiFormToEntryIds.TryGetValue(token.Headword, out var kIds)
                && kIds.Contains(entryId)
                && data.ReadingToEntryIds.TryGetValue(token.Reading, out var rIds)
                && rIds.Contains(entryId);
        }
        else if (token.Headword.IsAllKana())
        {
            match =
                data.ReadingToEntryIds.TryGetValue(token.Headword, out var ids)
                && ids.Contains(entryId);
        }
        else
        {
            match =
                data.KanjiFormToEntryIds.TryGetValue(token.Headword, out var ids)
                && ids.Contains(entryId);
        }

        if (!match)
        {
            LogIncorrectEntryId(token.ExampleId, token.SegmentationOrder, token.Order, entryId);
            return null;
        }

        int? senseOrder;
        if (!data.EntryIdToSenseCount.TryGetValue(entryId, out var senseCount))
        {
            LogIncorrectEntryId(token.ExampleId, token.SegmentationOrder, token.Order, entryId);
            return null;
        }
        else if (!token.SenseNumber.HasValue)
        {
            senseOrder = senseCount == 1 ? 0 : null;
        }
        else if (token.SenseNumber.Value > senseCount)
        {
            LogIncorrectSenseNumber(token.ExampleId, token.SegmentationOrder, token.Order, entryId, token.SenseNumber.Value);
            return null;
        }
        else
        {
            senseOrder = token.SenseNumber.Value - 1;
        }

        return new EntryLinkRow(token.ExampleId, token.SegmentationOrder, token.Order, entryId, senseOrder);
    }

    [LoggerMessage(LogLevel.Warning,
    "Token ID {ExampleId}-{SegmentationOrder}-{TokenOrder} could not be linked to any entries")]
    partial void LogNoMatches(int exampleId, int segmentationOrder, int tokenOrder);

    [LoggerMessage(LogLevel.Warning,
    "Token ID {ExampleId}-{SegmentationOrder}-{TokenOrder} contains an erroneous reference to entry {EntryId}")]
    partial void LogIncorrectEntryId(int exampleId, int segmentationOrder, int tokenOrder, int entryId);

    [LoggerMessage(LogLevel.Warning,
    "Token ID {ExampleId}-{SegmentationOrder}-{TokenOrder} says sense number #{Num}, but entry {EntryId} contains fewer senses")]
    partial void LogIncorrectSenseNumber(int exampleId, int segmentationOrder, int tokenOrder, int entryId, int num);
}
