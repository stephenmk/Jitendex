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
using Jitendex.JMdict.Entities.EntryItems.ReadingItems;

namespace Jitendex.JMdict.Import.Analysis;

internal partial class RestrictionOrderAssigner(ILogger<RestrictionOrderAssigner> logger, JmdictContext context)
{
    private sealed record Update(int EntryId, int ReadingOrder, int Order, int KanjiFormOrder);

    public void AssignOrders()
    {
        var restrictions = context.Restrictions
            .Select(static r => new
            {
                r.EntryId,
                r.ReadingOrder,
                r.Order,
                r.KanjiFormText,
                KanjiForms = r.Reading.Entry.KanjiForms
                    .Select(static k => new
                    {
                        k.Order,
                        k.Text,
                        IsSearchOnly = k.Infos.Any(static i => i.TagName == "sK"),
                    })
                    .ToImmutableArray(),
            })
            .ToList();

        List<Update> updates = new(restrictions.Count);

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
                        updates.Add(new(r.EntryId, r.ReadingOrder, r.Order, kanjiForm.Order));
                    }
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                LogInvalidRestriction(r.EntryId, r.KanjiFormText);
            }
        }

        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText =
            $"""
            UPDATE "{nameof(Restriction)}"
            SET    "{nameof(Restriction.KanjiFormOrder)}" = @0
            WHERE  "{nameof(Restriction.EntryId)}"        = @1
            AND    "{nameof(Restriction.ReadingOrder)}"   = @2
            AND    "{nameof(Restriction.Order)}"          = @3;
            """;

        foreach (var update in updates)
        {
            command.Parameters.AddRange(new SqliteParameter[]
            {
                new("@0", update.KanjiFormOrder),
                new("@1", update.EntryId),
                new("@2", update.ReadingOrder),
                new("@3", update.Order),
            });
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a reading restriction to invalid kanji form `{KanjiForm}`")]
    protected partial void LogInvalidRestriction(int entryId, string kanjiForm);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a kanji form restriction to search-only `{KanjiForm}`")]
    protected partial void LogReferenceToSearchOnlyForm(int entryId, string kanjiForm);
}
