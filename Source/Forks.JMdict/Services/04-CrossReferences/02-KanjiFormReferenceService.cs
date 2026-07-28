// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 02-KanjiFormReferenceService.cs, is part of Jitendex.
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
using Jitendex.Forks.JMdict.TableRows;
using Jitendex.Forks.JMdict.Tables.References;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.JMdict.Services.CrossReferences;

internal sealed partial class KanjiFormReferenceService
(
    ILogger<KanjiFormReferenceService> logger,
    JMdictForkContext context,
    KanjiFormReferenceTable kanjiFormReferenceTable
)
{
    public void Run()
    {
        var references = context.EntryReferences
            .Where(static r => r.Source.KanjiForm != null)
            .Select(static x => new
            {
                x.EntryId,
                x.SenseOrder,
                x.CrossReferenceOrder,
                x.RefEntryId,
                KanjiForm = x.Source.KanjiForm!,
                KanjiForms = x.Sense.Entry.KanjiForms
                    .Select(static k => new { k.Order, k.Text })
            });

        var rows = new List<KanjiFormReferenceRow>();
        foreach (var x in references)
        {
            int? order = null;
            foreach (var k in x.KanjiForms)
            {
                if (string.Equals(x.KanjiForm, k.Text, StringComparison.Ordinal))
                {
                    order = k.Order;
                    break;
                }
            }
            if (order.HasValue)
                rows.Add(new(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, order.Value));
            else
                LogMissingKanjiForm(x.EntryId, x.SenseOrder, x.CrossReferenceOrder, x.RefEntryId, x.KanjiForm);
        }
        kanjiFormReferenceTable.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning,
    "Reference {EntryId}・{KanjiFormOrder}・{Order}: could not find referenced kanji form {RefEntryId}・{RefKanjiFormText}")]
    partial void LogMissingKanjiForm(int entryId, int kanjiFormOrder, int order, int refEntryId, string refKanjiFormText);
}