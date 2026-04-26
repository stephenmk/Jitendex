// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, QueryCodeGroupReader.cs, is part of Jitendex.
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

internal partial class QueryCodeGroupReader(ILogger<QueryCodeGroupReader> logger)
    : XmlParentElementReader<Document, QueryCodeGroupElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var group = new QueryCodeGroupElement
        (
            EntryId: entry.Id,
            Order: document.QueryCodeGroups.NextOrder(entry.Id)
        );

        await ReadToEndAsync(xmlReader, document, group, XmlTagName.QueryCodeGroup);

        document.QueryCodeGroups.Add(group.Key(), group);
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, QueryCodeGroupElement group)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.QueryCode:
                await ReadQueryCode(xmlReader, document, group);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.QueryCodeGroup);
                break;
        }
    }

    private async Task ReadQueryCode(XmlReader xmlReader, Document document, QueryCodeGroupElement group)
    {
        var queryCode = new QueryCodeElement
        (
            EntryId: group.EntryId,
            GroupOrder: group.Order,
            Order: document.QueryCodes.NextOrder(group.Key()),
            TypeName: GetTypeName(xmlReader, document, group),
            Misclassification: GetMisclassification(xmlReader, document),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );
        document.QueryCodes.Add(queryCode.Key(), queryCode);
    }

    private string GetTypeName(XmlReader xmlReader, Document document, QueryCodeGroupElement group)
    {
        var typeName = xmlReader.GetAttribute(XmlAttributeName.QueryCodeType) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(typeName))
        {
            LogMissingTypeName(group.ToRune());
        }

        document.QueryCodeTypes.Add(typeName);

        return typeName;
    }

    private string? GetMisclassification(XmlReader xmlReader, Document document)
    {
        var typeName = xmlReader.GetAttribute(XmlAttributeName.MisclassificationType);

        if (string.IsNullOrWhiteSpace(typeName))
        {
            return null;
        }

        document.MisclassificationTypes.Add(typeName);

        return typeName;
    }

    [LoggerMessage(LogLevel.Warning,
    "Character `{Character}` is missing a query code type attribute")]
    partial void LogMissingTypeName(Rune character);
}
