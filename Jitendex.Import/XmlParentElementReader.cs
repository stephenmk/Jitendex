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

namespace Jitendex.Import;

public abstract partial class XmlParentElementReader<TDocument, TChild>(ILogger<XmlParentElementReader<TDocument, TChild>> logger)
    : XmlBaseReader(logger)
{
    protected async Task ReadToEndAsync(XmlReader xmlReader, TDocument document, TChild childElement, string tagName)
    {
        var exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, childElement);
                    break;
                case XmlNodeType.EndElement:
                    exit = tagName.Equals(xmlReader.Name, StringComparison.Ordinal);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(tagName, text);
                    break;
                case XmlNodeType.DocumentType:
                    LogUnexpectedDocumentType(xmlReader.Name);
                    break;
            }
        }
    }

    protected abstract Task ReadChildElementAsync(XmlReader xmlReader, TDocument document, TChild childElement);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected XML text node found in element <{TagName}>: `{Text}`")]
    partial void LogUnexpectedTextNode(string tagName, string text);

    [LoggerMessage(LogLevel.Warning, "Unexpected document type node `{Name}`")]
    partial void LogUnexpectedDocumentType(string name);
}
