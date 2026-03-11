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
using Jitendex.Data.JMdict;

namespace Jitendex.Forks.JMdict.Analyzers;

internal partial class IntegrityAnalyzer
(
    ILogger<IntegrityAnalyzer> logger,
    JMdictForkContext context
)
{
    public void Analyze()
    {
        CheckForUkTagOnEntriesWithoutKanjiForms();
        CheckForRightSingleQuotationMarks();
        CheckForZeroWidthSpaces();
        CheckForUnpairedPriorityTags();

        context.SaveChanges();
    }

    private void CheckForUkTagOnEntriesWithoutKanjiForms()
    {
        var miscs = context.Miscs
            .Where(static misc => misc.TagName == "uk")
            .Where(static misc => misc.Sense.Entry.KanjiForms.Count == 0);

        foreach (var misc in miscs)
        {
            LogUkTagOnEntryWithoutKanjiForms(misc.EntryId);
            context.Remove(misc);
        }
    }

    private void CheckForRightSingleQuotationMarks()
    {
        var glosses = context.Glosses
            .Where(static gloss => gloss.Text.Contains('\u2019'));

        foreach (var gloss in glosses)
        {
            LogRightSingleQuotationMark(gloss.EntryId, gloss.Text);
            gloss.Text = gloss.Text.Replace('\u2019', '\u0027');
        }
    }

    private void CheckForZeroWidthSpaces()
    {
        var glosses = context.Glosses
            .Where(static gloss => gloss.Text.Contains('\u200B'));

        foreach (var gloss in glosses)
        {
            LogZeroWidthSpace(gloss.EntryId, gloss.Text);
            gloss.Text = gloss.Text.Replace("\u200B", string.Empty);
        }
    }

    private void CheckForUnpairedPriorityTags()
    {
        var priorities = context.KanjiFormPriorities
            .Where(static p => !p.KanjiForm.Bridges.Any(b => b.Reading.Priorities.Any(i => i.TagName == p.TagName)));

        foreach (var priority in priorities)
        {
            LogUnpairedPriorityTag(priority.EntryId, priority.KanjiFormOrder + 1, priority.TagName);
            context.Remove(priority);
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a [uk] misc tag, but no kanji forms.")]
    partial void LogUkTagOnEntryWithoutKanjiForms(int entryId);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {EntryId} contains a gloss with the wrong apostrophe: `{Text}`")]
    partial void LogRightSingleQuotationMark(int entryId, string text);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{entryId}` contains a zero-width space: `{Text}`")]
    partial void LogZeroWidthSpace(int entryId, string text);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` kanji form #{Number} contains an unpaired priority tag `{TagName}`")]
    partial void LogUnpairedPriorityTag(int id, int number, string tagName);
}
