// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, TermService.cs, is part of Jitendex.
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

using Jitendex.Data.Export;
using Jitendex.Data.Export.Entities.TermChildren;
using Jitendex.Data.JMdict;
using Jitendex.Export.Base.TableRows;
using Jitendex.Export.Base.Tables;
using Jitendex.Export.Base.Tables.TermChildren;

namespace Jitendex.Export.Base.Services;

internal sealed class TermService
(
    ExportContext context,
    JMdictForkContext jmdictContext,
    TermTable termTable,
    TermGroupTable groupTable,
    JMdictEntryTable jmdictEntryTable,
    TermNumberTable numberTable,
    TermTagTypeTable tagTypeTable
)
{
    public void Write()
    {
        WriteTerms();
        WriteNumbers();
        WriteTagTypes();
    }

    private void WriteTerms()
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

    private void WriteTagTypes()
    {
        var rows = Enum.GetValues<TermTagTypeId>()
            .Select(static type => new TermTagTypeRow((int)type, type.ToString()));

        tagTypeTable.InsertItems(context, rows);
    }
}
