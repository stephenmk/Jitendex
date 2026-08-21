// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 09-SearchOnlyFormsAtEnd.cs, is part of Jitendex.
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

internal partial class CheckForSearchOnlyFormsAtEnd
(
    ILogger<CheckForSearchOnlyFormsAtEnd> logger,
    JMdictForkContext context
)
{
    public void Run()
    {
        RunReadings();
        RunKanjiForms();
    }

    private readonly record struct Info(
        int EntryId,
        int TotalFormCount,
        int HiddenFormCount,
        int FirstHiddenFormOrder)
    {
        public bool IsUnordered
            => TotalFormCount - HiddenFormCount != FirstHiddenFormOrder;
    }

    private void RunReadings()
    {
        var infos = context.ReadingInfos
            .Where(static i => i.TagName == "sk")
            .GroupBy(static r => r.EntryId)
            .Select(static g => new Info
            (
                EntryId: g.Key,
                TotalFormCount: g.First().Reading.Entry.Readings.Count,
                HiddenFormCount: g.Count(),
                FirstHiddenFormOrder: g
                    .OrderBy(static r => r.ReadingOrder)
                    .First()
                    .ReadingOrder
            ));

        foreach (var i in infos)
        {
            if (i.IsUnordered)
                LogUnordered(i.EntryId);
        }
    }

    private void RunKanjiForms()
    {
        var infos = context.KanjiFormInfos
            .Where(static i => i.TagName == "sK")
            .GroupBy(static r => r.EntryId)
            .Select(static g => new Info
            (
                EntryId: g.Key,
                TotalFormCount: g.First().KanjiForm.Entry.KanjiForms.Count,
                HiddenFormCount: g.Count(),
                FirstHiddenFormOrder: g
                    .OrderBy(static r => r.KanjiFormOrder)
                    .First()
                    .KanjiFormOrder
            ));

        foreach (var i in infos)
        {
            if (i.IsUnordered)
                LogUnordered(i.EntryId);
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Entry ID `{Id}` contains forms without search-only tags after forms with search-only tags.")]
    partial void LogUnordered(int id);
}
