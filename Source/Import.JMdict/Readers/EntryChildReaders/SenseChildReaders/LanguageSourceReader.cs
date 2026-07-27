// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, LanguageSourceReader.cs, is part of Jitendex.
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

using System.Xml;
using Jitendex.Import.JMdict.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMdict.Readers.EntryChildReaders.SenseChildReaders;

internal partial class LanguageSourceReader(ILogger<LanguageSourceReader> logger) : XmlBaseReader(logger)
{
    public Task ReadAsync(XmlReader xmlReader, Document document, EntryRow entry)
        => ReadAsync(xmlReader, document, entry.Id, null);

    public Task ReadAsync(XmlReader xmlReader, Document document, SenseRow sense)
        => ReadAsync(xmlReader, document, sense.EntryId, sense.Order);

    private async Task ReadAsync(XmlReader xmlReader, Document document, int entryId, int? senseOrder)
    {
        var typeName = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceType) ?? "full";
        document.LanguageSourceTypes.Add(typeName);

        var languageCode = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceCode) ?? "eng";
        document.Languages.Add(languageCode);

        var wasei = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceWasei);
        if (wasei is not null && wasei != "y")
        {
            if (senseOrder.HasValue)
                LogInvalidWaseiValue(entryId, senseOrder.Value, wasei);
            else
                LogInvalidWaseiValue(entryId, wasei);
        }

        var text = xmlReader.IsEmptyElement
            ? null
            : await xmlReader.ReadElementContentAsStringAsync();

        var languageSource = new LanguageSourceRow
        (
            EntryId: entryId,
            Order: document.LanguageSources.NextOrder(entryId),
            Text: text,
            LanguageCode: languageCode,
            TypeName: typeName,
            IsWasei: wasei == "y"
        );

        document.LanguageSources.Add(languageSource.Key(), languageSource);
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry `{EntryId}` has a language source WASEI attribute with an invalid value: `{Value}`")]
    partial void LogInvalidWaseiValue(int entryId, string value);

    [LoggerMessage(LogLevel.Warning,
    "Entry `{EntryId}` sense #{SenseOrder} has a language source WASEI attribute with an invalid value: `{Value}`")]
    partial void LogInvalidWaseiValue(int entryId, int senseOrder, string value);
}
