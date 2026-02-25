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

namespace Jitendex.JMdict.Import.Parsing.EntryElementReaders.ReadingElementReaders;

internal sealed class RPriorityReader(ILogger<RPriorityReader> logger) : BaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, ReadingElement reading)
    {
        var tagName = await xmlReader.ReadElementContentAsStringAsync();

        if (!document.PriorityTags.ContainsKey(tagName))
        {
            var tag = new PriorityTagElement(tagName, document.ArchiveKey);
            document.PriorityTags.Add(tagName, tag);
        }

        var priority = new ReadingPriorityElement
        (
            EntryId: reading.EntryId,
            ParentOrder: reading.Order,
            Order: document.ReadingPriorities.NextOrder(reading.Key()),
            TagName: tagName
        );

        document.ReadingPriorities.Add(priority.Key(), priority);
    }
}
