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
using Jitendex.Import.KanjiVG.Models;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.KanjiVG.Readers;

internal partial class StrokeNumberReader(ILogger<StrokeNumberReader> logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, StrokeNumberGroupElement group)
    {
        var strokeNumber = new StrokeNumberElement
        {
            UnicodeScalarValue = group.UnicodeScalarValue,
            VariantTypeId = group.VariantTypeId,
            Order = document.StrokeNumbers.NextOrder(group.Key()),
            TransformAttribute = GetTransformAttribute(xmlReader, group),
            Number = await xmlReader.ReadElementContentAsStringAsync(),
        };

        document.StrokeNumbers.Add(strokeNumber.Key(), strokeNumber);
    }

    private string GetTransformAttribute(XmlReader xmlReader, StrokeNumberGroupElement group)
    {
        string? transform = null;

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case XmlAttributeName.Transform:
                    transform = xmlReader.Value;
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

        if (transform is null)
        {
            LogMissingAttribute(group, XmlAttributeName.Transform);
            transform = string.Empty;
        }

        return transform;
    }

    [LoggerMessage(LogLevel.Warning,
    "Unknown component attribute name `{Name}` with value `{Value}` in stroke number group `{Group}`")]
    partial void LogUnknownAttributeName(string name, string value, StrokeNumberGroupElement group);

    [LoggerMessage(LogLevel.Warning,
    "Cannot find stroke number `{AttributeName}` attribute in stroke number group `{Group}`")]
    partial void LogMissingAttribute(StrokeNumberGroupElement group, string attributeName);
}
