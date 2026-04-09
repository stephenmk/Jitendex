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

using System.Text;
using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.Import.Kanjidic2.TableRows;

namespace Jitendex.Import.Kanjidic2.Readers.GroupReaders;

internal partial class ReadingMeaningGroupReader
(
    ILogger<ReadingMeaningGroupReader> logger,
    ReadingMeaningReader readingMeaningReader
) : XmlParentElementReader<Document, ReadingMeaningGroupElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var group = new ReadingMeaningGroupElement
        (
            EntryId: entry.Id,
            Order: document.ReadingMeaningGroups.NextOrder(entry.Id)
        );

        await ReadToEndAsync(xmlReader, document, group, XmlTagName.ReadingMeaningGroup);

        document.ReadingMeaningGroups.Add(group.Key(), group);
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, ReadingMeaningGroupElement group)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.ReadingMeaning:
                await readingMeaningReader.ReadAsync(xmlReader, document, group);
                break;
            case XmlTagName.Nanori:
                await ReadNanori(xmlReader, document, group);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.ReadingMeaningGroup);
                break;
        }
    }

    private async Task ReadNanori(XmlReader xmlReader, Document document, ReadingMeaningGroupElement group)
    {
        var nanori = new NanoriElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.Nanoris.NextOrder(group.Key()),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );
        document.Nanoris.Add(nanori.Key(), nanori);
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry for character `{Character}` has more than one <{XmlTagName}> child element.")]
    partial void LogUnexpectedGroup(Rune character, string xmlTagName);
}
