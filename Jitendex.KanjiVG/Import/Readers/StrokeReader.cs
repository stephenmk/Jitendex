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
using Jitendex.Import;

namespace Jitendex.KanjiVG.Import.Readers;

internal partial class StrokeReader(ILogger<StrokeReader> logger)
{
    private sealed record Attributes(string Id, string TypeText, string PathData);

    public void Read(XmlReader xmlReader, Document document, ComponentGroupElement group, ComponentElement component)
    {
        var attributes = GetAttributes(xmlReader, component);

        var stroke = new StrokeElement
        {
            UnicodeScalarValue = component.UnicodeScalarValue,
            VariantTypeId = component.VariantTypeId,
            Order = document.Strokes.NextOrder(group.Key()),
            ComponentOrder = component.Order,
            IdAttribute = attributes.Id,
            TypeId = document.StrokeTypes.GetLookupId(attributes.TypeText),
            PathData = attributes.PathData,
        };

        document.Strokes.Add(stroke.Key(), stroke);
    }

    private Attributes GetAttributes(XmlReader xmlReader, ComponentElement component)
    {
        string? id = null;
        string typeText = string.Empty;
        string? pathData = null;

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case XmlAttributeName.Id:
                    id = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgType:
                    typeText = xmlReader.Value;
                    break;
                case XmlAttributeName.PathData:
                    pathData = xmlReader.Value;
                    break;
                case XmlAttributeName.KvgNamespace:
                    // Nothing to be done.
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, component);
                    break;
            }
        }

        xmlReader.MoveToElement();

        if (id is null)
        {
            LogMissingAttribute(XmlAttributeName.Id, component);
            id = Guid.NewGuid().ToString();
        }

        if (pathData is null)
        {
            LogMissingAttribute(XmlAttributeName.PathData, component);
            pathData = string.Empty;
        }

        return new(id, typeText, pathData);
    }

    [LoggerMessage(LogLevel.Warning,
    "Unknown component attribute name `{Name}` with value `{Value}` for component `{Component}`")]
    partial void LogUnknownAttributeName(string name, string value, ComponentElement component);

    [LoggerMessage(LogLevel.Warning,
    "Stroke ID `{Id}` for component `{Component}` is non-empty")]
    partial void LogNonEmptyElement(string id, ComponentElement component);

    [LoggerMessage(LogLevel.Warning,
    "Cannot find stroke attribute `{AttributeName}` for component `{Component}`")]
    partial void LogMissingAttribute(string attributeName, ComponentElement component);
}
