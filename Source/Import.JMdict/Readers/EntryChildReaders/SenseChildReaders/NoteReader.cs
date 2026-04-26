// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, NoteReader.cs, is part of Jitendex.
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

using System.Xml;
using Jitendex.Import.JMdict.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMdict.Readers.EntryChildReaders.SenseChildReaders;

internal partial class NoteReader(ILogger<NoteReader> logger) : XmlBaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, SenseRow sense)
    {
        var note = new NoteRow
        (
            EntryId: sense.EntryId,
            ParentOrder: sense.Order,
            Order: document.Notes.NextOrder(sense.Key()),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );

        if (note.Order > 0)
        {
            LogTooManySenseNotes(sense.EntryId, sense.Order);
        }

        document.Notes.Add(note.Key(), note);
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{entryId}` sense #{SenseOrder} contains multiple sense notes")]
    partial void LogTooManySenseNotes(int entryId, int senseOrder);
}
