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

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.Models;
using Jitendex.Forks.JMdict.Tables.Restrictions;

namespace Jitendex.Forks.JMdict.Services.Restrictions;

internal partial class KanjiFormRestrictionService
(
    ILogger<KanjiFormRestrictionService> logger,
    JMdictForkContext context,
    KanjiFormRestrictionLinkTable table
)
{
    public void Write()
    {
        var restrictions = context.KanjiFormRestrictions
            .AsSplitQuery()
            .Select(static r => new
            {
                r.EntryId,
                r.SenseOrder,
                r.Order,
                r.KanjiFormText,
                KanjiForms = r.Sense.Entry.KanjiForms
                    .Select(static k => new
                    {
                        k.Order,
                        k.Text,
                        IsSearchOnly = k.Infos.Any(static i => i.TagName == "sK"),
                    })
            });

        var rows = new List<KanjiFormRestrictionLinkRow>(1_500);

        foreach (var r in restrictions)
        {
            bool found = false;
            foreach (var kanjiForm in r.KanjiForms)
            {
                if (string.Equals(r.KanjiFormText, kanjiForm.Text, StringComparison.Ordinal))
                {
                    if (kanjiForm.IsSearchOnly)
                    {
                        LogReferenceToSearchOnlyForm(r.EntryId, r.KanjiFormText);
                    }
                    else
                    {
                        rows.Add(new(r.EntryId, r.SenseOrder, r.Order, kanjiForm.Order));
                    }
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                LogInvalidSenseKanjiFormRestriction(r.EntryId, r.KanjiFormText);
            }
        }

        table.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a sense kanji form restriction to invalid form `{KanjiForm}`")]
    partial void LogInvalidSenseKanjiFormRestriction(int entryId, string kanjiForm);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a sense kanji form restriction to search-only `{KanjiForm}`")]
    partial void LogReferenceToSearchOnlyForm(int entryId, string kanjiForm);
}
