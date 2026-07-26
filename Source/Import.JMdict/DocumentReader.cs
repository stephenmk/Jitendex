// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DocumentReader.cs, is part of Jitendex.
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

using System.IO.Compression;
using System.Xml;
using Jitendex.Import.JMdict.NGReaders;
using Jitendex.Import.JMdict.Readers;

namespace Jitendex.Import.JMdict;

internal class DocumentReader
(
    DocumentTypeReader docTypeReader,
    JMdictReader jmdictReader,
    JMdictNGReader jmdictNGReader
) :
    IDocumentReader<DateOnly, Document>
{
    public async Task<Document> ReadAsync(FileInfo file, DateOnly fileDate)
    {
        await using var fileStream = file.OpenRead();
        await using var brotliStream = new BrotliStream(fileStream, CompressionMode.Decompress);
        using var xmlReader = XmlReader.Create(brotliStream, XmlReaderSettings);

        var document = new Document
        {
            ArchiveKey = fileDate,
            Version = null!,
        };

        await docTypeReader.ReadAsync(xmlReader, document);
        document.Version = await GetJMdictVersionAsync(xmlReader);

        switch (document.Version)
        {
            case JMdictVersion.OG:
                await jmdictReader.ReadAsync(xmlReader, document);
                break;
            case JMdictVersion.NG:
                await jmdictNGReader.ReadAsync(xmlReader, document);
                break;
            default:
                throw new NotSupportedException($"Cannot read entries for JMdict Version {document.Version}");
        }

        return document;
    }

    private async Task<string> GetJMdictVersionAsync(XmlReader xmlReader)
    {
        do
        {
            await xmlReader.ReadAsync();
        }
        while (xmlReader.NodeType is XmlNodeType.Comment or XmlNodeType.Whitespace);

        if (xmlReader.Name != XmlTagName.Jmdict)
            throw new InvalidDataException($"Expected node `{XmlTagName.Jmdict}`, but found element named `{xmlReader.Name}`");

        if (xmlReader.GetAttribute("version") is string version)
            return version;
        else
            return JMdictVersion.OG;
    }

    private static readonly XmlReaderSettings XmlReaderSettings = new()
    {
        Async = true,
        DtdProcessing = DtdProcessing.Parse,
        MaxCharactersFromEntities = long.MaxValue,
        MaxCharactersInDocument = long.MaxValue,
    };

    private static class JMdictVersion
    {
        public const string OG = "1.09";
        public const string NG = "1.10";
    }
}
