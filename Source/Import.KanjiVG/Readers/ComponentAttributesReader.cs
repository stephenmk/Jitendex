// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, ComponentAttributesReader.cs, is part of Jitendex.
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

using System.Xml;
using Jitendex.Import.KanjiVG.Models;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.KanjiVG.Readers;

internal partial class ComponentAttributesReader(ILogger<ComponentAttributesReader> logger)
{
    public ComponentAttributes Read(XmlReader xmlReader, ComponentGroupElement group)
    {
        var attributes = new ComponentAttributes
        {
            Id = null!
        };

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case XmlAttributeName.Id:
                    attributes.Id = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgElement:
                    attributes.Text = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgVariant:
                    attributes.IsVariant = GetBoolean(xmlReader.Name, xmlReader.Value, group);
                    break;
                case XmlAttributeName.KvgPartial:
                    attributes.IsPartial = GetBoolean(xmlReader.Name, xmlReader.Value, group);
                    break;
                case XmlAttributeName.KvgOriginal:
                    attributes.Original = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgPart:
                    attributes.Part = GetInt(xmlReader.Name, xmlReader.Value, group);
                    break;
                case XmlAttributeName.KvgNumber:
                    attributes.Number = GetInt(xmlReader.Name, xmlReader.Value, group);
                    break;
                case XmlAttributeName.KvgTradForm:
                    attributes.IsTradForm = GetBoolean(xmlReader.Name, xmlReader.Value, group);
                    break;
                case XmlAttributeName.KvgRadicalForm:
                    attributes.IsRadicalForm = GetBoolean(xmlReader.Name, xmlReader.Value, group);
                    break;
                case XmlAttributeName.KvgPosition:
                    attributes.Position = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgRadical:
                    attributes.Radical = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgPhon:
                    attributes.Phon = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgNamespace:
                    // Nothing to be done.
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, group);
                    break;
            }
        }

        xmlReader.MoveToElement();

        if (attributes.Id is null)
        {
            LogMissingId(group);
            attributes.Id = Guid.NewGuid().ToString();
        }

        return attributes;
    }

    private bool GetBoolean(string attributeName, string attributeValue, ComponentGroupElement group)
    {
        if (bool.TryParse(attributeValue, out bool value))
        {
            return value;
        }
        else
        {
            LogUnparsableText(attributeName, attributeValue, group);
            return default;
        }
    }

    private int GetInt(string attributeName, string attributeValue, ComponentGroupElement group)
    {
        if (int.TryParse(attributeValue, out int value))
        {
            return value;
        }
        else
        {
            LogUnparsableText(attributeName, attributeValue, group);
            return default;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Unknown component attribute name `{Name}` with value `{Value}` for component group {Group}")]
    partial void LogUnknownAttributeName(string name, string value, ComponentGroupElement group);

    [LoggerMessage(LogLevel.Warning,
    "Value `{Value}` for attribute name `{Name}` in component group `{Group}` cannot be parsed")]
    partial void LogUnparsableText(string name, string value, ComponentGroupElement group);

    [LoggerMessage(LogLevel.Warning,
    "Component group `{Group}` contains a component with no ID attribute")]
    partial void LogMissingId(ComponentGroupElement group);
}
