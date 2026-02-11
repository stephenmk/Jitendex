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
using Jitendex.JMdict.Import.Models;

namespace Jitendex.JMdict.Import.Parsing.EntryElementReaders.SenseElementReaders;

internal sealed class PartOfSpeechReader(ILogger<PartOfSpeechReader> logger) : BaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document,SenseElement sense)
    {
        var description = await xmlReader.ReadElementContentAsStringAsync();

        if (!document.KeywordDescriptionToName.TryGetValue(description, out var tagName))
        {
            tagName = description;
            document.KeywordDescriptionToName[description] = description;
            LogMissingEntityDefinition(description);
        }

        if (!document.PartOfSpeechTags.ContainsKey(tagName))
        {
            var tag = new PartOfSpeechTagElement(tagName, document.Header.Date);
            document.PartOfSpeechTags.Add(tagName, tag);
        }

        var partOfSpeech = new PartOfSpeechElement
        (
            EntryId: sense.EntryId,
            ParentOrder: sense.Order,
            Order: document.PartsOfSpeech.NextOrder(sense.Key()),
            TagName: tagName
        );

        document.PartsOfSpeech.Add(partOfSpeech.Key(), partOfSpeech);
    }
}
