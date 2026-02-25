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

using System.Text;
using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.Import;
using Jitendex.Kanjidic2.Import.Models;

namespace Jitendex.Kanjidic2.Import.Parsing.GroupReaders;

internal partial class CodepointGroupReader(ILogger<CodepointGroupReader> logger) : BaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var group = new CodepointGroupElement
        (
            EntryId: entry.Id,
            Order: document.CodepointGroups.NextOrder(entry.Id)
        );

        document.CodepointGroups.Add(group.Key(), group);

        var exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, group);
                    break;
                case XmlNodeType.Text:
                    await LogUnexpectedTextNodeAsync(xmlReader, entry.Id, XmlTagName.CodepointGroup);
                    break;
                case XmlNodeType.EndElement:
                    exit = xmlReader.Name == XmlTagName.CodepointGroup;
                    break;
            }
        }
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document, CodepointGroupElement group)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Codepoint:
                await ReadCodepoint(xmlReader, document, group);
                break;
            default:
                LogUnexpectedChildElement(group.ToRune(), xmlReader.Name, XmlTagName.CodepointGroup);
                break;
        }
    }

    private async Task ReadCodepoint(XmlReader xmlReader, Document document, CodepointGroupElement group)
    {
        var codepoint = new CodepointElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.Codepoints.NextOrder(group.Key()),
            TypeName: GetTypeName(xmlReader, document, group),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );
        document.Codepoints.Add(codepoint.Key(), codepoint);
    }

    private string GetTypeName(XmlReader xmlReader, Document document, CodepointGroupElement group)
    {
        string typeName;
        var attribute = xmlReader.GetAttribute("cp_type");

        if (string.IsNullOrWhiteSpace(attribute))
        {
            LogMissingTypeName(group.ToRune());
            typeName = string.Empty;
        }
        else
        {
            typeName = attribute;
        }

        if (!document.CodepointTypes.ContainsKey(typeName))
        {
            var type = new CodepointTypeElement(typeName, document.Header.Date);
            document.CodepointTypes.Add(typeName, type);
        }

        return typeName;
    }

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` is missing a codepoint type attribute")]
    partial void LogMissingTypeName(Rune character);
}
