// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, TrademarkService.cs, is part of Jitendex.
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

using System.Collections.Frozen;
using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.JMdict.Services.Patching;

internal partial class TrademarkService
(
    ILogger<TrademarkService> logger,
    JMdictForkContext forkContext,
    HomeContext homeContext
)
{
    public void Write()
    {
        var textToReplacement = homeContext.TrademarkGlosses
            .Select(static g => new { Key = g.OriginalText, Value = g.ReplacementText })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        var trademarkGlosses = forkContext.GlossTypes
            .Where(static t => t.TagName == "tm")
            .Select(static t => t.Gloss);

        foreach (var gloss in trademarkGlosses)
        {
            if (textToReplacement.TryGetValue(gloss.Text, out var replacement))
            {
                gloss.Text = replacement;
            }
            else
            {
                LogMissingReplacement(gloss.Text, gloss.EntryId);
            }
        }

        forkContext.SaveChanges();
    }

    [LoggerMessage(LogLevel.Warning,
    "No replacement trademark text found for gloss `{Gloss}` in entry `{EntryId}`")]
    partial void LogMissingReplacement(string gloss, int entryId);
}
