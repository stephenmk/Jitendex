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
using Jitendex.JMnedict.Import.Models;

namespace Jitendex.JMnedict.Import.Parsing.EntryElementReaders.TranslationElementReaders;

internal partial class DetailReader(ILogger<DetailReader> logger) : BaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, TranslationElement translation)
    {
        if (xmlReader.GetAttribute(XmlAttributeName.DetailLanguage) is not null)
        {
            LogNonEnglishTranslation(translation.EntryId);
            await xmlReader.SkipAsync();
        }

        var detail = new DetailElement
        (
            EntryId: translation.EntryId,
            ParentOrder: translation.Order,
            Order: document.Details.NextOrder(translation.Key()),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );

        document.Details.Add(detail.Key(), detail);
    }

    [LoggerMessage(LogLevel.Error,
    "Entry `{EntryId}` contains a non-English translation")]
    partial void LogNonEnglishTranslation(int entryId);
}
