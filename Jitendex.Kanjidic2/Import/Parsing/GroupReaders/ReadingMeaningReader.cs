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
using Jitendex.Import;
using Jitendex.Kanjidic2.Import.Models;

namespace Jitendex.Kanjidic2.Import.Parsing.GroupReaders;

internal partial class ReadingMeaningReader(ILogger<ReadingMeaningReader> logger)
    : XmlParentElementReader<Document, ReadingMeaningElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, ReadingMeaningGroupElement group)
    {
        var readingMeaning = new ReadingMeaningElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.ReadingMeanings.NextOrder(group.Key())
        );

        await ReadToEndAsync(xmlReader, document, readingMeaning, XmlTagName.ReadingMeaning);

        document.ReadingMeanings.Add(readingMeaning.Key(), readingMeaning);
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, ReadingMeaningElement readingMeaning)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Reading:
                await ReadReading(xmlReader, document, readingMeaning);
                break;
            case XmlTagName.Meaning:
                await ReadMeaning(xmlReader, document, readingMeaning);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.ReadingMeaning);
                break;
        }
    }

    private async Task ReadReading(XmlReader xmlReader, Document document, ReadingMeaningElement readingMeaning)
    {
        var reading = new ReadingElement
        (
            EntryId: readingMeaning.EntryId,
            GroupOrder: readingMeaning.GroupOrder,
            ReadingMeaningOrder: readingMeaning.Order,
            Order: document.Readings.NextOrder(readingMeaning.Key()),
            TypeName: GetReadingTypeName(xmlReader, document, readingMeaning),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );
        document.Readings.Add(reading.Key(), reading);
    }

    private string GetReadingTypeName(XmlReader xmlReader, Document document, ReadingMeaningElement readingMeaning)
    {
        var typeName = xmlReader.GetAttribute(XmlAttributeName.ReadingType) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(typeName))
        {
            LogMissingTypeName(readingMeaning.ToRune());
        }

        document.ReadingTypes.Add(typeName);

        return typeName;
    }

    private async Task ReadMeaning(XmlReader xmlReader, Document document, ReadingMeaningElement readingMeaning)
    {
        if (xmlReader.GetAttribute(XmlAttributeName.MeaningType) is not null)
        {
            // This is not an English-language meaning; skip.
            await xmlReader.SkipAsync();
            return;
        }

        var meaning = new MeaningElement
        (
            EntryId: readingMeaning.EntryId,
            GroupOrder: readingMeaning.GroupOrder,
            ReadingMeaningOrder: readingMeaning.Order,
            Order: document.Meanings.NextOrder(readingMeaning.Key()),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );

        if (string.Equals(meaning.Text, "(kokuji)", StringComparison.Ordinal))
        {
            readingMeaning.IsKokuji = true;
            return;
        }

        if (string.Equals(meaning.Text, "(ghost kanji)", StringComparison.Ordinal))
        {
            readingMeaning.IsGhost = true;
            return;
        }

        document.Meanings.Add(meaning.Key(), meaning);
    }

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` is missing a reading type attribute")]
    partial void LogMissingTypeName(Rune character);
}
