// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DictionaryGroupReader.cs, is part of Jitendex.
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

using System.Text;
using System.Xml;
using Jitendex.Import.Kanjidic2.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.Kanjidic2.Readers.GroupReaders;

internal partial class DictionaryGroupReader(ILogger<DictionaryGroupReader> logger)
    : XmlParentElementReader<Document, DictionaryGroupElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var group = new DictionaryGroupElement
        (
            EntryId: entry.Id,
            Order: document.DictionaryGroups.NextOrder(entry.Id)
        );

        await ReadToEndAsync(xmlReader, document, group, XmlTagName.DictionaryGroup);

        document.DictionaryGroups.Add(group.Key(), group);
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, DictionaryGroupElement group)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Dictionary:
                await ReadDictionary(xmlReader, document, group);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.DictionaryGroup);
                break;
        }
    }

    private async Task ReadDictionary(XmlReader xmlReader, Document document, DictionaryGroupElement group)
    {
        var dictionary = new DictionaryElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.Dictionaries.NextOrder(group.Key()),
            TypeName: GetTypeName(xmlReader, document, group),
            Volume: GetDictionaryVolume(xmlReader, group),
            Page: GetDictionaryPage(xmlReader, group),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );
        document.Dictionaries.Add(dictionary.Key(), dictionary);
    }

    private string GetTypeName(XmlReader xmlReader, Document document, DictionaryGroupElement group)
    {
        var typeName = xmlReader.GetAttribute(XmlAttributeName.DictionaryType) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(typeName))
        {
            LogMissingTypeName(group.ToRune());
        }

        document.DictionaryTypes.Add(typeName);

        return typeName;
    }

    private int? GetDictionaryVolume(XmlReader xmlReader, DictionaryGroupElement group)
    {
        var volume = xmlReader.GetAttribute(XmlAttributeName.Volume);
        if (volume is null)
        {
            // Not an error; allowed to be null
            return null;
        }
        if (int.TryParse(volume, out int value))
        {
            return value;
        }
        else
        {
            LogNonNumericVolume(group.ToRune(), volume);
            return null;
        }
    }

    private int? GetDictionaryPage(XmlReader xmlReader, DictionaryGroupElement group)
    {
        var page = xmlReader.GetAttribute(XmlAttributeName.Page);
        if (page is null)
        {
            // Not an error; allowed to be null
            return null;
        }
        if (int.TryParse(page, out int value))
        {
            return value;
        }
        else
        {
            LogNonNumericPage(group.ToRune(), page);
            return null;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` is missing a dictionary type attribute")]
    partial void LogMissingTypeName(Rune character);

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` has a dictionary volume attribute that is non-numeric: `{Volume}`")]
    partial void LogNonNumericVolume(Rune character, string volume);

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` has a dictionary page attribute that is non-numeric: `{Page}`")]
    partial void LogNonNumericPage(Rune character, string page);
}
