// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 01-UkTagOnEntriesWithoutKanjiForms.cs, is part of Jitendex.
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

internal partial class CheckForUkTagOnEntriesWithoutKanjiForms
(
    ILogger<CheckForUkTagOnEntriesWithoutKanjiForms> logger,
    JMdictForkContext context
)
{
    public void Run()
    {
        var miscs = context.Miscs
            .Where(static misc => misc.TagName == "uk")
            .Where(static misc => misc.Sense.Entry.Readings.All(static r => !r.Bridges.Any()));

        foreach (var misc in miscs)
        {
            LogUkTagOnEntryWithoutKanjiForms(misc.EntryId);
            context.Remove(misc);
        }
        context.SaveChanges();
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID {Id} contains a [uk] misc tag, but no kanji forms.")]
    partial void LogUkTagOnEntryWithoutKanjiForms(int id);
}
