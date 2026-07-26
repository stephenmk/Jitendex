// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, LanguageSourceNGReader.cs, is part of Jitendex.
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
using Jitendex.Import.JMdict.Readers;
using Jitendex.Import.JMdict.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMdict.NGReaders.EntryChildReaders;

internal sealed partial class LanguageSourceNGReader(ILogger<LanguageSourceNGReader> logger) : XmlBaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryRow entry)
    {
        var typeName = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceType) ?? "full";
        document.LanguageSourceTypes.Add(typeName);

        var languageCode = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceCode) ?? "eng";
        document.Languages.Add(languageCode);

        var wasei = xmlReader.GetAttribute(XmlAttributeName.LanguageSourceWasei);
        if (wasei is not null && wasei != "y")
        {
            LogInvalidWaseiValue(entry.Id, wasei);
        }

        var text = xmlReader.IsEmptyElement
            ? null
            : await xmlReader.ReadElementContentAsStringAsync();

        var languageSource = new LanguageSourceRow
        (
            EntryId: entry.Id,
            Order: document.LanguageSources.NextOrder(entry.Id),
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
}
