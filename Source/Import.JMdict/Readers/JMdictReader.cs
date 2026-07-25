// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, JMdictReader.cs, is part of Jitendex.
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
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMdict.Readers;

internal sealed class JMdictReader
(
    ILogger<JMdictReader> logger,
    EntryReader entryReader
) :
    XmlParentElementReader<Document, byte>(logger)
{
    public Task ReadAsync(XmlReader xmlReader, Document document)
        => ReadToEndAsync(xmlReader, document, default, XmlTagName.Jmdict);

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, byte _)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Entry:
                await entryReader.ReadAsync(xmlReader, document);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.Jmdict);
                break;
        }
    }
}
