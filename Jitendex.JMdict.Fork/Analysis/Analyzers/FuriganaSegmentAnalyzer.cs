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
using Jitendex.JMdict.Fork.Analysis.Services;
using Jitendex.JMdict.Fork.Analysis.Tables;

namespace Jitendex.JMdict.Fork.Analysis.Analyzers;

internal partial class FuriganaSegmentAnalyzer
(
    ILogger<FuriganaSegmentAnalyzer> logger,
    JMdictForkContext context,
    FuriganaSolverService furiganaServiceProvider,
    FuriganaSegmentTable table
)
{
    public void Analyze()
    {
        var furiganaService = furiganaServiceProvider.LoadFuriganaService();

        var entries = context.KanjiFormBridges
            .Select(static b => new
            {
                Id = b.EntryId,
                b.ReadingOrder,
                b.KanjiFormOrder,
                ReadingText = b.Reading.Text,
                KanjiFormText = b.KanjiForm.Text,
            })
            .ToList();

        var segments = new List<FuriganaSegmentRow>(entries.Count);

        foreach (var entry in entries)
        {
            var solution = furiganaService.Solve(entry.KanjiFormText, entry.ReadingText);
            if (solution is null)
            {
                LogUnsolvedFurigana(entry.Id, entry.ReadingText, entry.KanjiFormText);
                continue;
            }
            for (int i = 0; i < solution.Parts.Length; i++)
            {
                var part = solution.Parts[i];
                segments.Add(new
                (
                    entry.Id,
                    entry.ReadingOrder,
                    entry.KanjiFormOrder,
                    i,
                    part.BaseText,
                    part.RubyText,
                    null
                ));
            }
        }

        table.InsertItems(context, segments);
    }

    [LoggerMessage(LogLevel.Warning,
    "Unable to solve furigana for {KanjiForm}【{Reading}】 from Entry ID {EntryId}")]
    protected partial void LogUnsolvedFurigana(int entryId, string reading, string kanjiForm);
}
