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
using Jitendex.KanjiVG.Models;
using Jitendex.KanjiVG.Readers.Lookups;

namespace Jitendex.KanjiVG.Readers;

internal partial class StrokeNumberGroupReader
(
    ILogger<StrokeNumberGroupReader> logger,
    StrokeNumberReader strokeNumberReader,
    StrokeNumberGroupStyleCache groupStyleCache
)
{
    public async Task ReadAsync(XmlReader xmlReader, Entry entry)
    {
        var (id, styleText) = GetAttributes(xmlReader, entry);
        var style = groupStyleCache.Get(styleText);

        var group = new StrokeNumberGroup
        {
            UnicodeScalarValue = entry.UnicodeScalarValue,
            VariantTypeId = entry.VariantTypeId,
            StyleId = style.Id,
            Entry = entry,
            Style = style,
        };

        style.Groups.Add(group);

        if (!string.Equals(id, group.XmlIdAttribute(), StringComparison.Ordinal))
        {
            LogWrongId(entry.FileName(), id, group.XmlIdAttribute());
        }

        bool exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadElementAsync(xmlReader, group);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(entry.FileName(), text);
                    break;
                case XmlNodeType.EndElement:
                    exit = xmlReader.Name == "g";
                    break;
            }
        }

        if (entry.StrokeNumberGroup is null)
        {
            entry.StrokeNumberGroup = group;
        }
        else
        {
            LogMultipleGroups(entry.FileName());
        }
    }

    private (string, string) GetAttributes(XmlReader xmlReader, Entry entry)
    {
        string? id = null,
                style = null;
        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case nameof(id):
                    id = xmlReader.Value;
                    break;
                case nameof(style):
                    style = xmlReader.Value;
                    break;
                case "xmlns:kvg":
                    // Nothing to be done.
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, entry.FileName());
                    break;
            }
        }
        xmlReader.MoveToElement();
        if (id is null)
        {
            LogMissingAttribute(entry.FileName(), nameof(id));
            id = Guid.NewGuid().ToString();
        }
        if (style is null)
        {
            LogMissingAttribute(entry.FileName(), nameof(style));
            style = string.Empty;
        }
        return (id, style);
    }

    private async Task ReadElementAsync(XmlReader xmlReader, StrokeNumberGroup group)
    {
        switch (xmlReader.Name)
        {
            case "text":
                await strokeNumberReader.ReadAsync(xmlReader, group);
                break;
            default:
                LogUnexpectedElementName(xmlReader.Name, group.Entry.FileName(), group.XmlIdAttribute());
                break;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Unknown stroke number group attribute name `{Name}` with value `{Value}` in file `{File}`")]
    partial void LogUnknownAttributeName(string name, string value, string file);

    [LoggerMessage(LogLevel.Warning,
    "Cannot find stroke number group `{AttributeName}` attribute in file `{File}`")]
    partial void LogMissingAttribute(string file, string attributeName);

    [LoggerMessage(LogLevel.Warning,
    "{File}: Unexpected XML text node `{Text}`")]
    partial void LogUnexpectedTextNode(string file, string text);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected element name `{Name}` in file `{FileName}`, parent ID `{ParentId}`")]
    partial void LogUnexpectedElementName(string name, string fileName, string parentId);

    [LoggerMessage(LogLevel.Warning,
    "File `{FileName}` contains multiple stroke number groups")]
    partial void LogMultipleGroups(string fileName);

    [LoggerMessage(LogLevel.Warning,
    "{File}: Stroke number group ID `{Actual}` not equal to expected value `{Expected}`")]
    partial void LogWrongId(string file, string actual, string expected);
}
