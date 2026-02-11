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

using System.Text;
using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.Kanjidic2.Import.Models;
using Jitendex.Kanjidic2.Import.Parsing.GroupReaders;

namespace Jitendex.Kanjidic2.Import.Parsing;

internal partial class EntryReader
(
    ILogger<EntryReader> logger,
    CodepointGroupReader codepointGroupReader,
    DictionaryGroupReader dictionaryGroupReader,
    MiscGroupReader miscGroupReader,
    QueryCodeGroupReader queryCodeGroupReader,
    RadicalGroupReader radicalGroupReader,
    ReadingMeaningGroupReader readingMeaningGroupReader
) : BaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document)
    {
        var entry = new EntryElement
        {
            Id = default
        };

        var exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, entry);
                    break;
                case XmlNodeType.Text:
                    await LogUnexpectedTextNodeAsync(xmlReader, entry.Id, XmlTagName.Entry);
                    break;
                case XmlNodeType.EndElement:
                    exit = xmlReader.Name == XmlTagName.Entry;
                    break;
            }
        }

        if (entry.Id != default)
        {
            document.Entries.Add(entry.Id, entry);
        }
        else
        {
            LogMissingCharacter(XmlTagName.EntryCharacter);
        }
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        if (xmlReader.Name != XmlTagName.EntryCharacter && entry.Id == default)
        {
            LogPrematureElement(xmlReader.Name);
            return;
        }

        switch (xmlReader.Name)
        {
            case XmlTagName.EntryCharacter:
                await ReadCharacterAsync(xmlReader, entry);
                break;
            case XmlTagName.CodepointGroup:
                await codepointGroupReader.ReadAsync(xmlReader, document, entry);
                break;
            case XmlTagName.DictionaryGroup:
                await dictionaryGroupReader.ReadAsync(xmlReader, document, entry);
                break;
            case XmlTagName.MiscGroup:
                await miscGroupReader.ReadAsync(xmlReader, document, entry);
                break;
            case XmlTagName.QueryCodeGroup:
                await queryCodeGroupReader.ReadAsync(xmlReader, document, entry);
                break;
            case XmlTagName.RadicalGroup:
                await radicalGroupReader.ReadAsync(xmlReader, document, entry);
                break;
            case XmlTagName.ReadingMeaningGroup:
                await readingMeaningGroupReader.ReadAsync(xmlReader, document, entry);
                break;
            default:
                LogUnexpectedChildElement(entry.ToRune(), xmlReader.Name, XmlTagName.Entry);
                break;
        }
    }

    private async Task ReadCharacterAsync(XmlReader xmlReader, EntryElement entry)
    {
        if (entry.Id != default)
        {
            LogUnexpectedGroup(entry.ToRune(), XmlTagName.EntryCharacter);
        }

        var text = await xmlReader.ReadElementContentAsStringAsync();

        if (string.IsNullOrWhiteSpace(text))
        {
            LogEmptyElement(XmlTagName.EntryCharacter);
            return;
        }

        var runes = text.EnumerateRunes().ToArray();

        if (runes.Length > 1)
        {
            LogMultipleCharacters(XmlTagName.EntryCharacter, text);
        }

        entry.Id = runes[0].Value;
    }

    [LoggerMessage(LogLevel.Error,
    "Attempted to read <{XmlTagName}> child element before reading the entry primary key")]
    partial void LogPrematureElement(string xmlTagName);

    [LoggerMessage(LogLevel.Warning,
    "Entry contains no text in <{XmlTagName}> child element")]
    partial void LogEmptyElement(string xmlTagName);

    [LoggerMessage(LogLevel.Warning,
    "Entry contains more than one character in <{XmlTagName}> child element: `{Text}`")]
    partial void LogMultipleCharacters(string xmlTagName, string text);

    [LoggerMessage(LogLevel.Warning,
    "Entry for character `{Character}` has more than one <{XmlTagName}> child element")]
    partial void LogUnexpectedGroup(Rune character, string xmlTagName);

    [LoggerMessage(LogLevel.Error,
    "Entry contains no <{XmlTagName}> element; no primary key can be assigned")]
    partial void LogMissingCharacter(string xmlTagName);
}
