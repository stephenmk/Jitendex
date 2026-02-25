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
using Jitendex.Import;
using Jitendex.JMdict.Import.Models;

namespace Jitendex.JMdict.Import.Parsing;

internal partial class DocumentTypeReader(ILogger<DocumentTypeReader> logger) : XmlBaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document)
    {
        var exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.DocumentType:
                    var dtd = await xmlReader.GetValueAsync();
                    ParseEntities(document, dtd);
                    exit = true;
                    break;
                case XmlNodeType.Element:
                    LogUnexpectedChildElement(xmlReader, XmlTagName.Root);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(XmlTagName.Root, text);
                    break;
            }
        }
    }

    private void ParseEntities(Document document, string dtd)
    {
        foreach (Match match in DtdEntityRegex().Matches(dtd))
        {
            var name = match.Groups[1].Value;
            var description = match.Groups[2].Value;

            if (!document.KeywordDescriptionToName.TryGetValue(description, out var oldName))
            {
                document.KeywordDescriptionToName.Add(description, name);
            }
            else if (!string.Equals(name, oldName, StringComparison.Ordinal))
            {
                LogMultipleDescriptions(description);
            }
        }
    }

    [GeneratedRegex(@"<!ENTITY\s+(.*?)\s+""(.*?)"">", RegexOptions.None)]
    private static partial Regex DtdEntityRegex();

    [LoggerMessage(LogLevel.Warning,
    "Keyword description `{Description}` corresponds to multiple keyword names in the Jmdict DTD")]
    partial void LogMultipleDescriptions(string description);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected XML text node found before DTD: <{TagName}>: `{Text}`")]
    partial void LogUnexpectedTextNode(string tagName, string text);
}
