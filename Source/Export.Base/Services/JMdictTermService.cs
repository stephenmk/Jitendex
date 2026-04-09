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
using Jitendex.Export.Base.Tables.TermChildren;

namespace Jitendex.Export.Base.Services;

internal sealed class JMdictTermService
(
    // ILogger<TermService> logger,
    ExportContext context,
    JMdictForkContext jmdictContext,
    TermRedirectTable redirectTable,
    TermRuleTable ruleTable
)
{
    private sealed record Mapping
    (
        FrozenDictionary<(string, string?), int> HeadwordToId,
        FrozenDictionary<int, int> EntryIdToGroupId
    );

    public void Write()
    {
        var headwordToId = context.Headwords
            .Select(static h => new { h.Id, h.Surface, h.Reading })
            .ToFrozenDictionary
            (
                static x => (x.Surface, x.Reading),
                static x => x.Id
            );

        var entryIdToGroupId = context.JMdictEntries
            .Select(static e => new { Key = e.Id, Value = e.GroupId })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        var mapping = new Mapping(headwordToId, entryIdToGroupId);

        WriteRedirects(mapping);
        WriteRules(mapping);
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

            var groupId = mapping.EntryIdToGroupId[x.EntryId];
            var redirectGroupId = mapping.EntryIdToGroupId[x.RedirectEntryId];

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
            var groupId = mapping.EntryIdToGroupId[x.EntryId];

            rows.Add(new(headwordId, groupId, x.Name));
        }

        ruleTable.InsertItems(context, rows);
    }
}
