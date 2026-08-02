// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 05-PriorityTagsOnRareForms.cs, is part of Jitendex.
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

internal partial class CheckForPriorityTagsOnRareForms
(
    ILogger<CheckForPriorityTagsOnRareForms> logger,
    JMdictForkContext context
)
{
    public void Run()
    {
        var kanjiPriorities = context.KanjiFormPriorities
            .Where(static p => p.KanjiForm.Infos.Any(static i => i.TagName == "rK" || i.TagName == "sK"));

        var readingPriorities = context.ReadingPriorities
            .Where(static p => p.Reading.Infos.Any(static i => i.TagName == "rk" || i.TagName == "sk"));

        foreach (var priority in kanjiPriorities)
        {
            LogPriorityTagOnRareForm(priority.EntryId, priority.TagName);
            context.Remove(priority);
        }

        foreach (var priority in readingPriorities)
        {
            LogPriorityTagOnRareForm(priority.EntryId, priority.TagName);
            context.Remove(priority);
        }

        context.SaveChanges();
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` contains a priority tag `{TagName}` on a rare form")]
    partial void LogPriorityTagOnRareForm(int id, string tagName);

}
