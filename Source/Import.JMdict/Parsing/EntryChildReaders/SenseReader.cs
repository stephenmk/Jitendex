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
using Jitendex.Import.JMdict.RowModels;
using Jitendex.Import.JMdict.Parsing.EntryChildReaders.SenseChildReaders;

namespace Jitendex.Import.JMdict.Parsing.EntryChildReaders;

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
) : XmlParentElementReader<Document, SenseRow>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryRow entry)
    {
        var sense = new SenseRow
        {
            EntryId = entry.Id,
            Order = document.Senses.NextOrder(entry.Id),
        };

        await ReadToEndAsync(xmlReader, document, sense, XmlTagName.Sense);

        document.Senses.Add(sense.Key(), sense);
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, SenseRow sense)
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
