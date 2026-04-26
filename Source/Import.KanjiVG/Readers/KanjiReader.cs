// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, KanjiReader.cs, is part of Jitendex.
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

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Jitendex.Import.KanjiVG.Models;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.KanjiVG.Readers;

internal partial class KanjiReader
(
    ILogger<KanjiReader> logger,
    ComponentGroupReader componentGroupReader,
    StrokeNumberGroupReader strokeNumberGroupReader
)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, string fileName)
    {
        if (Parse(fileName) is not (int unicodeScalarValue, string variantTypeName))
        {
            return;
        }

        document.Kanjis.Add(unicodeScalarValue);

        var variant = new VariantElement
        {
            UnicodeScalarValue = unicodeScalarValue,
            TypeId = document.VariantTypes.GetLookupId(variantTypeName),
        };

        if (!document.Variants.TryAdd(variant.Key(), variant))
        {
            LogMultipleVariantEntries(fileName, new(unicodeScalarValue), variantTypeName);
            return;
        }

        while (await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, variant);
                    break;
                case XmlNodeType.Comment:
                    await ReadCommentAsync(xmlReader, document, variant);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(fileName, text);
                    break;
            }
        }

        if (variant.CommentIsUninitialized())
        {
            variant.CommentId = document.Comments.GetLookupId(string.Empty);
        }
    }

    private (int, string)? Parse(string fileName)
    {
        Match match = FileNameRegex.Match(fileName);
        if (!match.Success)
        {
            logger.LogError("Cannot parse filename {FileName}", fileName);
            return null;
        }
        else if (int.TryParse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier, provider: null, out int value))
        {
            return (value, match.Groups[2].Value);
        }
        else
        {
            logger.LogError("Hex code in filename {FileName} is invalid", fileName);
            return null;
        }
    }

    private async Task ReadCommentAsync(XmlReader xmlReader, Document document, VariantElement variant)
    {
        if (!variant.CommentIsUninitialized())
        {
            LogMultipleComments(variant);
        }
        var commentText = await xmlReader.GetValueAsync();
        variant.CommentId = document.Comments.GetLookupId(commentText);
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document, VariantElement variant)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Group:
                await ReadGroupAsync(xmlReader, document, variant);
                break;
            case XmlTagName.SvgHeader:
                ReadSvgHeader(xmlReader, document, variant);
                break;
            default:
                LogUnexpectedElementName(xmlReader.Name, variant);
                break;
        }
    }

    private async Task ReadGroupAsync(XmlReader xmlReader, Document document, VariantElement variant)
    {
        var id = xmlReader.GetAttribute(XmlAttributeName.Id);
        if (id is null)
        {
            LogMissingGroupId(variant);
        }
        else if (id.StartsWith(XmlAttributeName.KvgStrokePathsPrefix, StringComparison.Ordinal))
        {
            await componentGroupReader.ReadAsync(xmlReader, document, variant);
        }
        else if (id.StartsWith(XmlAttributeName.KvgStrokeNumbersPrefix, StringComparison.Ordinal))
        {
            await strokeNumberGroupReader.ReadAsync(xmlReader, document, variant);
        }
        else
        {
            LogUnexpectedGroupIdPrefix(id, variant);
        }
    }

    private void ReadSvgHeader(XmlReader xmlReader, Document document, VariantElement variant)
    {
        string? width = null;
        string? height = null;
        string? viewBox = null;

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case XmlAttributeName.Width:
                    width = xmlReader.Value;
                    break;
                case XmlAttributeName.Height:
                    height = xmlReader.Value;
                    break;
                case XmlAttributeName.ViewBox:
                    viewBox = xmlReader.Value;
                    break;
                case XmlAttributeName.XmlNamespace:
                    // Nothing to be done.
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, variant);
                    break;
            }
        }

        xmlReader.MoveToElement();

        if (!string.Equals(width, "109", StringComparison.Ordinal))
        {
            LogAbnormalSvgAttribute(nameof(width), width, variant);
        }
        if (!string.Equals(height, "109", StringComparison.Ordinal))
        {
            LogAbnormalSvgAttribute(nameof(height), height, variant);
        }
        if (!string.Equals(viewBox, "0 0 109 109", StringComparison.Ordinal))
        {
            LogAbnormalSvgAttribute(nameof(viewBox), viewBox, variant);
        }
    }

    [GeneratedRegex(pattern: @"^(.+?)(?:-(.+?))?\.svg$", RegexOptions.None)]
    private static partial Regex FileNameRegex { get; }

    [LoggerMessage(LogLevel.Warning,
    "{File}: Unexpected XML text node `{Text}`")]
    partial void LogUnexpectedTextNode(string file, string text);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected element name `{Name}` for variant {Variant}")]
    partial void LogUnexpectedElementName(string name, VariantElement variant);

    [LoggerMessage(LogLevel.Warning,
    "Group element for variant `{Variant}` is missing an ID attribute")]
    partial void LogMissingGroupId(VariantElement variant);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected group element ID `{Id}` for variant `{Variant}`")]
    partial void LogUnexpectedGroupIdPrefix(string id, VariantElement variant);

    [LoggerMessage(LogLevel.Warning,
    "Unknown SVG attribute name `{Name}` with value `{Value}` for variant `{Variant}`")]
    partial void LogUnknownAttributeName(string name, string value, VariantElement variant);

    [LoggerMessage(LogLevel.Warning,
    "File `{File}` redefines variant `{Rune}` - `{Variant}`")]
    partial void LogMultipleVariantEntries(string file, Rune rune, string variant);

    [LoggerMessage(LogLevel.Warning,
    "Variant `{Variant}` contains multiple file comments")]
    partial void LogMultipleComments(VariantElement variant);

    [LoggerMessage(LogLevel.Warning,
    "Abnormal SVG `{Name}` attribute `{Value}` in variant `{Variant}`")]
    partial void LogAbnormalSvgAttribute(string name, string? value, VariantElement variant);
}
