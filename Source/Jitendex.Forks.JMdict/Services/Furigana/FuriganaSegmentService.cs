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

using System.Text;
using Microsoft.Extensions.Logging;
using Jitendex.Furigana;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.Models;
using Jitendex.Forks.JMdict.Tables.Furigana;

namespace Jitendex.Forks.JMdict.Services.Furigana;

internal partial class FuriganaSegmentService
(
    ILogger<FuriganaSegmentService> logger,
    JMdictForkContext context,
    FuriganaSegmentTable table
)
{
    public void Write()
    {
        var furiganaService = LoadFuriganaService();

        var entries = context.ReadingKanjiFormBridges
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
                    part.RubyText
                ));
            }
        }

        table.InsertItems(context, segments);
    }

    private IFuriganaService LoadFuriganaService()
    {
        var service = FuriganaServiceProvider.GetFuriganaService();

        var characters = context.CharacterReadings
            .Select(static g => new
            {
                Rune = new Rune(g.CharacterValue),
                DerivedReadings = g.DerivedReadings
                    .Select(static x => new { x.Text, x.IsPrefix, x.IsSuffix })
            });

        foreach (var character in characters)
        {
            foreach (var reading in character.DerivedReadings)
            {
                service.AddCharacterReading(character.Rune, reading.Text, reading.IsPrefix, reading.IsSuffix);
            }
        }

        var compoundReadings = context.CompoundReadings
            .Select(static c => new
            {
                c.CompoundText,
                c.Text,
            });

        foreach (var reading in compoundReadings)
        {
            service.AddCompoundReading(reading.CompoundText, reading.Text);
        }

        return service;
    }

    [LoggerMessage(LogLevel.Warning,
    "Unable to solve furigana for {KanjiForm}【{Reading}】 from Entry ID {EntryId}")]
    protected partial void LogUnsolvedFurigana(int entryId, string reading, string kanjiForm);
}
