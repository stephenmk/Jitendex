/*
Copyright (c) 2026 Stephen Kraus
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
using Microsoft.Extensions.Logging;
using Jitendex.Import;
using Jitendex.JMnedict.Import.Models;

namespace Jitendex.JMnedict.Import.Parsing.EntryElementReaders.TranslationElementReaders;

internal sealed class DetailReader(ILogger<DetailReader> logger) : XmlBaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, TranslationElement translation)
    {
        var languageName = xmlReader.GetAttribute(XmlAttributeName.DetailLanguage);
        if (languageName is not null && !document.DetailLanguages.ContainsKey(languageName))
        {
            var language = new DetailLanguageElement(languageName, document.ArchiveKey);
            document.DetailLanguages.Add(languageName, language);
        }

        var detail = new DetailElement
        (
            EntryId: translation.EntryId,
            ParentOrder: translation.Order,
            Order: document.Details.NextOrder(translation.Key()),
            Text: await xmlReader.ReadElementContentAsStringAsync(),
            LanguageName: languageName
        );

        document.Details.Add(detail.Key(), detail);
    }
}
