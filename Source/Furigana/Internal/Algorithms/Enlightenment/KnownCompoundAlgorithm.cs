// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, KnownCompoundAlgorithm.cs, is part of Jitendex.
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

using Jitendex.Furigana.Internal.Models;
using Jitendex.JapaneseTextUtils;

namespace Jitendex.Furigana.Internal.Algorithms.Enlightenment;

internal sealed class KnownCompoundAlgorithm(IReadOnlyKnowledge knowledge)
{
    public ImmutableArray<ImmutableArray<SolutionPart>> Solve(in TextSlice textSlice, in ReadingState readingState)
    {
        var readings = knowledge.GetCompoundReadings(textSlice.Runes);

        if (!readings.Any())
            return [];

        // Note: this is an array (mutable) of immutable arrays.
        var partsLists = new ImmutableArray<SolutionPart>[readings.Count];
        int i = 0;

        foreach (var readingArray in readings)
        {
            if (readingArray[0].IsSuffix && textSlice.ContainsFirstRune)
                continue;

            if (readingArray[^1].IsPrefix && textSlice.ContainsFinalRune)
                continue;

            var text = string.Concat(readingArray.Select(static r => r.Text));

            if (!readingState.RemainingTextNormalized.StartsWith(text, StringComparison.Ordinal))
                continue;

            // If the array contains one reading, then there is one
            // reading for all the runes in the surface form.
            if (readingArray.Length == 1)
            {
                var baseText = textSlice.RawRunes.FastToString();
                var furigana = baseText.IsKanaEquivalent(text)
                    ? null
                    : new string(readingState.RemainingText[..text.Length]);

                var part = new SolutionPart(baseText, furigana)
                {
                    ReadingIds = [readingArray[0].Id]
                };
                partsLists[i++] = [part];
                continue;
            }

            // If the array contains multiple readings, then
            // there is one reading per surface rune.
            var partsList = new SolutionPart[readingArray.Length];
            int start = 0;

            for (int j = 0; j < readingArray.Length; j++)
            {
                var partBaseText = textSlice.RawRunes[j].ToString();
                var length = readingArray[j].Text.Length;
                var range = new Range(start, start + length);
                var partReading = readingState.RemainingText[range];
                var partFurigana = partBaseText.IsKanaEquivalent(partReading)
                    ? null
                    : new string(partReading);
                partsList[j] = new SolutionPart(partBaseText, partFurigana)
                {
                    ReadingIds = [readingArray[j].Id]
                };
                start += length;
            }

            partsLists[i++] = ImmutableArray.Create(partsList);
        }

        return ImmutableArray.Create(partsLists.AsSpan(..i));
    }
}
