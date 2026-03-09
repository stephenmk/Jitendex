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

using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.Import.JMdict.Models;
using Jitendex.Import.JMdict.Parsing.EntryElementReaders.ReadingElementReaders;

namespace Jitendex.Import.JMdict.Parsing.EntryElementReaders;

internal partial class ReadingReader
(
    ILogger<ReadingReader> logger,
    RestrictionReader restrictionReader,
    RInfoReader infoReader,
    RPriorityReader priorityReader
) : XmlParentElementReader<Document, ReadingElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var reading = new ReadingElement
        {
            EntryId = entry.Id,
            Order = document.Readings.NextOrder(entry.Id),
            Text = null!,
            NoKanji = false,
        };

        await ReadToEndAsync(xmlReader, document, reading, XmlTagName.Reading);

        if (reading.Text is not null)
        {
            document.Readings.Add(reading.Key(), reading);
        }
        else
        {
            LogMissingElement(reading.EntryId, reading.Order, XmlTagName.ReadingText);
        }
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, ReadingElement reading)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.ReadingText:
                await ReadReadingText(xmlReader, reading);
                break;
            case XmlTagName.ReadingPriority:
                await priorityReader.ReadAsync(xmlReader, document, reading);
                break;
            case XmlTagName.ReadingRestriction:
                await restrictionReader.ReadAsync(xmlReader, document, reading);
                break;
            case XmlTagName.ReadingInfo:
                await infoReader.ReadAsync(xmlReader, document, reading);
                break;
            case XmlTagName.ReadingNoKanji:
                reading.NoKanji = true;
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.Reading);
                break;
        }
    }

    private async Task ReadReadingText(XmlReader xmlReader, ReadingElement reading)
    {
        if (reading.Text is not null)
        {
            LogMultipleElements(reading.EntryId, reading.Order, XmlTagName.ReadingText);
        }

        reading.Text = await xmlReader.ReadElementContentAsStringAsync();

        if (string.IsNullOrWhiteSpace(reading.Text))
        {
            LogEmptyTextForm(reading.EntryId, reading.Order);
        }
    }

    [LoggerMessage(LogLevel.Error,
    "Entry `{EntryId}` reading #{Order} does not contain a <{XmlTagName}> element")]
    partial void LogMissingElement(int entryId, int order, string xmlTagName);

    [LoggerMessage(LogLevel.Warning,
    "Entry `{EntryId}` reading #{Order} contains no text")]
    partial void LogEmptyTextForm(int entryId, int order);

    [LoggerMessage(LogLevel.Warning,
    "Entry `{EntryId}` reading #{Order} contains multiple <{XmlTagName}> elements")]
    partial void LogMultipleElements(int entryId, int order, string xmlTagName);
}
