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
using System.Text;
using System.Text.RegularExpressions;
using Jitendex.JapaneseTextUtils;
using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana.Internal.Algorithms.APriori;

internal abstract class CharacterAlgorithm
{
    public abstract ImmutableArray<List<Solution.Part>> Solve(in TextSlice textSlice, in ReadingState readingState);

    protected static string? RegexReading(in TextSlice textSlice, in ReadingState readingState)
    {
        var remainingText = textSlice.RemainingRunes.KatakanaToHiragana();
        var remainingReading = readingState.RemainingTextNormalized.ToString();

        var greedyRegex = MakeRegex("(.+)", remainingText);
        var lazyRegex = MakeRegex("(.+?)", remainingText);

        var greedyMatch = greedyRegex.Match(remainingReading);
        var lazyMatch = lazyRegex.Match(remainingReading);

        if (!greedyMatch.Success || !lazyMatch.Success)
        {
            return null;
        }

        var greedyValue = greedyMatch.Groups[1].Value;
        var lazyValue = lazyMatch.Groups[1].Value;

        if (greedyValue != string.Empty && string.Equals(greedyValue, lazyValue, StringComparison.Ordinal))
        {
            return greedyValue;
        }
        else
        {
            return null;
        }
    }

    private static Regex MakeRegex(ReadOnlySpan<char> groupPattern, ReadOnlySpan<char> text)
    {
        var pattern = new StringBuilder($"^{groupPattern}");
        bool newGroup = false;
        foreach (var character in text)
        {
            if (character.IsKana())
            {
                pattern.Append(character);
                newGroup = true;
            }
            else if (newGroup)
            {
                pattern.Append(groupPattern);
                newGroup = false;
            }
        }
        pattern.Append('$');
        return new Regex(pattern.ToString());
    }
}
