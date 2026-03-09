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

using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.Import.JMdict.Models;

namespace Jitendex.Import.JMdict.Parsing.EntryElementReaders.SenseElementReaders;

internal partial class LanguageSourceReader(ILogger<LanguageSourceReader> logger) : XmlBaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, SenseElement sense)
    {
        var typeName = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceType) ?? "full";
        document.LanguageSourceTypes.Add(typeName);

        var languageCode = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceCode) ?? "eng";
        document.Languages.Add(languageCode);

        var wasei = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceWasei);
        if (wasei is not null && wasei != "y")
        {
            LogInvalidWaseiValue(sense.EntryId, sense.Order, wasei);
        }

        var text = xmlReader.IsEmptyElement
            ? null
            : await xmlReader.ReadElementContentAsStringAsync();

        var languageSource = new LanguageSourceElement
        (
            EntryId: sense.EntryId,
            ParentOrder: sense.Order,
            Order: document.LanguageSources.NextOrder(sense.Key()),
            Text: text,
            LanguageCode: languageCode,
            TypeName: typeName,
            IsWasei: wasei == "y"
        );

        document.LanguageSources.Add(languageSource.Key(), languageSource);
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry `{EntryId}` sense #{SenseOrder} has a language source WASEI attribute with an invalid value: `{Value}`")]
    partial void LogInvalidWaseiValue(int entryId, int senseOrder, string value);
}
