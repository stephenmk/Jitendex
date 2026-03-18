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
using static Jitendex.Data.JMdict.ForkEntities.Kanwa.DerivedCharacterReadingTypeId;

namespace Jitendex.Forks.JMdict.Services.Furigana;

internal partial class FuriganaSegmentService
(
    ILogger<FuriganaSegmentService> logger,
    JMdictForkContext context,
    FuriganaSegmentTable segmentTable,
    CharacterReadingLinkTable characterTable,
    CompoundReadingLinkTable compoundTable
)
{
    private record ReadingKey(int Id, string Text);
    private sealed record CompoundReadingKey(int Id, string Text) : ReadingKey(Id, Text);

    public void Write()
    {
        var (furiganaService, idToReadingKey) = LoadFuriganaService();

        var entries = context.ReadingKanjiFormBridges
            .Select(static b => new
            {
                Id = b.EntryId,
                b.ReadingOrder,
                b.KanjiFormOrder,
                ReadingText = b.Reading.Text,
                KanjiFormText = b.KanjiForm.Text,
            });

        var chineseEntryIds = LoadLanguageEntryIds("chi");
        var koreanEntryIds = LoadLanguageEntryIds("kor");

        var segments = new List<FuriganaSegmentRow>(700_000);
        var characterLinks = new List<CharacterReadingLinkRow>(700_000);
        var compoundLinks = new List<CompoundReadingLinkRow>(700_000);

        var troubleReadings = new Dictionary<(string, string?), int>();

        foreach (var entry in entries)
        {
            var solution
                = chineseEntryIds.Contains(entry.Id)
                ? furiganaService.SolveChineseLoanword(entry.KanjiFormText, entry.ReadingText)

                : koreanEntryIds.Contains(entry.Id)
                ? furiganaService.SolveKoreanLoanword(entry.KanjiFormText, entry.ReadingText)

                : furiganaService.Solve(entry.KanjiFormText, entry.ReadingText);

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
                if (part.ReadingIds is [])
                {
                    continue;
                }
                var key = idToReadingKey[part.ReadingIds.First()];
                if (part.ReadingIds.Length > 1)
                {
                    // LogMultipleReadings(part.BaseText, part.RubyText, entry.KanjiFormText);
                    if (troubleReadings.TryGetValue((part.BaseText, part.RubyText), out var count))
                    {
                        troubleReadings[(part.BaseText, part.RubyText)] = count + 1;
                    }
                    else
                    {
                        troubleReadings[(part.BaseText, part.RubyText)] = 1;
                    }
                }
                if (key is CompoundReadingKey)
                {
                    compoundLinks.Add(new
                    (
                        entry.Id,
                        entry.ReadingOrder,
                        entry.KanjiFormOrder,
                        i,
                        key.Id,
                        key.Text
                    ));
                }
                else
                {
                    characterLinks.Add(new
                    (
                        entry.Id,
                        entry.ReadingOrder,
                        entry.KanjiFormOrder,
                        i,
                        key.Id,
                        key.Text
                    ));
                }
            }
        }

        foreach(var x in troubleReadings)
        {
            Console.Error.WriteLine($"{x.Key.Item1}\t{x.Key.Item2}\t{x.Value}");
        }
        Console.Error.WriteLine(troubleReadings.Count);

        segmentTable.InsertItems(context, segments);
        characterTable.InsertItems(context, characterLinks);
        compoundTable.InsertItems(context, compoundLinks);
    }

    private (IFuriganaService, Dictionary<int, ReadingKey>) LoadFuriganaService()
    {
        var service = FuriganaServiceProvider.GetFuriganaService();
        var idToKey = new Dictionary<int, ReadingKey>();

        var characters = context.CharacterReadings
            .Select(static g => new
            {
                Rune = new Rune(g.CharacterValue),
                DerivedReadings = g.DerivedReadings
                    .Select(static x => new { x.ReadingId, x.Text, x.IsPrefix, x.IsSuffix, x.TypeId })
            });

        foreach (var character in characters)
        {
            foreach (var r in character.DerivedReadings)
            {
                var id = r.TypeId switch
                {
                    Chinese => service.AddHanziReading(character.Rune, r.Text, r.IsPrefix, r.IsSuffix),
                    Korean => service.AddHanjaReading(character.Rune, r.Text, r.IsPrefix, r.IsSuffix),
                    _ => service.AddCharacterReading(character.Rune, r.Text, r.IsPrefix, r.IsSuffix),
                };
                idToKey[id] = new ReadingKey(r.ReadingId, r.Text);
            }
        }

        var compounds = context.Compounds
            .Select(static c => new
            {
                c.Id,
                c.Text,
                Readings = c.Readings.Select(static r => r.Text),
            });

        foreach (var compound in compounds)
        {
            foreach (var reading in compound.Readings)
            {
                var id = service.AddCompoundReading(compound.Text, reading);
                idToKey[id] = new CompoundReadingKey(compound.Id, reading);
            }
        }

        return (service, idToKey);
    }

    private HashSet<int> LoadLanguageEntryIds(string languageCode)
        => context.LanguageSources
            .Where(l => l.LanguageCode == languageCode)
            .Select(static l => l.EntryId)
            .ToHashSet();

    [LoggerMessage(LogLevel.Warning,
    "Unable to solve furigana for {KanjiForm}【{Reading}】 from Entry ID {EntryId}")]
    protected partial void LogUnsolvedFurigana(int entryId, string reading, string kanjiForm);

    [LoggerMessage(LogLevel.Information, "{Rune}: {Reading} in {BaseText}【{RubyText}】")]
    protected partial void LogNewReading(Rune rune, string reading, string baseText, string rubyText);

    [LoggerMessage(LogLevel.Information,
    "Segment {BaseText}【{RubyText}】 in `{Text}` corresponds to multiple readings")]
    protected partial void LogMultipleReadings(string baseText, string? rubyText, string text);
}
