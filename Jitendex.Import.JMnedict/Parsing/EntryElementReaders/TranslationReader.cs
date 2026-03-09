/*
Copyright (c) 2026 Stephen Kraus
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
using Jitendex.Import.JMnedict.Models;
using Jitendex.Import.JMnedict.Parsing.EntryElementReaders.TranslationElementReaders;

namespace Jitendex.Import.JMnedict.Parsing.EntryElementReaders;

internal partial class TranslationReader
(
    ILogger<TranslationReader> logger,
    CrossReferenceReader crossReferenceReader,
    DetailReader detailReader,
    NameTypeReader nameTypeReader
) : XmlParentElementReader<Document, TranslationElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var translation = new TranslationElement
        {
            EntryId = entry.Id,
            Order = document.Translations.NextOrder(entry.Id),
        };

        await ReadToEndAsync(xmlReader, document, translation, XmlTagName.Translation);

        document.Translations.Add(translation.Key(), translation);
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, TranslationElement translation)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Detail:
                await detailReader.ReadAsync(xmlReader, document, translation);
                break;
            case XmlTagName.NameType:
                await nameTypeReader.ReadAsync(xmlReader, document, translation);
                break;
            case XmlTagName.CrossReference:
                await crossReferenceReader.ReadAsync(xmlReader, document, translation);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.Translation);
                break;
        }
    }
}
