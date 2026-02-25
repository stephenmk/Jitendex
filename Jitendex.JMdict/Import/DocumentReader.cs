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

using System.IO.Compression;
using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.Import;
using Jitendex.JMdict.Import.Models;
using Jitendex.JMdict.Import.Parsing;

namespace Jitendex.JMdict.Import;

internal partial class DocumentReader
(
    ILogger<DocumentReader> logger,
    DocumentTypeReader docTypeReader,
    EntriesReader entriesReader
) :
    BaseReader(logger),
    IDocumentReader<DateOnly, Document>
{
    public async Task<Document> ReadAsync(FileInfo file, DateOnly fileDate)
    {
        await using FileStream f = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using BrotliStream b = new(f, CompressionMode.Decompress);
        using var xmlReader = XmlReader.Create(b, XmlReaderSettings);

        var document = new Document
        {
            ArchiveKey = fileDate
        };

        await docTypeReader.ReadAsync(xmlReader, document);

        var exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document);
                    break;
                case XmlNodeType.Text:
                    await LogUnexpectedTextNodeAsync(xmlReader, XmlTagName.Root);
                    break;
            }
        }

        return document;
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Jmdict:
                await entriesReader.ReadAsync(xmlReader, document);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.Root);
                break;
        }
    }

    private static readonly XmlReaderSettings XmlReaderSettings = new()
    {
        Async = true,
        DtdProcessing = DtdProcessing.Parse,
        MaxCharactersFromEntities = long.MaxValue,
        MaxCharactersInDocument = long.MaxValue,
    };
}
