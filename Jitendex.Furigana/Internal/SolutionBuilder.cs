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

internal class SolutionBuilder
{
    public ImmutableList<SolutionPart> Parts { get; private set; }

    public SolutionBuilder() : this([]) { }
    public SolutionBuilder(ImmutableList<SolutionPart> parts) => Parts = parts;

    public int ReadingTextLength() => Parts.Sum(static part => (part.Furigana ?? part.BaseText).Length);
    public void Add(SolutionPart part) => Parts = Parts.Add(part);

    public Solution? ToSolution(Entry entry) => !IsValid(entry) ? null : new Solution
    {
        KanjiFormText = entry.KanjiFormText,
        ReadingText = entry.ReadingText,
        Parts = NormalizedParts(),
    };

    /// <summary>
    /// Determine if the parts contained within this builder are valid for the given entry.
    /// </summary>
    /// <remarks>
    /// Solutions may be valid even if they do not contain furigana for every non-kana rune
    /// in the entry's <see cref="Entry.KanjiFormText"/> property. This is by design to allow
    /// for entries containing punctuation to be solved.
    /// <list type="bullet">
    /// <item>ブルータス、お[前|まえ]もか</item>
    /// <item>アンドロイドは[電|でん][気|き][羊|ひつじ]の[夢|ゆめ]を[見|み]るか？</item>
    /// </list>
    /// However, any part that contains a kanji rune must contain furigana.
    /// </remarks>
    private bool IsValid(Entry entry)
        => string.Equals(entry.NormalizedReadingText, NormalizedReadingText(), StringComparison.Ordinal)
        && string.Equals(entry.KanjiFormText, KanjiFormText(), StringComparison.Ordinal)
        && Parts
            .Where(static part => part.BaseText.EnumerateRunes().Any(KanjiComparison.IsKanji))
            .All(static part => !string.IsNullOrWhiteSpace(part.Furigana));

    /// <summary>
    /// Merge consecutive parts together if they have null furigana.
    /// Ignore merged parts with both empty text and null furigana.
    /// </summary>
    private ImmutableArray<SolutionPart> NormalizedParts()
    {
        var parts = new List<SolutionPart>(Parts.Count);
        var mergedTexts = new StringBuilder();
        foreach (var part in Parts)
        {
            if (part.Furigana is null)
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
        return parts
            .Where(static part => part.BaseText != string.Empty || part.Furigana is not null)
            .ToImmutableArray();
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
                var text = part.Furigana ?? part.BaseText;
                foreach (var character in text)
                {
                    destination[charsWritten++] = character.KatakanaToHiragana();
                }
            }
        }
    );
}
