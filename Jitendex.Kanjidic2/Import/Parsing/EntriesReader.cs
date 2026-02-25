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
using Jitendex.Kanjidic2.Import.Models;

namespace Jitendex.Kanjidic2.Import.Parsing;

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
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(text);
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
            case XmlTagName.Header:
                await xmlReader.SkipAsync();
                break;
            case XmlTagName.Kanjidic2:
                // Nothing to do
                break;
            default:
                LogUnexpectedElement(xmlReader.Name);
                break;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Unexpected text node found between entries: `{Text}`")]
    partial void LogUnexpectedTextNode(string text);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected element node found between entries: <{Name}>")]
    partial void LogUnexpectedElement(string name);
}
