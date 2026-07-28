// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 04-UnpairedPriorityTags.cs, is part of Jitendex.
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

internal partial class CheckForUnpairedPriorityTags
(
    ILogger<CheckForUnpairedPriorityTags> logger,
    JMdictForkContext context
)
{
    public void Run()
    {
        var priorities = context.KanjiFormPriorities
            .Where(static p => !p.KanjiForm.Bridges.Any(b => b.Reading.Priorities.Any(i => i.TagName == p.TagName)));

        foreach (var priority in priorities)
        {
            LogUnpairedPriorityTag(priority.EntryId, priority.KanjiFormOrder + 1, priority.TagName);
            context.Remove(priority);
        }
        context.SaveChanges();
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` kanji form #{Number} contains an unpaired priority tag `{TagName}`")]
    partial void LogUnpairedPriorityTag(int id, int number, string tagName);

}
