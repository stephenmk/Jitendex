// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, XmlBaseReader.cs, is part of Jitendex.
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

namespace Jitendex.Import;

public abstract partial class XmlBaseReader
{
    protected readonly ILogger<XmlBaseReader> _logger;
    protected XmlBaseReader(ILogger<XmlBaseReader> logger)
        => _logger = logger;

    protected void LogUnexpectedChildElement(XmlReader xmlReader, string parentTagName)
        => LogUnexpectedChildElement(xmlReader.Name, parentTagName);

    [LoggerMessage(LogLevel.Warning,
    "XML document type `{Entity}` was not defined in DTD preamble")]
    protected partial void LogMissingEntityDefinition(string entity);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected XML element node <{TagName}> found in element <{ParentTagName}>")]
    private partial void LogUnexpectedChildElement(string tagName, string parentTagName);
}
