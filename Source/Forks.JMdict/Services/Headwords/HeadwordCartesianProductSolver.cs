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

using System.Collections.Frozen;
using System.Text;
using Jitendex.JapaneseTextUtils;

namespace Jitendex.Forks.JMdict.Services.Headwords;

internal class HeadwordCartesianProductSolver
{
    public required FrozenDictionary<Rune, string[]> KanjiToVariants { get; init; }
    public required ImmutableArray<string> ValidCombinations { get; init; }

    public ReadOnlySpan<string> SolveCartesianProducts(string[][] parts)
    {
        var sets = new HashSet<string>[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            var set = new HashSet<string>();
            for (int j = 0; j < parts[i].Length; j++)
            {
                var text = parts[i][j];
                var variants = GetVariants(text);
                set.UnionWith(variants);
            }
            sets[i] = set;
        }

        var product = ComputeCartesianProduct(sets, SuffixValidator);
        var validCombos = new string[product.Count];
        int k = 0;

        foreach (var combo in product)
        {
            foreach (var validCombo in ValidCombinations)
            {
                if (validCombo.Equals(combo, StringComparison.Ordinal))
                {
                    validCombos[k++] = combo;
                    break;
                }
            }
        }

        return validCombos.AsSpan(..k);
    }

    private HashSet<string> GetVariants(string text)
    {
        var runes = text.EnumerateRunes().ToArray();
        var sets = new HashSet<string>[runes.Length];
        for (int i = 0; i < runes.Length; i++)
        {
            var rune = runes[i];
            var set = new HashSet<string>
            {
                runes[i].ToString()
            };
            if (rune.IsHiragana())
            {
                set.Add(rune.HiraganaToKatakana().ToString());
                set.Add(string.Empty); // Particles and okurigana are sometimes omitted.
            }
            else if (rune.IsKatakana())
            {
                set.Add(rune.KatakanaToHiragana().ToString());
            }
            else if (KanjiToVariants.TryGetValue(rune, out var variants))
            {
                set.UnionWith(variants);
            }
            sets[i] = set;
        }
        return ComputeCartesianProduct(sets, SubstringValidator);
    }

    private HashSet<string> ComputeCartesianProduct
    (
        ReadOnlySpan<HashSet<string>> sets,
        Func<ReadOnlySpan<char>, ReadOnlySpan<char>, bool> validator
    )
    {
        if (sets.Length == 0)
        {
            throw new ArgumentException(nameof(sets));
        }
        if (sets.Length == 1)
        {
            return sets[0];
        }

        var product = new HashSet<string>();

        foreach (var prefix in sets[0])
        {
            foreach (var suffix in ComputeCartesianProduct(sets[1..], validator))
            {
                var combination = prefix + suffix;
                foreach (var validCombination in ValidCombinations)
                {
                    if (validator(validCombination, combination))
                    {
                        product.Add(combination);
                        break;
                    }
                }
            }
        }

        return product;
    }

    private static bool SuffixValidator(ReadOnlySpan<char> valid, ReadOnlySpan<char> combo)
        => valid.EndsWith(combo, StringComparison.Ordinal);

    private static bool SubstringValidator(ReadOnlySpan<char> valid, ReadOnlySpan<char> combo)
        => valid.Contains(combo, StringComparison.Ordinal);
}
