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

        context.SaveChanges();
    }

    private void CheckForUkTagOnEntriesWithoutKanjiForms()
    {
        var miscs = context.Miscs
            .Where(static misc => misc.TagName == "uk")
            .Where(static misc => misc.Sense.Entry.KanjiForms.Count == 0)
            .Select(static misc => misc);

        foreach (var misc in miscs)
        {
            LogUkTagOnEntryWithoutKanjiForms(misc.EntryId);
            context.Remove(misc);
        }
    }

    private void CheckForRightSingleQuotationMarks()
    {
        var glosses = context.Glosses
            .Where(gloss => gloss.Text.Contains('\u2019'))
            .Select(static gloss => gloss);

        foreach (var gloss in glosses)
        {
            LogRightSingleQuotationMark(gloss.EntryId, gloss.Text);
            gloss.Text = gloss.Text.Replace('\u2019', '\u0027');
        }
    }

    private void CheckForZeroWidthSpaces()
    {
        var glosses = context.Glosses
            .Where(gloss => gloss.Text.Contains('\u200B'))
            .Select(static gloss => gloss);

        foreach (var gloss in glosses)
        {
            LogZeroWidthSpace(gloss.EntryId, gloss.Text);
            gloss.Text = gloss.Text.Replace("\u200B", string.Empty);
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
}
