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
using Jitendex.Kanjidic2.Import.Models;
using Jitendex.Kanjidic2.Import.Parsing;

namespace Jitendex.Kanjidic2.Import;

internal partial class DocumentReader
(
    ILogger<DocumentReader> logger,
    EntryReader entryReader
) : XmlParentElementReader<Document, byte>(logger),
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

        await ReadDocumentType(xmlReader);
        await ReadToEndAsync(xmlReader, document, 0, XmlTagName.Kanjidic2);

        return document;
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, byte _)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Entry:
                await entryReader.ReadAsync(xmlReader, document);
                break;
            case XmlTagName.Header:
                await xmlReader.SkipAsync();
                break;
            case XmlTagName.Kanjidic2:
                // Nothing to do
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.Kanjidic2);
                break;
        }
    }

    public async Task ReadDocumentType(XmlReader xmlReader)
    {
        var exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.DocumentType:
                    exit = true;
                    break;
            }
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
