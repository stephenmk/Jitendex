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
using Attributes = (string Id, string TypeText, string PathData);

namespace Jitendex.KanjiVG.Readers;

internal partial class StrokeReader(ILogger<StrokeReader> logger, StrokeTypeCache strokeTypeCache)
{
    public void Read(XmlReader xmlReader, Component component)
    {
        var (id, typeText, pathData) = GetAttributes(xmlReader, component);

        var type = strokeTypeCache.Get(typeText);

        var stroke = new Stroke
        {
            UnicodeScalarValue = component.UnicodeScalarValue,
            VariantTypeId = component.VariantTypeId,
            GlobalOrder = component.Group.StrokeCount() + 1,
            LocalOrder = component.Strokes.Count + 1,
            ComponentGlobalOrder = component.GlobalOrder,
            TypeId = type.Id,
            PathData = pathData,
            Component = component,
            Type = type,
        };

        type.Strokes.Add(stroke);
        component.Strokes.Add(stroke);

        if (!xmlReader.IsEmptyElement)
        {
            LogNonEmptyElement(stroke.XmlIdAttribute(), component.Group.Entry.FileName());
        }

        if (!string.Equals(id, stroke.XmlIdAttribute(), StringComparison.Ordinal))
        {
            LogWrongId(stroke.XmlIdAttribute(), id, stroke.XmlIdAttribute());
        }
    }

    private Attributes GetAttributes(XmlReader xmlReader, Component component)
    {
        var attributes = new Attributes(null!, string.Empty, null!);

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case "id":
                    attributes.Id = xmlReader.Value;
                    break;
                case "kvg:type":
                    attributes.TypeText = xmlReader.Value;
                    break;
                case "d":
                    attributes.PathData = xmlReader.Value;
                    break;
                case "xmlns:kvg":
                    // Nothing to be done.
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, component.Group.Entry.FileName());
                    break;
            }
        }

        xmlReader.MoveToElement();

        if (attributes.Id is null)
        {
            LogMissingAttribute(nameof(attributes.Id), component.Group.Entry.FileName());
            attributes.Id = Guid.NewGuid().ToString();
        }

        if (attributes.PathData is null)
        {
            LogMissingAttribute(nameof(attributes.PathData), component.Group.Entry.FileName());
            attributes.Id = string.Empty;
        }

        return attributes;
    }

    [LoggerMessage(LogLevel.Warning,
    "Unknown component attribute name `{Name}` with value `{Value}` in file `{File}`")]
    partial void LogUnknownAttributeName(string name, string value, string file);

    [LoggerMessage(LogLevel.Warning,
    "Stroke ID `{Id}` in file `{FileName}` is non-empty")]
    partial void LogNonEmptyElement(string id, string fileName);

    [LoggerMessage(LogLevel.Warning,
    "Cannot find stroke attribute `{AttributeName}` in file `{File}`")]
    partial void LogMissingAttribute(string attributeName, string file);

    [LoggerMessage(LogLevel.Warning,
    "{File}: Stroke ID `{Actual}` not equal to expected value `{Expected}`")]
    partial void LogWrongId(string file, string actual, string expected);
}
