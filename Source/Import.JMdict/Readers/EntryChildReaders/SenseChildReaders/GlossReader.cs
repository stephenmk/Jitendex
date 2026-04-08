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
using Jitendex.Import.JMdict.TableRows;

namespace Jitendex.Import.JMdict.Readers.EntryChildReaders.SenseChildReaders;

internal sealed class GlossReader(ILogger<GlossReader> logger) : XmlBaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, SenseRow sense)
    {
        var typeTag = xmlReader.GetAttribute(XmlAttributeName.GlossType);

        if (typeTag is not null)
        {
            document.GlossTypeTags.Add(typeTag);
        }

        var gloss = new GlossRow
        (
            EntryId: sense.EntryId,
            ParentOrder: sense.Order,
            Order: document.Glosses.NextOrder(sense.Key()),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );

        document.Glosses.Add(gloss.Key(), gloss);

        if (typeTag is not null)
        {
            var glossType = new GlossTypeRow
            (
                sense.EntryId,
                sense.Order,
                gloss.Order,
                typeTag
            );
            document.GlossTypes.Add(glossType.Key(), glossType);
        }
    }
}
