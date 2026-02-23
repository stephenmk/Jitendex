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

namespace Jitendex.JMnedict.Import.Parsing;

internal partial class EntriesReader(ILogger<EntriesReader> logger, EntryReader entryReader) : BaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document)
    {
        while (await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document);
                    break;
                case XmlNodeType.Text:
                    await LogUnexpectedTextNodeAsync(xmlReader, XmlTagName.JMnedict);
                    break;
                case XmlNodeType.DocumentType:
                    LogUnexpectedDocumentType(xmlReader.Name);
                    break;
            }
        }
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Entry:
                await entryReader.ReadAsync(xmlReader, document);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.JMnedict);
                break;
        }
    }

    [LoggerMessage(LogLevel.Warning, "Unexpected document type node `{Name}`")]
    partial void LogUnexpectedDocumentType(string name);
}
