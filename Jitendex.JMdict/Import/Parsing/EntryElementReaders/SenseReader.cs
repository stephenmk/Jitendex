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
using Jitendex.Import;
using Jitendex.JMdict.Import.Models;
using Jitendex.JMdict.Import.Parsing.EntryElementReaders.SenseElementReaders;

namespace Jitendex.JMdict.Import.Parsing.EntryElementReaders;

internal partial class SenseReader
(
    ILogger<SenseReader> logger,
    KanjiFormRestrictionReader kRestrictionReader,
    ReadingRestrictionReader rRestrictionReader,
    CrossReferenceReader crossReferenceReader,
    DialectReader dialectReader,
    FieldReader fieldReader,
    GlossReader glossReader,
    LanguageSourceReader languageSourceReader,
    MiscReader miscReader,
    NoteReader noteReader,
    PartOfSpeechReader partOfSpeechReader
) : BaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var sense = new SenseElement
        {
            EntryId = entry.Id,
            Order = document.Senses.NextOrder(entry.Id),
        };

        var exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, sense);
                    break;
                case XmlNodeType.Text:
                    await LogUnexpectedTextNodeAsync(xmlReader, XmlTagName.Sense);
                    break;
                case XmlNodeType.EndElement:
                    exit = IsClosingTag(xmlReader, XmlTagName.Sense);
                    break;
            }
        }

        document.Senses.Add(sense.Key(), sense);
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document, SenseElement sense)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Gloss:
                await glossReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.PartOfSpeech:
                await partOfSpeechReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.Misc:
                await miscReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.CrossReference:
            case XmlTagName.Antonym:
                await crossReferenceReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.Example:
                await xmlReader.SkipAsync();
                break;
            case XmlTagName.Field:
                await fieldReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.LanguageSource:
                await languageSourceReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.SenseNote:
                await noteReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.SenseReadingRestriction:
                await rRestrictionReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.SenseKanjiFormRestriction:
                await kRestrictionReader.ReadAsync(xmlReader, document, sense);
                break;
            case XmlTagName.Dialect:
                await dialectReader.ReadAsync(xmlReader, document, sense);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.Sense);
                break;
        }
    }
}
