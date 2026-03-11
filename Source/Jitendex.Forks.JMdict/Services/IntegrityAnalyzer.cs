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

namespace Jitendex.Forks.JMdict.Services;

internal partial class IntegrityAnalyzer
(
    ILogger<IntegrityAnalyzer> logger,
    JMdictForkContext context
)
{
    public void Analyze()
    {
        CheckForUkTagOnEntriesWithoutKanjiForms();
        CheckForRightSingleQuotes();
        CheckForZeroWidthSpaces();
        CheckForUnpairedPriorityTags();
        CheckForPriorityTagsOnRareForms();

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

    private void CheckForRightSingleQuotes()
    {
        const char rightSingleQuote = '\u2019';
        const char apostrophe = '\u0027';

        var glosses = context.Glosses
            .Where(static gloss => gloss.Text.Contains(rightSingleQuote));

        foreach (var gloss in glosses)
        {
            LogRightSingleQuote(gloss.EntryId, gloss.Text);
            gloss.Text = gloss.Text.Replace(rightSingleQuote, apostrophe);
        }
    }

    private void CheckForZeroWidthSpaces()
    {
        const string zeroWidthSpace = "\u200B";

        var glosses = context.Glosses
            .Where(static gloss => gloss.Text.Contains(zeroWidthSpace));

        foreach (var gloss in glosses)
        {
            LogZeroWidthSpace(gloss.EntryId, gloss.Text);
            gloss.Text = gloss.Text.Replace(zeroWidthSpace, string.Empty);
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

    private void CheckForPriorityTagsOnRareForms()
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
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {Id} contains a [uk] misc tag, but no kanji forms.")]
    partial void LogUkTagOnEntryWithoutKanjiForms(int id);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {Id} contains a gloss with the wrong apostrophe: `{Text}`")]
    partial void LogRightSingleQuote(int id, string text);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` contains a zero-width space: `{Text}`")]
    partial void LogZeroWidthSpace(int id, string text);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` kanji form #{Number} contains an unpaired priority tag `{TagName}`")]
    partial void LogUnpairedPriorityTag(int id, int number, string tagName);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` contains a priority tag `{TagName}` on a rare form")]
    partial void LogPriorityTagOnRareForm(int id, string tagName);
}
