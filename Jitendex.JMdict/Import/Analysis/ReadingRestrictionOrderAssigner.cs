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
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Jitendex.JMdict.Entities.EntryItems.SenseItems;

namespace Jitendex.JMdict.Import.Analysis;

internal partial class ReadingRestrictionOrderAssigner(ILogger<ReadingRestrictionOrderAssigner> logger, JmdictContext context)
{
    private sealed record Update(int EntryId, int SenseOrder, int Order, int ReadingOrder);

    public void AssignOrders()
    {
        var restrictions = context.ReadingRestrictions
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
                    .ToImmutableArray(),
            })
            .ToList();

        List<Update> updates = new(restrictions.Count);

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
                        updates.Add(new(r.EntryId, r.SenseOrder, r.Order, reading.Order));
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

        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText =
            $"""
            UPDATE "{nameof(ReadingRestriction)}"
            SET    "{nameof(ReadingRestriction.ReadingOrder)}" = @0
            WHERE  "{nameof(ReadingRestriction.EntryId)}"      = @1
            AND    "{nameof(ReadingRestriction.SenseOrder)}"   = @2
            AND    "{nameof(ReadingRestriction.Order)}"        = @3;
            """;

        foreach (var update in updates)
        {
            command.Parameters.AddRange(new SqliteParameter[]
            {
                new("@0", update.ReadingOrder),
                new("@1", update.EntryId),
                new("@2", update.SenseOrder),
                new("@3", update.Order),
            });
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a sense reading restriction to invalid `{Reading}`")]
    protected partial void LogInvalidSenseReadingRestriction(int entryId, string reading);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a sense reading restriction to search-only `{Reading}`")]
    protected partial void LogReferenceToSearchOnlyForm(int entryId, string reading);
}
