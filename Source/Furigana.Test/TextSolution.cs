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

using System.Text.RegularExpressions;
using Jitendex.Furigana.Internal;
using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana.Test;

internal static partial class TextSolution
{
    [GeneratedRegex(@"([^\[]*)\[(.+?)\|(.*?)\]([^\[]*)", RegexOptions.None)]
    private static partial Regex TextSolutionRegex { get; }

    public static Solution Parse(string text, Entry entry)
    {
        var parts = new List<Solution.Part>();
        var matches = TextSolutionRegex.Matches(text);

        if (matches.Count == 0)
        {
            parts.Add(new Solution.Part(text, null));
        }

        foreach (Match match in matches)
        {
            var noFurigana1 = match.Groups[1].Value;
            var baseText = match.Groups[2].Value;
            var furigana = match.Groups[3].Value;
            var noFurigana2 = match.Groups[4].Value;

            parts.Add(new Solution.Part(noFurigana1, null));
            parts.Add(new Solution.Part(baseText, furigana));
            parts.Add(new Solution.Part(noFurigana2, null));
        }

        var solutionBuilder = new SolutionBuilder([.. parts]);
        var solution = solutionBuilder.ToSolution(entry) ??
            throw new ArgumentException("Malformatted solution text", nameof(text));

        return solution;
    }
}
