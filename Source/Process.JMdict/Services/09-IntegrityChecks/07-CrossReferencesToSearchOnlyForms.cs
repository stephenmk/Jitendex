// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 07-CrossReferencesToSearchOnlyForms.cs, is part of Jitendex.
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

internal partial class CheckForCrossReferencesToSearchOnlyForms
(
    ILogger<CheckForCrossReferencesToSearchOnlyForms> logger,
    JMdictForkContext context
)
{
    public void Run()
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
    "Entry ID `{Id}` sense number {SenseNumber} contains a reference to search-only form `{Text}`")]
    partial void LogReferenceToSearchOnlyForm(int id, int senseNumber, string text);
}
