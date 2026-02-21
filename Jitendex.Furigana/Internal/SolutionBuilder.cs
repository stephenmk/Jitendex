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
using Jitendex.JapaneseTextUtils;
using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana.Internal;

internal sealed record SolutionBuilder(ImmutableList<SolutionPart> Parts)
{
    public int ReadingTextLength()
        => Parts.Sum(static part => (part.RubyText ?? part.BaseText).Length);

    public Solution? ToSolution(Entry entry)
        => !IsValid(entry) ? null : new Solution
        {
            Text = entry.Text,
            Reading = entry.Reading,
            Parts = NormalizedParts(),
        };

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
    private bool IsValid(Entry entry)
        => string.Equals(entry.NormalizedReading, NormalizedReadingText(), StringComparison.Ordinal)
        && string.Equals(entry.Text, KanjiFormText(), StringComparison.Ordinal)
        && Parts
            .Where(static part => part.BaseText.EnumerateRunes().Any(KanjiComparison.IsKanji))
            .All(static part => !string.IsNullOrWhiteSpace(part.RubyText));

    /// <summary>
    /// Merge consecutive parts together if they have null furigana.
    /// </summary>
    private ImmutableArray<SolutionPart> NormalizedParts()
    {
        var parts = new List<SolutionPart>(Parts.Count);
        var mergedTexts = new StringBuilder();
        foreach (var part in Parts)
        {
            if (part.RubyText is null)
            {
                mergedTexts.Append(part.BaseText);
                continue;
            }
            if (mergedTexts.Length > 0)
            {
                parts.Add(new(mergedTexts.ToString(), null));
                mergedTexts.Clear();
            }
            parts.Add(part);
        }
        if (mergedTexts.Length > 0)
        {
            parts.Add(new(mergedTexts.ToString(), null));
        }
        return parts.ToImmutableArray();
    }

    private string KanjiFormText() => string.Create
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

    private string NormalizedReadingText() => string.Create
    (
        length: ReadingTextLength(),
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
