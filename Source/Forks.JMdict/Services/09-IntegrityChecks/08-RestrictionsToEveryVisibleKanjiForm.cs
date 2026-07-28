// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 08-RestrictionsToEveryVisibleKanjiForm.cs, is part of Jitendex.
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

namespace Jitendex.Forks.JMdict.Services.IntegrityChecks;

internal partial class CheckForRestrictionsToEveryVisibleKanjiForm
(
    ILogger<CheckForRestrictionsToEveryVisibleKanjiForm> logger,
    JMdictForkContext context
)
{
    public void Run()
    {
        var references = context.Readings
            .Where(static r => r.Restrictions.Any())
            .Where(static r => r.Restrictions.Count == r.Entry.KanjiForms.Count(static k => k.Bridges.Any()))
            .Select(static r => new { r.EntryId, r.Order, r.Text });

        foreach (var r in references)
        {
            LogRedundantRestriction(r.EntryId, r.Order + 1, r.Text);
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` reading number #{Number} ({Text}) contains a restriction to every visible kanji form")]
    partial void LogRedundantRestriction(int id, int number, string text);
}
