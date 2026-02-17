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

using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Jitendex.Furigana;
using Jitendex.Kanjidic2;
using Jitendex.JMdict.Import.Analysis.Tables;

namespace Jitendex.JMdict.Import.Analysis.Analyzers;

internal partial class FuriganaSegmentAnalyzer
(
    ILogger<FuriganaSegmentAnalyzer> logger,
    JmdictContext context,
    Kanjidic2Context kanjiContext,
    IFuriganaSolver furiganaSolver
)
{
    private readonly static FuriganaSegmentTable FuriganaSegmentTable = new();

    private void AddCharactersToSolver()
    {
        var characters = kanjiContext.DerivedReadings
            .GroupBy(static r => r.UnicodeScalarValue)
            .Select(static g => new JapaneseCharacter
            (
                Rune: new(g.Key),
                VocabReadings: g.Select(static r => new CharacterReading(r.Text, r.IsPrefix, r.IsSuffix)).ToImmutableArray(),
                NameReadings: new()
            ));
        furiganaSolver.AddCharacters(characters);
    }

    public void Analyze()
    {
        AddCharactersToSolver();

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

        var segments = new List<FuriganaSegmentElement>(entries.Count);

        foreach (var entry in entries)
        {
            var solution = furiganaSolver.SolveVocab(entry.KanjiFormText, entry.ReadingText);
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
                    part.Furigana
                ));
            }
        }

        FuriganaSegmentTable.InsertItems(context, segments);
    }

    [LoggerMessage(LogLevel.Warning,
    "Unable to solve furigana for {KanjiForm}【{Reading}】 from Entry ID {EntryId}")]
    protected partial void LogUnsolvedFurigana(int entryId, string reading, string kanjiForm);
}
