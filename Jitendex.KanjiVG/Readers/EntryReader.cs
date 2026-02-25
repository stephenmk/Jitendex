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

using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.KanjiVG.Entities;
using Jitendex.KanjiVG.Readers.Lookups;

namespace Jitendex.KanjiVG.Readers;

internal partial class EntryReader
(
    ILogger<EntryReader> logger,
    ComponentGroupReader componentGroupReader,
    StrokeNumberGroupReader strokeNumberGroupReader,
    VariantTypeCache variantTypeCache,
    CommentCache commentCache
)
{
    public async Task<Entry?> ReadAsync(string fileName, XmlReader xmlReader)
    {
        var (unicodeScalarValue, variantTypeName) = Parse(fileName);
        if (unicodeScalarValue == default)
        {
            return null;
        }

        var variantType = variantTypeCache.Get(variantTypeName);

        var entry = new Entry
        {
            UnicodeScalarValue = unicodeScalarValue,
            VariantTypeId = variantType.Id,
            CommentId = default,
            ComponentGroup = null!,
            StrokeNumberGroup = null!,
            VariantType = variantType,
            Comment = null!,
        };

        variantType.Entries.Add(entry);

        while (await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, entry);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(fileName, text);
                    break;
                case XmlNodeType.Comment:
                    await ReadCommentAsync(xmlReader, entry);
                    break;
                case XmlNodeType.DocumentType:
                    break;
            }
        }

        return IsEntryValid(entry, fileName)
            ? entry
            : null;
    }

    private bool IsEntryValid(Entry entry, string fileName)
    {
        bool isValid = true;
        if (entry.Comment is null)
        {
            LogMissingGroup(nameof(entry.Comment), fileName);
            isValid = false;
        }
        if (entry.ComponentGroup is null)
        {
            LogMissingGroup(nameof(entry.ComponentGroup), fileName);
            isValid = false;
        }
        if (entry.StrokeNumberGroup is null)
        {
            LogMissingGroup(nameof(entry.StrokeNumberGroup), fileName);
            isValid = false;
        }
        return isValid;
    }

    private (int, string) Parse(string fileName)
    {
        Match match = FileNameRegex().Match(fileName);
        if (!match.Success)
        {
            logger.LogError("Cannot parse filename {FileName}", fileName);
            return (default, string.Empty);
        }
        else if (int.TryParse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier, provider: null, out int value))
        {
            return (value, match.Groups[2].Value);
        }
        else
        {
            logger.LogError("Hex code in filename {FileName} is invalid", fileName);
            return (default, string.Empty);
        }
    }

    private async Task ReadCommentAsync(XmlReader xmlReader, Entry entry)
    {
        if (entry.Comment is not null)
        {
            logger.LogWarning("File `{File}` contains multiple header comments", entry.FileName());
        }
        var commentText = await xmlReader.GetValueAsync();
        entry.Comment = commentCache.Get(commentText);
        entry.Comment.Entries.Add(entry);
        entry.CommentId = entry.Comment.Id;
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Entry entry)
    {
        switch (xmlReader.Name)
        {
            case "svg":
                ReadSvgHeader(xmlReader, entry);
                break;
            case "g":
                await ReadGroupAsync(xmlReader, entry);
                break;
            default:
                LogUnexpectedElementName(xmlReader.Name, entry.FileName());
                break;
        }
    }

    private async Task ReadGroupAsync(XmlReader xmlReader, Entry entry)
    {
        var id = xmlReader.GetAttribute("id");
        if (id is null)
        {
            LogMissingGroupId(entry.FileName());
        }
        else if (id.StartsWith("kvg:StrokePaths", StringComparison.Ordinal))
        {
            await componentGroupReader.ReadAsync(xmlReader, entry);
        }
        else if (id.StartsWith("kvg:StrokeNumbers", StringComparison.Ordinal))
        {
            await strokeNumberGroupReader.ReadAsync(xmlReader, entry);
        }
        else
        {
            LogUnexpectedGroupIdPrefix(id, entry.FileName());
        }
    }

    private void ReadSvgHeader(XmlReader xmlReader, Entry entry)
    {
        string? width = null,
                height = null,
                viewBox = null;
        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case nameof(width):
                    width = xmlReader.Value;
                    break;
                case nameof(height):
                    height = xmlReader.Value;
                    break;
                case nameof(viewBox):
                    viewBox = xmlReader.Value;
                    break;
                case "xmlns":
                    // Nothing to be done.
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, entry.FileName());
                    break;
            }
        }
        xmlReader.MoveToElement();
        if (!string.Equals(width, "109", StringComparison.Ordinal))
        {
            LogAbnormalSvgAttribute(nameof(width), width, entry.FileName());
        }
        if (!string.Equals(height, "109", StringComparison.Ordinal))
        {
            LogAbnormalSvgAttribute(nameof(height), height, entry.FileName());
        }
        if (!string.Equals(viewBox, "0 0 109 109", StringComparison.Ordinal))
        {
            LogAbnormalSvgAttribute(nameof(viewBox), viewBox, entry.FileName());
        }
    }

    [GeneratedRegex(pattern: @"^(.+?)(?:-(.+?))?\.svg$", RegexOptions.None)]
    private static partial Regex FileNameRegex();

    [LoggerMessage(LogLevel.Warning,
    "{File}: Unexpected XML text node `{Text}`")]
    partial void LogUnexpectedTextNode(string file, string text);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected element name `{Name}` in file `{FileName}`")]
    partial void LogUnexpectedElementName(string name, string fileName);

    [LoggerMessage(LogLevel.Warning,
    "No `{GroupName}` group found in file `{FileName}`")]
    partial void LogMissingGroup(string groupName, string fileName);

    [LoggerMessage(LogLevel.Warning,
    "Group element in file `{FileName}` is missing an ID attribute")]
    partial void LogMissingGroupId(string fileName);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected group element ID `{Id}` in file `{FileName}`")]
    partial void LogUnexpectedGroupIdPrefix(string id, string fileName);

    [LoggerMessage(LogLevel.Warning,
    "Unknown SVG attribute name `{Name}` with value `{Value}` in file `{File}`")]
    partial void LogUnknownAttributeName(string name, string value, string file);

    [LoggerMessage(LogLevel.Warning,
    "Abnormal SVG `{Name}` attribute `{Value}` in file `{File}`")]
    partial void LogAbnormalSvgAttribute(string name, string? value, string file);
}
