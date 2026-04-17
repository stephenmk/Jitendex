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
using Jitendex.Import.Kanjidic2.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.Kanjidic2.Readers.GroupReaders;

internal partial class MiscGroupReader(ILogger<MiscGroupReader> logger)
    : XmlParentElementReader<Document, MiscGroupElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var group = new MiscGroupElement
        (
            EntryId: entry.Id,
            Order: document.MiscGroups.NextOrder(entry.Id)
        );

        await ReadToEndAsync(xmlReader, document, group, XmlTagName.MiscGroup);

        document.MiscGroups.Add(group.Key(), group);
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, MiscGroupElement group)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Grade:
                await ReadGrade(xmlReader, group);
                break;
            case XmlTagName.Frequency:
                await ReadFrequency(xmlReader, group);
                break;
            case XmlTagName.Jlpt:
                await ReadJlpt(xmlReader, group);
                break;
            case XmlTagName.StrokeCount:
                await ReadStrokeCount(xmlReader, document, group);
                break;
            case XmlTagName.Variant:
                await ReadVariant(xmlReader, document, group);
                break;
            case XmlTagName.RadicalName:
                await ReadRadicalName(xmlReader, document, group);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.MiscGroup);
                break;
        }
    }

    private async Task ReadGrade(XmlReader xmlReader, MiscGroupElement group)
    {
        var text = await xmlReader.ReadElementContentAsStringAsync();
        if (int.TryParse(text, out int value))
        {
            group.Grade = value;
        }
        else
        {
            LogNonNumeric(group.ToRune(), XmlTagName.Grade, text);
        }
    }

    private async Task ReadFrequency(XmlReader xmlReader, MiscGroupElement group)
    {
        var text = await xmlReader.ReadElementContentAsStringAsync();
        if (int.TryParse(text, out int value))
        {
            group.Frequency = value;
        }
        else
        {
            LogNonNumeric(group.ToRune(), XmlTagName.Frequency, text);
        }
    }

    private async Task ReadJlpt(XmlReader xmlReader, MiscGroupElement group)
    {
        var text = await xmlReader.ReadElementContentAsStringAsync();
        if (int.TryParse(text, out int value))
        {
            group.JlptLevel = value;
        }
        else
        {
            LogNonNumeric(group.ToRune(), XmlTagName.Jlpt, text);
        }
    }

    private async Task ReadStrokeCount(XmlReader xmlReader, Document document, MiscGroupElement group)
    {
        var text = await xmlReader.ReadElementContentAsStringAsync();

        if (!int.TryParse(text, out int value))
        {
            LogNonNumeric(group.ToRune(), XmlTagName.StrokeCount, text);
            return;
        }

        var strokeCount = new StrokeCountElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.StrokeCounts.NextOrder(group.Key()),
            Value: value
        );

        document.StrokeCounts.Add(strokeCount.Key(), strokeCount);
    }

    private async Task ReadVariant(XmlReader xmlReader, Document document, MiscGroupElement group)
    {
        var variant = new VariantElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.Variants.NextOrder(group.Key()),
            TypeName: GetVariantTypeName(xmlReader, document, group),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );
        document.Variants.Add(variant.Key(), variant);
    }

    private string GetVariantTypeName(XmlReader xmlReader, Document document, MiscGroupElement group)
    {
        var typeName = xmlReader.GetAttribute(XmlAttributeName.VariantType) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(typeName))
        {
            LogMissingTypeName(group.ToRune());
        }

        document.VariantTypes.Add(typeName);

        return typeName;
    }

    private async Task ReadRadicalName(XmlReader xmlReader, Document document, MiscGroupElement group)
    {
        var radicalName = new RadicalNameElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.RadicalNames.NextOrder(group.Key()),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );
        document.RadicalNames.Add(radicalName.Key(), radicalName);
    }

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` contains a non-numeric <{TagName}> value : `{Text}")]
    partial void LogNonNumeric(Rune character, string tagName, string text);

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` is missing a variant type attribute")]
    partial void LogMissingTypeName(Rune character);
}
