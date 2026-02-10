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

using Jitendex.JMdict.Entities.EntryItems.ReadingItems;
using Jitendex.JMdict.Entities.EntryItems.SenseItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jitendex.JMdict.Import.Analysis;

internal sealed class Analyzer(
    ILogger<Analyzer> logger,
    JmdictContext context,
    RestrictionOrderAssigner restrictionOrderAssigner,
    ReadingRestrictionOrderAssigner readingRestrictionOrderAssigner,
    KanjiFormRestrictionOrderAssigner kanjiFormRestrictionOrderAssigner,
    ReadingBridger readingBridger,
    ReferenceSequencer referenceSequencer)
{
    public void Clean()
    {
        logger.LogInformation("Deleting previous analysis data from database");
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText =
            $"""
            UPDATE "{nameof(Restriction)}"
            SET    "{nameof(Restriction.KanjiFormOrder)}" = NULL;

            UPDATE "{nameof(ReadingRestriction)}"
            SET    "{nameof(ReadingRestriction.ReadingOrder)}" = NULL;

            UPDATE "{nameof(KanjiFormRestriction)}"
            SET    "{nameof(KanjiFormRestriction.KanjiFormOrder)}" = NULL;

            DELETE FROM "{nameof(KanjiFormBridge)}";

            UPDATE "{nameof(CrossReference)}"
            SET    "{nameof(CrossReference.RefReadingOrder)}"   = NULL
            ,      "{nameof(CrossReference.RefKanjiFormOrder)}" = NULL
            ,      "{nameof(CrossReference.RefSenseOrder)}"     = NULL;
            """;
        command.ExecuteNonQuery();
    }

    public void Analyze()
    {
        logger.LogInformation("Starting data analysis");

        restrictionOrderAssigner.AssignOrders();
        readingRestrictionOrderAssigner.AssignOrders();
        kanjiFormRestrictionOrderAssigner.AssignOrders();

        readingBridger.BridgeReadingsToKanjiForms();
        referenceSequencer.FindCrossReferenceSequenceIds();
    }
}
