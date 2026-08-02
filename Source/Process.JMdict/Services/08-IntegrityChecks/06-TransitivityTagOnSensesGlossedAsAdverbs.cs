// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 06-TransitivityTagOnSensesGlossedAsAdverbs.cs, is part of Jitendex.
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

internal partial class CheckForTransitivityTagOnSensesGlossedAsAdverbs
(
    ILogger<CheckForTransitivityTagOnSensesGlossedAsAdverbs> logger,
    JMdictForkContext context
)
{
    public void Run()
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

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` sense number {SenseNumber} is glossed as an adverb and contains a transitivity tag")]
    partial void LogTransitivityTagOnAdverb(int id, int senseNumber);

}
