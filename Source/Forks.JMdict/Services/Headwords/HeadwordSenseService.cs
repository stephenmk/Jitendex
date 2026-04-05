/*
Copyright (c) 2026 Stephen Kraus
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

using Microsoft.EntityFrameworkCore;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.RowModels;
using Jitendex.Forks.JMdict.Tables.Headwords;

namespace Jitendex.Forks.JMdict.Services.Headwords;

internal partial class HeadwordSenseService
(
    JMdictForkContext context,
    HeadwordSenseTable senseTable,
    HeadwordRuleTable ruleTable
)
{
    public void Write()
    {
        var senseRows = new List<HeadwordSenseRow>();
        var ruleRows = new List<HeadwordRuleRow>();

        var headwords = context.Headwords
            .AsSplitQuery()
            .OrderBy(static h => h.EntryId)
            .ThenBy(static h => h.Order)
            .Select(static h => new
            {
                h.EntryId,
                h.Order,
                h.Surface,
                h.Reading,
                h.ReadingOrder,
                h.KanjiFormOrder,
                IsRedirect = h.Redirect != null,
                Senses = h.Entry.Senses
                    .OrderBy(static s => s.Order)
                    .Select(static s => new
                    {
                        s.Order,
                        ReadingRestrictionsAreForSurfaceForms = s.ReadingRestrictions
                            .Where(static r => r.Link != null)
                            .All(static r => !r.Link!.Reading.Bridges.Any()),
                        ReadingRestrictions = s.ReadingRestrictions
                            .Select(static r => r.Order),
                        KanjiFormRestrictions = s.KanjiFormRestrictions
                            .Select(static r => r.Order),
                        PartsOfSpeech = s.PartsOfSpeech
                            .Select(static p => p.TagName)
                    })
            });

        var partsOfSpeech = new HashSet<string>();
        var orders = new List<int>();
        var rules = new HashSet<string>();

        foreach (var headword in headwords)
        {
            partsOfSpeech.Clear();
            orders.Clear();
            rules.Clear();

            foreach (var sense in headword.Senses)
            {
                if (headword.IsRedirect)
                {
                    partsOfSpeech.UnionWith(sense.PartsOfSpeech);
                    continue;
                }

                var readingRestrs = sense.ReadingRestrictions.ToArray();
                var kanjiRestrs = sense.KanjiFormRestrictions.ToArray();
                var readingMatch = headword.ReadingOrder.HasValue && readingRestrs.Contains(headword.ReadingOrder.Value);
                var kanjiMatch = headword.KanjiFormOrder.HasValue && kanjiRestrs.Contains(headword.KanjiFormOrder.Value);
                bool isMatch;

                if (readingRestrs.Length > 0 && kanjiRestrs.Length > 0)
                {
                    if (sense.ReadingRestrictionsAreForSurfaceForms)
                    {
                        isMatch = readingMatch || kanjiMatch;
                    }
                    else
                    {
                        isMatch = readingMatch && kanjiMatch;
                    }
                }
                else if (readingRestrs.Length > 0)
                {
                    isMatch = readingMatch;
                }
                else if (kanjiRestrs.Length > 0)
                {
                    isMatch = kanjiMatch;
                }
                else
                {
                    isMatch = true;
                }

                if (isMatch)
                {
                    orders.Add(sense.Order);
                    partsOfSpeech.UnionWith(sense.PartsOfSpeech);
                }
            }

            int i = 0;
            foreach (var order in orders)
            {
                senseRows.Add(new(headword.EntryId, headword.Order, i++, order));
            }

            foreach (var pos in partsOfSpeech)
            {
                if (GetRule(pos, headword.Surface, headword.Reading) is string rule)
                {
                    rules.Add(rule);
                }
            }

            foreach (var rule in rules)
            {
                ruleRows.Add(new(headword.EntryId, headword.Order, rule));
            }
        }

        senseTable.InsertItems(context, senseRows);
        ruleTable.InsertItems(context, ruleRows);
    }

    private static string? GetRule(string partOfSpeech, string surfaceForm, string? reading)
    {
        if (partOfSpeech.StartsWith("v5", StringComparison.Ordinal))
        {
            return "v5";
        }
        if (partOfSpeech.StartsWith("v1", StringComparison.Ordinal))
        {
            return "v1";
        }
        if (partOfSpeech.StartsWith("vs-", StringComparison.Ordinal))
        {
            return "vs";
        }
        if (partOfSpeech is "adj-i" or "vk" or "vz")
        {
            return partOfSpeech;
        }
        if (partOfSpeech is "adj-ix")
        {
            const string yoi = "よい";
            if (surfaceForm.EndsWith(yoi, StringComparison.Ordinal))
            {
                return "adj-i";
            }
            if (reading is not null && reading.EndsWith(yoi, StringComparison.Ordinal))
            {
                return "adj-i";
            }
        }
        return null;
    }
}
