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
using Jitendex.JMnedict.Import.Models;
using Jitendex.JMnedict.Import.Parsing.EntryElementReaders.TranslationElementReaders;

namespace Jitendex.JMnedict.Import.Parsing.EntryElementReaders;

internal partial class TranslationReader
(
    ILogger<TranslationReader> logger,
    CrossReferenceReader crossReferenceReader,
    DetailReader detailReader,
    NameTypeReader nameTypeReader
) : BaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var translation = new TranslationElement
        {
            EntryId = entry.Id,
            Order = document.Translations.NextOrder(entry.Id),
        };

        var exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, translation);
                    break;
                case XmlNodeType.Text:
                    await LogUnexpectedTextNodeAsync(xmlReader, XmlTagName.Translation);
                    break;
                case XmlNodeType.EndElement:
                    exit = IsClosingTag(xmlReader, XmlTagName.Translation);
                    break;
            }
        }

        document.Translations.Add(translation.Key(), translation);
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document, TranslationElement translation)
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
