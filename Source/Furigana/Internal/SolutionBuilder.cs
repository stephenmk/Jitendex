// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, SolutionBuilder.cs, is part of Jitendex.
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

using System.Text;
using Jitendex.Furigana.Internal.Models;
using Jitendex.JapaneseTextUtils;

namespace Jitendex.Furigana.Internal;

internal sealed record SolutionBuilder(ImmutableList<SolutionPart> Parts)
{
    public int ReadingLength()
        => Parts.Sum(static part => (part.RubyText ?? part.BaseText).Length);

    public Solution? ToSolution(in Entry entry)
        => !IsValid(entry) ? null : new Solution
        (
            Text: new(entry.Text),
            Reading: new(entry.Reading),
            Parts: GetNormalizedParts()
        );

    /// <summary>
    /// Determine if the parts contained within this builder are valid for the given entry.
    /// </summary>
    /// <remarks>
    /// Solutions may be valid even if they do not contain furigana for every non-kana rune
    /// in the entry's <see cref="Entry.Text"/> property. This is by design to allow
    /// for entries containing punctuation to be solved.
    /// <list type="bullet">
    /// <item>ブルータス、お[前|まえ]もか</item>
    /// <item>アンドロイドは[電|でん][気|き][羊|ひつじ]の[夢|ゆめ]を[見|み]るか？</item>
    /// </list>
    /// However, any part that contains a kanji rune must contain furigana.
    /// </remarks>
    private bool IsValid(in Entry entry)
        => NormalizedReading.Equals(entry.NormalizedReading, StringComparison.Ordinal)
        && Text.Equals(entry.Text, StringComparison.Ordinal)
        && Parts
            .Where(static part => part.BaseText.EnumerateRunes().Any(KanjiComparison.IsKanji))
            .All(static part => !string.IsNullOrWhiteSpace(part.RubyText));

    /// <summary>
    /// Merge consecutive parts together if they have unnecessary furigana.
    /// </summary>
    private ImmutableArray<SolutionPart> GetNormalizedParts()
    {
        var normParts = new SolutionPart[Parts.Count];
        int i = 0;
        var mergedTexts = new StringBuilder();
        foreach (var part in Parts)
        {
            if (IsMergeablePart(part))
            {
                mergedTexts.Append(part.BaseText);
                continue;
            }
            if (mergedTexts.Length > 0)
            {
                normParts[i++] = new(mergedTexts.ToString(), null);
                mergedTexts.Clear();
            }
            normParts[i++] = part;
        }
        if (mergedTexts.Length > 0)
        {
            normParts[i++] = new(mergedTexts.ToString(), null);
        }
        return ImmutableArray.Create(normParts.AsSpan(..i));
    }

    private static bool IsMergeablePart(SolutionPart part)
        => string.IsNullOrWhiteSpace(part.RubyText)
        || IsChōonpuWithVowelRuby(part);

    private static bool IsChōonpuWithVowelRuby(SolutionPart part)
        => part.BaseText is "ー"
        && part.RubyText is "あ" or "い" or "う" or "え" or "お";

    private ReadOnlySpan<char> Text => string.Create
    (
        length: Parts.Sum(static part => part.BaseText.Length),
        state: Parts,
        action: static (destination, state) =>
        {
            int charsWritten = 0;
            foreach (var part in state)
            {
                foreach (var character in part.BaseText)
                {
                    destination[charsWritten++] = character;
                }
            }
        }
    );

    private ReadOnlySpan<char> NormalizedReading => string.Create
    (
        length: ReadingLength(),
        state: Parts,
        action: static (destination, state) =>
        {
            int charsWritten = 0;
            foreach (var part in state)
            {
                var text = part.RubyText ?? part.BaseText;
                foreach (var character in text)
                {
                    destination[charsWritten++] = character.KatakanaToHiragana();
                }
            }
        }
    );
}
