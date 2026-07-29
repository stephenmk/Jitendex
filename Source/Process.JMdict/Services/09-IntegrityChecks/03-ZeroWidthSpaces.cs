// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 03-ZeroWidthSpaces.cs, is part of Jitendex.
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

using Jitendex.Data.JMdict;
using Microsoft.Extensions.Logging;

namespace Jitendex.Process.JMdict.Services.IntegrityChecks;

internal partial class CheckForZeroWidthSpaces
(
    ILogger<CheckForZeroWidthSpaces> logger,
    JMdictForkContext context
)
{
    public void Run()
    {
        const char zeroWidthSpace = '\u200B';

        var glosses = context.Glosses
            .Where(static gloss => gloss.Text.Contains(zeroWidthSpace));

        foreach (var gloss in glosses)
        {
            LogZeroWidthSpace(gloss.EntryId, gloss.Text);
            gloss.Text = gloss.Text.Replace(zeroWidthSpace.ToString(), string.Empty);
        }
        context.SaveChanges();
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` contains a zero-width space: `{Text}`")]
    partial void LogZeroWidthSpace(int id, string text);

}
