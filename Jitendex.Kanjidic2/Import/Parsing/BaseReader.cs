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

using System.Text;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Jitendex.Kanjidic2.Import.Parsing;

internal abstract partial class BaseReader
{
    protected readonly ILogger _logger;

    public BaseReader(ILogger logger)
        => _logger = logger;

    protected async Task LogUnexpectedTextNodeAsync(XmlReader xmlReader, int unicodeScalarValue, string tagName)
    {
        var text = await xmlReader.GetValueAsync();
        LogUnexpectedTextNode(new Rune(unicodeScalarValue), tagName, text);
    }

    [LoggerMessage(LogLevel.Warning,
    "`{Character}`: Unexpected XML element node <{TagName}> found in element <{ParentTagName}>")]
    protected partial void LogUnexpectedChildElement(Rune character, string tagName, string parentTagName);

    [LoggerMessage(LogLevel.Warning,
    "`{Character}`: Unexpected XML text node found in element <{TagName}>: `{Text}`")]
    private partial void LogUnexpectedTextNode(Rune character, string tagName, string text);
}
