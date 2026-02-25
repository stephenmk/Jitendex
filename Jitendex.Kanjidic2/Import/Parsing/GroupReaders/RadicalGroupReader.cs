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
using Jitendex.Import;
using Jitendex.Kanjidic2.Import.Models;

namespace Jitendex.Kanjidic2.Import.Parsing.GroupReaders;

internal partial class RadicalGroupReader(ILogger<RadicalGroupReader> logger)
    : XmlParentElementReader<Document, RadicalGroupElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var group = new RadicalGroupElement
        (
            EntryId: entry.Id,
            Order: document.RadicalGroups.NextOrder(entry.Id)
        );

        await ReadToEndAsync(xmlReader, document, group, XmlTagName.RadicalGroup);

        document.RadicalGroups.Add(group.Key(), group);
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, RadicalGroupElement group)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Radical:
                await ReadRadical(xmlReader, document, group);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.RadicalGroup);
                break;
        }
    }

    private async Task ReadRadical(XmlReader xmlReader, Document document, RadicalGroupElement group)
    {
        var radical = new RadicalElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.Radicals.NextOrder(group.Key()),
            TypeName: GetTypeName(xmlReader, document, group),
            Number: await GetNumber(xmlReader, group)
        );
        document.Radicals.Add(radical.Key(), radical);
    }

    private string GetTypeName(XmlReader xmlReader, Document document, RadicalGroupElement group)
    {
        string typeName;
        var attribute = xmlReader.GetAttribute("rad_type");
        if (string.IsNullOrWhiteSpace(attribute))
        {
            LogMissingTypeName(group.ToRune());
            typeName = string.Empty;
        }
        else
        {
            typeName = attribute;
        }

        document.RadicalTypes.Add(typeName);

        return typeName;
    }

    private async Task<int> GetNumber(XmlReader xmlReader, RadicalGroupElement group)
    {
        var text = await xmlReader.ReadElementContentAsStringAsync();
        if (int.TryParse(text, out int value))
        {
            return value;
        }
        else
        {
            LogNonNumericRadicalNumber(group.ToRune(), text);
            return default;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` is missing a radical type attribute")]
    partial void LogMissingTypeName(Rune character);

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` has a radical number that is non-numeric: `{Text}`")]
    partial void LogNonNumericRadicalNumber(Rune character, string text);
}
