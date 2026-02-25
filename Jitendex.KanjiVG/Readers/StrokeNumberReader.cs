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

using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.KanjiVG.Models;

namespace Jitendex.KanjiVG.Readers;

internal partial class StrokeNumberReader(ILogger<StrokeNumberReader> logger)
{
    public async Task ReadAsync(XmlReader xmlReader, StrokeNumberGroup group)
    {
        var (translateX, translateY) = GetTranslation(xmlReader, group);

        var strokeNumber = new StrokeNumber
        {
            UnicodeScalarValue = group.Entry.UnicodeScalarValue,
            VariantTypeId = group.Entry.VariantTypeId,
            TranslateX = translateX,
            TranslateY = translateY,
            Number = await GetNumberAsync(xmlReader),
            Group = group,
        };

        int order = group.StrokeNumbers.Count + 1;
        if (strokeNumber.Number != order)
        {
            LogNumberOutOfOrder(group.Entry.FileName(), strokeNumber.Number, order);
        }

        group.StrokeNumbers.Add(strokeNumber);
    }

    private (string X, string Y) GetTranslation(XmlReader xmlReader, StrokeNumberGroup group)
    {
        var transform = GetTransformAttribute(xmlReader, group);

        Match match = TransformRegex().Match(transform);

        if (!match.Success)
        {
            LogMalformattedTransform(group.Entry.FileName(), transform);
            return (string.Empty, string.Empty);
        }

        var translateX = match.Groups[1].Value;
        var translateY = match.Groups[2].Value;

        if (!decimal.TryParse(translateX, out decimal _))
        {
            LogMalformattedTranslation(group.Entry.FileName(), "x", translateX);
        }

        if (!decimal.TryParse(translateY, out decimal _))
        {
            LogMalformattedTranslation(group.Entry.FileName(), "y", translateY);
        }

        return (translateX, translateY);
    }

    private string GetTransformAttribute(XmlReader xmlReader, StrokeNumberGroup group)
    {
        string? transform = null;

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case "transform":
                    transform = xmlReader.Value;
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

        if (transform is null)
        {
            LogMissingAttribute(group.Entry.FileName(), nameof(transform));
            transform = string.Empty;
        }

        return transform;
    }

    private async Task<int> GetNumberAsync(XmlReader xmlReader)
    {
        var text = await xmlReader.ReadElementContentAsStringAsync();
        if (int.TryParse(text, out int value))
        {
            return value;
        }
        else
        {
            LogUnparsableStrokeNumber(text);
            return -1;
        }
    }

    [GeneratedRegex(@"^matrix\(1 0 0 1 (-?[0-9.]+) (-?[0-9.]+)\)$", RegexOptions.None)]
    private static partial Regex TransformRegex();


    [LoggerMessage(LogLevel.Warning,
    "Unknown component attribute name `{Name}` with value `{Value}` in file `{File}`")]
    partial void LogUnknownAttributeName(string name, string value, string file);

    [LoggerMessage(LogLevel.Warning,
    "Cannot find stroke number `{AttributeName}` attribute in file `{File}`")]
    partial void LogMissingAttribute(string file, string attributeName);

    [LoggerMessage(LogLevel.Warning,
    "Stroke number text `{Text}` is not an integer")]
    partial void LogUnparsableStrokeNumber(string text);

    [LoggerMessage(LogLevel.Warning,
    "In file `{FileName}`, stroke number `{Number}` is not equal to its order `{Order}`")]
    partial void LogNumberOutOfOrder(string fileName, int number, int order);

    [LoggerMessage(LogLevel.Warning,
    "In file `{FileName}`, stroke number transform attribute `{Attribute}` is not in the expected format")]
    partial void LogMalformattedTransform(string fileName, string attribute);

    [LoggerMessage(LogLevel.Warning,
    "In file `{FileName}`, stroke number {Axis}-axis translation `{Value}` is not a valid decimal number")]
    partial void LogMalformattedTranslation(string fileName, string axis, string value);
}
