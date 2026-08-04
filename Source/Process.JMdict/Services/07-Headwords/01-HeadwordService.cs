// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 01-HeadwordService.cs, is part of Jitendex.
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

using System.Collections.Frozen;
using System.Text;
using Jitendex.Data.JMdict;
using Jitendex.Process.JMdict.TableRows;
using Jitendex.Process.JMdict.Tables.Headwords;
using Jitendex.JapaneseTextUtils;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Process.JMdict.Services.Headwords;

internal sealed class HeadwordService
(
    JMdictForkContext context,
    HeadwordTable headwordTable,
    HeadwordRedirectTable redirectTable,
    HeadwordTagTable tagTable
)
{
    private static readonly ImmutableArray<string> HighPriorityTagNames = ["spec1", "news1", "ichi1", "gai1"];
    private static readonly ImmutableArray<string> IrregularKanjiTags = ["sK", "iK", "rK", "io", "ik"];
    private static readonly ImmutableArray<string> IrregularReadingTags = ["ik", "sk", "rk", "ok"];

    private static readonly FrozenSet<string> HighPriorityTagNameSet = [.. HighPriorityTagNames];
    private static readonly FrozenSet<string> IrregularInfoTagSet = [.. IrregularKanjiTags, .. IrregularReadingTags];

    public void Write()
    {
        var kanaOnlyHeadwords = GetKanaOnlyHeadwords();
        var headwordRows = new List<HeadwordRow>();
        var redirectRows = new List<HeadwordRedirectRow>();
        var tagRows = new List<HeadwordTagRow>();

        var kanjiToVariants = context.CharacterVariants
            .GroupBy(static v => v.CharacterValue)
            .Select(static g => new
            {
                Key = new Rune(g.Key),
                Value = g.Select(static v => new Rune(v.VariantValue).ToString())
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value.ToArray());

        var entries = context.Entries
            .AsSplitQuery()
            .Select(static e => new
            {
                e.Id,
                Readings = e.Readings
                    .OrderBy(static r => r.Order)
                    .Select(static r => new
                    {
                        r.Order,
                        r.Text,
                        IsSearchOnly = r.Infos.Any(static i => i.TagName == "sk"),
                        Infos = r.Infos.Select(static i => i.TagName),
                        Prios = r.Priorities.Select(static p => p.TagName),
                        KanjiForms = r.Bridges
                            .OrderBy(static b => b.KanjiFormOrder)
                            .Select(static b => new
                            {
                                Order = b.KanjiFormOrder,
                                b.KanjiForm.Text,
                                Infos = b.KanjiForm.Infos.Select(static i => i.TagName),
                                Prios = b.KanjiForm.Priorities.Select(static p => p.TagName),
                                Furigana = b.FuriganaSegments
                                    .OrderBy(static f => f.Order)
                                    .Select(static f => new { f.BaseText, f.RubyText })
                            })
                    }),
                KanjiForms = e.KanjiForms
                    .OrderBy(static k => k.Order)
                    .Select(static k => new
                    {
                        k.Order,
                        k.Text,
                        IsSearchOnly = k.Infos.Any(static i => i.TagName == "sK"),
                        Infos = k.Infos.Select(static i => i.TagName),
                        Prios = k.Priorities.Select(static p => p.TagName),
                    }),
                ContainsSearchOnlyKanji = e.KanjiForms
                    .Any(static k => k.Infos.Any(static i => i.TagName == "sK")),
            });

        foreach (var entry in entries)
        {
            int entryOrder = 0;
            var formToHeadwordOrder = new Dictionary<string, int>();
            var variantToHeadwordOrder = new Dictionary<string, int>();

            var cartProdSolver = new HeadwordCartesianProductSolver()
            {
                KanjiToVariants = kanjiToVariants,
                ValidCombinations = entry.ContainsSearchOnlyKanji
                    ? entry.KanjiForms
                        .Where(static k => k.IsSearchOnly)
                        .Select(static k => k.Text)
                        .ToImmutableArray()
                    : []
            };

            foreach (var reading in entry.Readings)
            {
                if (kanaOnlyHeadwords.Contains((entry.Id, reading.Order)) || !reading.KanjiForms.Any())
                {
                    if (reading.IsSearchOnly)
                    {
                        redirectRows.Add(new(
                            EntryId: entry.Id,
                            HeadwordOrder: entryOrder,
                            RedirectHeadwordOrder: 0
                        ));
                    }
                    tagRows.AddRange(reading.Infos
                        .Where(static t => t != "gikun")
                        .Concat(reading.Prios)
                        .Select(t => new HeadwordTagRow(entry.Id, entryOrder, t)));
                    headwordRows.Add(new(
                        EntryId: entry.Id,
                        Order: entryOrder++,
                        Score: CalculateScore(reading.Infos, reading.Prios),
                        Surface: reading.Text,
                        Reading: null,
                        ReadingOrder: reading.Order,
                        KanjiFormOrder: null
                    ));
                }
                foreach (var kanjiForm in reading.KanjiForms)
                {
                    if (entry.ContainsSearchOnlyKanji)
                    {
                        if (!formToHeadwordOrder.ContainsKey(kanjiForm.Text))
                        {
                            formToHeadwordOrder[kanjiForm.Text] = entryOrder;
                        }
                        var parts = kanjiForm.Furigana
                            .Select(static f => f.RubyText == null
                                ? new string[] { f.BaseText }
                                : [f.BaseText, f.RubyText])
                            .ToArray();
                        var variants = cartProdSolver.SolveCartesianProducts(parts);
                        foreach (var variant in variants)
                        {
                            if (!variantToHeadwordOrder.ContainsKey(variant))
                            {
                                variantToHeadwordOrder[variant] = entryOrder;
                            }
                        }
                    }
                    var infos = reading.Infos.Union(kanjiForm.Infos); // Union excludes duplicates.
                    var prios = reading.Prios.Intersect(kanjiForm.Prios);
                    var readingParts = kanjiForm.Furigana
                        .Select(static f => f.RubyText ?? f.BaseText);
                    var normalizedReading = string.Concat(readingParts);
                    tagRows.AddRange(infos.Concat(prios)
                        .Select(name => new HeadwordTagRow(entry.Id, entryOrder, name)));
                    headwordRows.Add(new(
                        EntryId: entry.Id,
                        Order: entryOrder++,
                        Score: CalculateScore(infos, prios),
                        Surface: kanjiForm.Text,
                        Reading: normalizedReading,
                        ReadingOrder: reading.Order,
                        KanjiFormOrder: kanjiForm.Order
                    ));
                }
            }

            foreach (var (key, value) in variantToHeadwordOrder)
            {
                if (!formToHeadwordOrder.ContainsKey(key))
                {
                    formToHeadwordOrder[key] = value;
                }
            }

            foreach (var kanjiForm in entry.KanjiForms)
            {
                if (kanjiForm.IsSearchOnly)
                {
                    var redirectOrder = formToHeadwordOrder.TryGetValue(kanjiForm.Text, out var order)
                        ? order
                        : 0;
                    redirectRows.Add(new(
                        EntryId: entry.Id,
                        HeadwordOrder: entryOrder,
                        RedirectHeadwordOrder: redirectOrder
                    ));
                }
                tagRows.AddRange(kanjiForm.Infos.Concat(kanjiForm.Prios)
                    .Select(name => new HeadwordTagRow(entry.Id, entryOrder, name)));
                headwordRows.Add(new(
                    EntryId: entry.Id,
                    Order: entryOrder++,
                    Score: CalculateScore(kanjiForm.Infos, kanjiForm.Prios),
                    Surface: kanjiForm.Text,
                    Reading: null,
                    ReadingOrder: null,
                    KanjiFormOrder: kanjiForm.Order
                ));
            }
        }

        headwordTable.InsertItems(context, headwordRows);
        redirectTable.InsertItems(context, redirectRows);
        tagTable.InsertItems(context, tagRows);
    }

    private int CalculateScore(IEnumerable<string> infoTags, IEnumerable<string> prioTags)
    {
        foreach (var prioTag in prioTags)
        {
            if (HighPriorityTagNameSet.Contains(prioTag))
            {
                return 1;
            }
        }
        foreach (var infoTag in infoTags)
        {
            if (IrregularInfoTagSet.Contains(infoTag))
            {
                return -1;
            }
        }
        return 0;
    }

    private HashSet<(int, int)> GetKanaOnlyHeadwords()
    {
        var set = new HashSet<(int, int)>();

        // Add readings which have only rare/irregular/search-only kanji forms.
        // Don't add the reading if it already exists as a no-kanji form.
        var entries = context.Entries
            .AsSplitQuery()
            .Where(static e => e.KanjiForms.Any())
            .Where(static e => e.KanjiForms
                .All(static k => k.Infos
                    .Any(static i => IrregularKanjiTags.Contains(i.TagName))))
            .Select(static e => new
            {
                e.Id,
                Readings = e.Readings
                    .Where(static r => !r.NoKanji)
                    .Select(static r => new { r.Order, r.Text }),
                NoKanjiReadings = e.Readings
                    .Where(static r => r.NoKanji)
                    .Select(static r => r.Text)
            });

        foreach (var entry in entries)
        {
            var normalizedNoKanjiReadings = entry.NoKanjiReadings
                .Select(static r => r.KatakanaToHiragana())
                .ToHashSet();
            foreach (var reading in entry.Readings)
            {
                var normalizedReading = reading.Text.KatakanaToHiragana();
                if (!normalizedNoKanjiReadings.Contains(normalizedReading))
                {
                    set.Add((entry.Id, reading.Order));
                }
            }
        }

        // Add readings which have a high-priority tag without
        // a corresponding kanji form with the same tag.
        var readings = context.Readings
            .AsSplitQuery()
            .Where(static r => r.Bridges.Any())
            .Where(static r => r.Infos
                .Any(static i => HighPriorityTagNames.Contains(i.TagName)))
            .Select(static r => new
            {
                r.EntryId,
                r.Order,
                Infos = r.Infos
                    .Where(static i => HighPriorityTagNames.Contains(i.TagName))
                    .Select(static i => i.TagName),
                KanjiInfos = r.Bridges
                    .SelectMany(static b => b.KanjiForm.Infos.Select(static i => i.TagName)),
            });

        foreach (var reading in readings)
        {
            var kanjiInfos = reading.KanjiInfos.ToHashSet();
            var match = false;
            foreach (var info in reading.Infos)
            {
                if (kanjiInfos.Contains(info))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                set.Add((reading.EntryId, reading.Order));
            }
        }

        return set;
    }
}
