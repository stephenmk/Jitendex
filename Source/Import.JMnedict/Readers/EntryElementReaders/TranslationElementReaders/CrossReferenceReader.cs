// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, CrossReferenceReader.cs, is part of Jitendex.
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
using Jitendex.Import.JMnedict.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMnedict.Readers.EntryElementReaders.TranslationElementReaders;

internal sealed class CrossReferenceReader(ILogger<CrossReferenceReader> logger) : XmlBaseReader(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, TranslationRow translation)
    {
        var xref = new CrossReferenceRow
        (
            EntryId: translation.EntryId,
            ParentOrder: translation.Order,
            Order: document.CrossReferences.NextOrder(translation.Key()),
            Text: await xmlReader.ReadElementContentAsStringAsync()
        );

        document.CrossReferences.Add(xref.Key(), xref);
    }
}
