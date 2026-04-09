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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Jitendex.Data.Export;
using Jitendex.Data.JMdict;
using Jitendex.Export.Base.TableRows;
using Jitendex.Export.Base.Tables;

namespace Jitendex.Export.Base.Services;

internal partial class TermService
(
    ILogger<TermService> logger,
    ExportContext context,
    JMdictForkContext jmdictContext,
    TermTable termTable,
    TermGroupTable groupTable,
    JMdictEntryTable jmdictEntryTable
)
{
    public void Write()
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

        var nextGroupId = 1;
        var entryIdToGroupId = new Dictionary<int, int>();
        var termRows = new List<TermRow>();
        var groupRows = new List<TermGroupRow>();
        var jmdictRows = new List<JMdictEntryRow>();

        foreach (var term in jmdictTerms)
        {
            if (!headwordToId.TryGetValue((term.Surface, term.Reading), out var headwordId))
            {
                LogMissingHeadwordId(term.Surface, term.Reading);
                continue;
            }
            if (!entryIdToGroupId.TryGetValue(term.EntryId, out var groupId))
            {
                groupId = nextGroupId++;
                entryIdToGroupId[term.EntryId] = groupId;
                groupRows.Add(new(groupId));
                jmdictRows.Add(new(term.EntryId, groupId));
            }
            termRows.Add(new(headwordId, groupId, term.Score));
        }

        groupTable.InsertItems(context, groupRows);
        termTable.InsertItems(context, termRows);
        jmdictEntryTable.InsertItems(context, jmdictRows);
    }

    [LoggerMessage(LogLevel.Warning, "No ID found for headword {Reading}【{Surface}】")]
    partial void LogMissingHeadwordId(string surface, string? reading);
}
