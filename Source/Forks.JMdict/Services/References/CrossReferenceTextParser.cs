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

using Microsoft.Extensions.Logging;
using Jitendex.Forks.JMdict.RowModels;

namespace Jitendex.Forks.JMdict.Services.References;

internal partial class CrossReferenceTextParser(ILogger<CrossReferenceTextParser> logger)
{
    public ParsedReferenceText? Parse(string referenceText)
    {
        const char separator = '・';
        var split = referenceText.Split(separator);
        ParsedReferenceText parsed;
        switch (split.Length)
        {
            case 1:
                parsed = new(split[0], null, 1);
                break;
            case 2:
                if (int.TryParse(split[1], out int x))
                {
                    parsed = new(split[0], null, x);
                }
                else
                {
                    parsed = new(split[0], split[1], 1);
                }
                break;
            case 3:
                if (int.TryParse(split[2], out int y))
                {
                    parsed = new(split[0], split[1], y);
                }
                else
                {
                    LogNonIntegerSenseOrder(referenceText, split[2]);
                    return null;
                }
                break;
            default:
                LogTooManySeparators(referenceText, separator);
                return null;
        }
        return parsed;
    }

    [LoggerMessage(LogLevel.Error,
    "Third value `{ThirdValue}` in reference text `{Text}` must be an integer")]
    partial void LogNonIntegerSenseOrder(string text, string thirdValue);

    [LoggerMessage(LogLevel.Error,
    "Too many separator characters `{Separator}` in reference text `{Text}`")]
    partial void LogTooManySeparators(string text, char separator);
}
