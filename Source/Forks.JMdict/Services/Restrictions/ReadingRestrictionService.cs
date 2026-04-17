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

using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.TableRows;
using Jitendex.Forks.JMdict.Tables.Restrictions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.JMdict.Services.Restrictions;

internal partial class ReadingRestrictionService
(
    ILogger<ReadingRestrictionService> logger,
    JMdictForkContext context,
    ReadingRestrictionLinkTable table
)
{
    public void Write()
    {
        var restrictions = context.ReadingRestrictions
            .AsSplitQuery()
            .Select(static r => new
            {
                r.EntryId,
                r.SenseOrder,
                r.Order,
                r.ReadingText,
                Readings = r.Sense.Entry.Readings
                    .Select(static reading => new
                    {
                        reading.Order,
                        reading.Text,
                        IsSearchOnly = reading.Infos.Any(static i => i.TagName == "sk"),
                    })
            });

        var rows = new List<ReadingRestrictionLinkRow>(5_000);

        foreach (var r in restrictions)
        {
            bool found = false;
            foreach (var reading in r.Readings)
            {
                if (string.Equals(r.ReadingText, reading.Text, StringComparison.Ordinal))
                {
                    if (reading.IsSearchOnly)
                    {
                        LogReferenceToSearchOnlyForm(r.EntryId, r.ReadingText);
                    }
                    else
                    {
                        rows.Add(new(r.EntryId, r.SenseOrder, r.Order, reading.Order));
                    }
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                LogInvalidSenseReadingRestriction(r.EntryId, r.ReadingText);
            }
        }

        table.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a sense reading restriction to invalid `{Reading}`")]
    partial void LogInvalidSenseReadingRestriction(int entryId, string reading);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a sense reading restriction to search-only `{Reading}`")]
    partial void LogReferenceToSearchOnlyForm(int entryId, string reading);
}
