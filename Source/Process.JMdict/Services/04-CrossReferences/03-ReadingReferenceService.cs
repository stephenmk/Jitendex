// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 03-ReadingReferenceService.cs, is part of Jitendex.
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

using Jitendex.Data.JMdict;
using Jitendex.Process.JMdict.TableRows;
using Jitendex.Process.JMdict.Tables.References;
using Microsoft.Extensions.Logging;

namespace Jitendex.Process.JMdict.Services.CrossReferences;

internal sealed partial class ReadingReferenceService
(
    ILogger<ReadingReferenceService> logger,
    JMdictForkContext context,
    ReadingReferenceTable readingReferenceTable
)
{
    public void Run()
    {
        var references = context.EntryReferences
            .Where(static r => r.Source.Reading != null)
            .Select(static x => new
            {
                x.EntryId,
                x.SenseOrder,
                x.CrossReferenceOrder,
                x.RefEntryId,
                Reading = x.Source.Reading!,
                Readings = x.Sense.Entry.Readings
                    .Select(static r => new { r.Order, r.Text })
            });

        var rows = new List<ReadingReferenceRow>();
        foreach (var x in references)
        {
            int? order = null;
            foreach (var r in x.Readings)
            {
                if (string.Equals(x.Reading, r.Text, StringComparison.Ordinal))
                {
                    order = r.Order;
                    break;
                }
            }
            if (order.HasValue)
                rows.Add(new(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, order.Value));
            else
                LogMissingReading(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, x.Reading);
        }
        readingReferenceTable.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning,
    "Reference {EntryId}・{ReadingOrder}・{Order}: could not find referenced reading {RefEntryId}・{RefReadingText}")]
    partial void LogMissingReading(int entryId, int readingOrder, int order, int refEntryId, string refReadingText);
}
