/*
Copyright (c) 2026 Stephen Kraus
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

// using Microsoft.Extensions.Logging;
using Jitendex.Data.Export;
using Jitendex.Data.JMdict;
using Jitendex.Export.Base.TableRows;
using Jitendex.Export.Base.Tables;
using Jitendex.Export.Base.Tables.TermChildren;

namespace Jitendex.Export.Base.Services;

internal sealed class TermService
(
    // ILogger<TermService> logger,
    ExportContext context,
    JMdictForkContext jmdictContext,
    TermTable termTable,
    TermGroupTable groupTable,
    JMdictEntryTable jmdictEntryTable,
    TermRedirectTable redirectTable,
    TermRuleTable ruleTable,
    TermNumberTable numberTable
)
{
    private sealed record Mapping
    (
        FrozenDictionary<(string, string?), int> HeadwordToId,
        IReadOnlyDictionary<int, int> JmdictEntryIdToGroupId
    );

    public void Write()
    {
        var mapping = WriteTerms();
        WriteRedirects(mapping);
        WriteRules(mapping);
        WriteNumbers();
    }

    private Mapping WriteTerms()
    {
        var headwordToId = context.Headwords
            .Select(static h => new { h.Id, h.Surface, h.Reading })
            .ToFrozenDictionary
            (
                static x => (x.Surface, x.Reading),
                static x => x.Id
            );

        var jmdictTerms = jmdictContext.Headwords
            .OrderBy(static h => h.Surface)
            .ThenByDescending(static h => h.Score)
            .ThenBy(static h => h.Order)
            .Select(static h => new
            {
                h.EntryId,
                h.Surface,
                h.Reading,
                Score = (h.Score * 1000) + (100 - h.Order),
            });

        const int jmdictSize = 225_000;
        var entryIdToGroupId = new Dictionary<int, int>(jmdictSize);
        var termRows = new List<TermRow>(600_000);
        var groupRows = new List<TermGroupRow>(jmdictSize);
        var jmdictRows = new List<JMdictEntryRow>(jmdictSize);

        var nextGroupId = 1;

        foreach (var term in jmdictTerms)
        {
            if (!entryIdToGroupId.TryGetValue(term.EntryId, out var groupId))
            {
                groupId = nextGroupId++;
                entryIdToGroupId[term.EntryId] = groupId;
                groupRows.Add(new(groupId));
                jmdictRows.Add(new(term.EntryId, groupId));
            }
            var headwordId = headwordToId[(term.Surface, term.Reading)];
            termRows.Add(new(headwordId, groupId, term.Score));
        }

        groupTable.InsertItems(context, groupRows);
        termTable.InsertItems(context, termRows);
        jmdictEntryTable.InsertItems(context, jmdictRows);

        return new Mapping(headwordToId, entryIdToGroupId);
    }

    private void WriteRedirects(Mapping mapping)
    {
        var redirects = jmdictContext.HeadwordRedirects
            .Select(static h => new
            {
                h.EntryId,
                h.Headword.Surface,
                h.Headword.Reading,
                RedirectEntryId = h.RedirectHeadword.EntryId,
                RedirectSurface = h.RedirectHeadword.Surface,
                RedirectReading = h.RedirectHeadword.Reading,
            });

        var rows = new List<TermRedirectRow>(25_000);

        foreach (var x in redirects)
        {
            var headword = (x.Surface, x.Reading);
            var headwordId = mapping.HeadwordToId[headword];

            var redirectHeadword = (x.RedirectSurface, x.RedirectReading);
            var redirectHeadwordId = mapping.HeadwordToId[redirectHeadword];

            var groupId = mapping.JmdictEntryIdToGroupId[x.EntryId];
            var redirectGroupId = mapping.JmdictEntryIdToGroupId[x.RedirectEntryId];

            rows.Add(new(headwordId, groupId, redirectHeadwordId, redirectGroupId));
        }

        redirectTable.InsertItems(context, rows);
    }

    private void WriteRules(Mapping mapping)
    {
        var rules = jmdictContext.HeadwordRules
            .Select(static h => new
            {
                h.Headword.Surface,
                h.Headword.Reading,
                h.EntryId,
                h.Name,
            });

        var rows = new List<TermRuleRow>(60_000);

        foreach (var x in rules)
        {
            var headword = (x.Surface, x.Reading);
            var headwordId = mapping.HeadwordToId[headword];
            var groupId = mapping.JmdictEntryIdToGroupId[x.EntryId];

            rows.Add(new(headwordId, groupId, x.Name));
        }

        ruleTable.InsertItems(context, rows);
    }

    private void WriteNumbers()
    {
        var rows = new List<TermNumberRow>();

        var groups = context.Terms
            .GroupBy(static t => t.HeadwordId)
            .Select(static group => new
            {
                HeadwordId = group.Key,
                GroupIds = group
                    .OrderByDescending(static t => t.Score)
                    .ThenBy(static t => t.GroupId)
                    .Select(static t => t.GroupId)
            });

        foreach (var x in groups)
        {
            var ids = x.GroupIds.ToArray();
            for (int i = 0; i < ids.Length; i++)
            {
                rows.Add(new(x.HeadwordId, ids[i], i + 1, ids.Length));
            }
        }

        numberTable.InsertItems(context, rows);
    }

    // [LoggerMessage(LogLevel.Warning, "No ID found for headword {Reading}【{Surface}】")]
    // partial void LogMissingHeadwordId(string surface, string? reading);
}
