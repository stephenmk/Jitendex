// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 01-EntryReferenceService.cs, is part of Jitendex.
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

internal sealed partial class EntryReferenceService
(
    ILogger<EntryReferenceService> logger,
    JMdictForkContext context,
    EntryReferenceTable entryReferenceTable
)
{
    public void Run()
    {
        var references = context.CrossReferences
            .Where(static x => x.Sequence != null)
            .Where(static x => x.Corpus == null)
            .Select(static x => new
            {
                x.EntryId,
                x.SenseOrder,
                x.Order,
                Sequence = x.Sequence!.Value,
                x.SenseNumber,
            });

        var rows = new List<EntryReferenceRow>();
        foreach (var x in references)
        {
            var senseOrder = x.SenseNumber.HasValue
                ? x.SenseNumber.Value - 1
                : 0;
            if (context.Senses.Any(s => s.EntryId == x.Sequence && s.Order == senseOrder))
                rows.Add(new(x.EntryId, x.SenseOrder, x.Order, x.Sequence, senseOrder));
            else
                LogMissingSense(x.EntryId, x.SenseOrder, x.Order, x.Sequence, senseOrder);
        }
        entryReferenceTable.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning,
    "Reference {EntryId}・{SenseOrder}・{Order}: could not find referenced sense {RefEntryId}・{RefSenseOrder}")]
    partial void LogMissingSense(int entryId, int senseOrder, int order, int refEntryId, int refSenseOrder);
}
