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
using Jitendex.KanjiVG.Entities;
using Jitendex.KanjiVG.Import.Models;

namespace Jitendex.KanjiVG.Import.Readers;

internal partial class ComponentAttributesReader(ILogger<ComponentAttributesReader> logger)
{
    public ComponentAttributes Read(XmlReader xmlReader, ComponentGroup group)
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
                case "id":
                    attributes.Id = xmlReader.Value;
                    break;
                case "kvg:element":
                    attributes.Text = xmlReader.Value;
                    break;
                case "kvg:variant":
                    attributes.IsVariant = GetBoolean(xmlReader.Name, xmlReader.Value, group);
                    break;
                case "kvg:partial":
                    attributes.IsPartial = GetBoolean(xmlReader.Name, xmlReader.Value, group);
                    break;
                case "kvg:original":
                    attributes.Original = xmlReader.Value;
                    break;
                case "kvg:part":
                    attributes.Part = GetInt(xmlReader.Name, xmlReader.Value, group);
                    break;
                case "kvg:number":
                    attributes.Number = GetInt(xmlReader.Name, xmlReader.Value, group);
                    break;
                case "kvg:tradForm":
                    attributes.IsTradForm = GetBoolean(xmlReader.Name, xmlReader.Value, group);
                    break;
                case "kvg:radicalForm":
                    attributes.IsRadicalForm = GetBoolean(xmlReader.Name, xmlReader.Value, group);
                    break;
                case "kvg:position":
                    attributes.Position = xmlReader.Value;
                    break;
                case "kvg:radical":
                    attributes.Radical = xmlReader.Value;
                    break;
                case "kvg:phon":
                    attributes.Phon = xmlReader.Value;
                    break;
                case "xmlns:kvg":
                    // Nothing to be done.
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, group.Entry.FileName());
                    break;
            }
        }

        xmlReader.MoveToElement();

        if (attributes.Id is null)
        {
            LogMissingId(group.Entry.FileName(), group.XmlIdAttribute());
            attributes.Id = Guid.NewGuid().ToString();
        }

        return attributes;
    }

    private bool GetBoolean(string attributeName, string attributeValue, ComponentGroup group)
    {
        if (bool.TryParse(attributeValue, out bool value))
        {
            return value;
        }
        else
        {
            LogUnparsableText(attributeName, attributeValue, group.Entry.FileName());
            return default;
        }
    }

    private int GetInt(string attributeName, string attributeValue, ComponentGroup group)
    {
        if (int.TryParse(attributeValue, out int value))
        {
            return value;
        }
        else
        {
            LogUnparsableText(attributeName, attributeValue, group.Entry.FileName());
            return default;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Unknown component attribute name `{Name}` with value `{Value}` in file `{File}`")]
    partial void LogUnknownAttributeName(string name, string value, string file);

    [LoggerMessage(LogLevel.Warning,
    "Value `{Value}` for attribute name `{Name}` in file `{File}` cannot be parsed")]
    partial void LogUnparsableText(string name, string value, string file);

    [LoggerMessage(LogLevel.Warning,
    "File `{File}` component group `{GroupId}` contains a component with no ID attribute")]
    partial void LogMissingId(string file, string groupId);
}
