// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, IntegrityService.cs, is part of Jitendex.
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

namespace Jitendex.Forks.JMdict.Services;

internal partial class IntegrityService
(
    ILogger<IntegrityService> logger,
    JMdictForkContext context
)
{
    public void Write()
    {
        CheckForUkTagOnEntriesWithoutKanjiForms();
        CheckForRightSingleQuotes();
        CheckForZeroWidthSpaces();
        CheckForUnpairedPriorityTags();
        CheckForPriorityTagsOnRareForms();
        CheckForTransitivityTagOnSensesGlossedAsAdverbs();
        CheckForCrossReferencesToSearchOnlyForms();

        context.SaveChanges();
    }

    private void CheckForUkTagOnEntriesWithoutKanjiForms()
    {
        var miscs = context.Miscs
            .Where(static misc => misc.TagName == "uk")
            .Where(static misc => misc.Sense.Entry.Readings.All(static r => !r.Bridges.Any()));

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
        const char zeroWidthSpace = '\u200B';

        var glosses = context.Glosses
            .Where(static gloss => gloss.Text.Contains(zeroWidthSpace));

        foreach (var gloss in glosses)
        {
            LogZeroWidthSpace(gloss.EntryId, gloss.Text);
            gloss.Text = gloss.Text.Replace(zeroWidthSpace.ToString(), string.Empty);
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

    private void CheckForTransitivityTagOnSensesGlossedAsAdverbs()
    {
        // Query all senses containing "adv", "vs", and either "vi" or "vt" tags,
        // and where the "adv" tag is before the "vs" tag.
        var senses = context.Senses
            .Where(static s => s.PartsOfSpeech.Any(static p => p.TagName == "adv"))
            .Where(static s => s.PartsOfSpeech.Any(static p => p.TagName == "vs"))
            .Where(static s => s.PartsOfSpeech.Any(static p => new[] { "vi", "vt" }.Contains(p.TagName)))
            .Where(static s => s.PartsOfSpeech.First(static p => p.TagName == "adv").Order <
                               s.PartsOfSpeech.First(static p => p.TagName == "vs").Order)
            .Select(static s => new { s.EntryId, s.Order });

        foreach (var sense in senses)
        {
            LogTransitivityTagOnAdverb(sense.EntryId, sense.Order + 1);
        }
    }

    private void CheckForCrossReferencesToSearchOnlyForms()
    {
        var kanjiReferences = context.KanjiFormReferences
            .Where(static r => r.KanjiForm.Infos.Any(static i => i.TagName == "sK"))
            .Select(static r => new { r.EntryId, r.SenseOrder, r.KanjiForm.Text });

        foreach (var r in kanjiReferences)
        {
            LogReferenceToSearchOnlyForm(r.EntryId, r.SenseOrder + 1, r.Text);
        }

        var readingReferences = context.ReadingReferences
            .Where(static r => r.Reading.Infos.Any(static i => i.TagName == "sk"))
            .Select(static r => new { r.EntryId, r.SenseOrder, r.Reading.Text });

        foreach (var r in readingReferences)
        {
            LogReferenceToSearchOnlyForm(r.EntryId, r.SenseOrder + 1, r.Text);
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

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` sense number {SenseNumber} is glossed as an adverb and contains a transitivity tag")]
    partial void LogTransitivityTagOnAdverb(int id, int senseNumber);

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` sense number {SenseNumber} contains a reference to search-only form `{Text}`")]
    partial void LogReferenceToSearchOnlyForm(int id, int senseNumber, string text);
}
