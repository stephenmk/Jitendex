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
using Jitendex.KanjiVG.Import.Models;

namespace Jitendex.KanjiVG.Import.Readers;

internal partial class ComponentGroupReader(ILogger<ComponentGroupReader> logger, ComponentReader componentReader)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, VariantElement variant)
    {
        var (id, styleText) = GetAttributes(xmlReader, variant);

        var group = new ComponentGroupElement
        {
            UnicodeScalarValue = variant.UnicodeScalarValue,
            VariantTypeId = variant.TypeId,
            StyleId = document.ComponentGroupStyles.GetLookupId(styleText),
            IdAttribute = id,
        };

        if (!document.ComponentGroups.TryAdd(group.Key(), group))
        {
            LogMultipleGroups(variant);
        }

        bool exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, group);
                    break;
                case XmlNodeType.EndElement:
                    exit = string.Equals(xmlReader.Name, XmlTagName.Group, StringComparison.Ordinal);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(variant, text);
                    break;
            }
        }
    }

    private (string, string) GetAttributes(XmlReader xmlReader, VariantElement variant)
    {
        string? id = null;
        string? style = null;

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case XmlAttributeName.Id:
                    id = xmlReader.Value;
                    break;
                case XmlAttributeName.Style:
                    style = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgNamespace:
                    // Nothing to be done.
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, variant);
                    break;
            }
        }

        xmlReader.MoveToElement();

        if (id is null)
        {
            LogMissingAttribute(variant, XmlAttributeName.Id);
            id = Guid.NewGuid().ToString();
        }

        if (style is null)
        {
            LogMissingAttribute(variant, XmlAttributeName.Style);
            style = string.Empty;
        }

        return (id, style);
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document, ComponentGroupElement group)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Group:
                await componentReader.ReadAsync(xmlReader, document, group);
                break;
            default:
                LogUnexpectedElementName(xmlReader.Name, group.IdAttribute);
                break;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Unexpected XML text node `{Text}` found in variant `{Variant}`")]
    partial void LogUnexpectedTextNode(VariantElement variant, string text);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected element name `{Name}` under parent ID `{ParentId}`")]
    partial void LogUnexpectedElementName(string name, string parentId);

    [LoggerMessage(LogLevel.Warning,
    "Unknown component attribute name `{Name}` with value `{Value}` for variant `{Variant}`")]
    partial void LogUnknownAttributeName(string name, string value, VariantElement variant);

    [LoggerMessage(LogLevel.Warning,
    "Variant `{Variant}` contains multiple component groups")]
    partial void LogMultipleGroups(VariantElement variant);

    [LoggerMessage(LogLevel.Warning,
    "Cannot find component group `{AttributeName}` attribute for variant `{Variant}`")]
    partial void LogMissingAttribute(VariantElement variant, string attributeName);

    [LoggerMessage(LogLevel.Warning,
    "{File}: Component group ID `{Actual}` not equal to expected value `{Expected}`")]
    partial void LogWrongId(string file, string actual, string expected);
}
