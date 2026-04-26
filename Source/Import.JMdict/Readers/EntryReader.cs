// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, EntryReader.cs, is part of Jitendex.
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
using Jitendex.Import.JMdict.Readers.EntryChildReaders;
using Jitendex.Import.JMdict.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMdict.Readers;

internal partial class EntryReader
(
    ILogger<EntryReader> logger,
    KanjiFormReader kanjiFormReader,
    ReadingReader readingReader,
    SenseReader senseReader
) : XmlParentElementReader<Document, EntryRow>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document)
    {
        var entry = new EntryRow
        {
            Id = default
        };

        await ReadToEndAsync(xmlReader, document, entry, XmlTagName.Entry);

        if (entry.Id.Equals(default))
        {
            LogMissingEntryId(XmlTagName.Sequence);
        }
        else if (entry.IsJmdictEntry())
        {
            document.Entries.Add(entry.Id, entry);
        }
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, EntryRow entry)
    {
        if (entry.Id.Equals(default))
        {
            if (string.Equals(xmlReader.Name, XmlTagName.Sequence, StringComparison.Ordinal))
            {
                await ReadEntryId(xmlReader, entry);
            }
            else
            {
                LogPrematureElement(xmlReader.Name);
            }
            return;
        }
        else if (!entry.IsJmdictEntry())
        {
            await xmlReader.SkipAsync();
            return;
        }

        switch (xmlReader.Name)
        {
            case XmlTagName.Sense:
                await senseReader.ReadAsync(xmlReader, document, entry);
                break;
            case XmlTagName.Reading:
                await readingReader.ReadAsync(xmlReader, document, entry);
                break;
            case XmlTagName.KanjiForm:
                await kanjiFormReader.ReadAsync(xmlReader, document, entry);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.Entry);
                break;
        }
    }

    private async Task ReadEntryId(XmlReader xmlReader, EntryRow entry)
    {
        var idText = await xmlReader.ReadElementContentAsStringAsync();
        if (int.TryParse(idText, out int id))
        {
            entry.Id = id;
        }
        else
        {
            LogUnparsableId(idText);
        }
    }

    [LoggerMessage(LogLevel.Error,
    "Attempted to read <{XmlTagName}> child element before reading the entry primary key")]
    partial void LogPrematureElement(string xmlTagName);

    [LoggerMessage(LogLevel.Error,
    "Cannot parse entry ID from text `{Text}`")]
    partial void LogUnparsableId(string Text);

    [LoggerMessage(LogLevel.Error,
    "Entry contains no <{XmlTagName}> element; no primary key can be assigned")]
    partial void LogMissingEntryId(string xmlTagName);
}
