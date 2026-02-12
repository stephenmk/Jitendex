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
using Jitendex.Furigana.Models;

namespace Jitendex.Furigana.Solver;

internal class SolutionBuilder
{
    private readonly List<SolutionPart> _parts;

    public SolutionBuilder() : this([]) { }

    public SolutionBuilder(IEnumerable<SolutionPart> parts)
    {
        _parts = [.. parts];
    }

    public string KanjiFormText() => new(_parts.SelectMany(static x => x.BaseText).ToArray());
    public string ReadingText() => new(_parts.SelectMany(static x => x.Furigana ?? x.BaseText).ToArray());
    private string NormalizedReadingText() => ReadingText().KatakanaToHiragana();
    public int ReadingTextLength() => ReadingText().Length;

    public void Add(SolutionPart part) => _parts.Add(part);
    public ImmutableArray<SolutionPart> ToParts() => [.. _parts];

    public Solution? ToSolution(Entry entry) => !IsValid(entry) ? null : new Solution
    {
        Entry = entry,
        Parts = NormalizedParts(),
    };

    /// <summary>
    /// Determine if the parts contained within this builder are valid for the given entry.
    /// </summary>
    /// <remarks>
    /// Solutions may be valid even if they do not contain furigana for every non-kana rune
    /// in the entry's <see cref="Entry.KanjiFormText"/> property. This is by design to allow for solutions to
    /// entries containing punctuation.
    /// <list type="bullet">
    /// <item>ブルータス、お[前|まえ]もか</item>
    /// <item>アンドロイドは[電|でん][気|き][羊|ひつじ]の[夢|ゆめ]を[見|み]るか？</item>
    /// </list>
    /// However, any part that contains a kanji rune must contain furigana.
    /// </remarks>
    private bool IsValid(Entry entry)
        => entry.NormalizedReadingText == NormalizedReadingText()
        && entry.KanjiFormText == KanjiFormText()
        && _parts
            .Where(static part => part.BaseText.EnumerateRunes().Any(KanjiComparison.IsKanji))
            .All(static part => !string.IsNullOrWhiteSpace(part.Furigana));

    /// <summary>
    /// Merge consecutive parts together if they have null furigana.
    /// Ignore merged parts with both empty text and null furigana.
    /// </summary>
    private ImmutableArray<SolutionPart> NormalizedParts()
    {
        var parts = new List<SolutionPart>(_parts.Count);
        var mergedTexts = new StringBuilder();
        foreach (var part in _parts)
        {
            if (part.Furigana is null)
            {
                mergedTexts.Append(part.BaseText);
                continue;
            }
            if (mergedTexts.Length > 0)
            {
                parts.Add(new SolutionPart { BaseText = mergedTexts.ToString() });
                mergedTexts.Clear();
            }
            parts.Add(part);
        }
        if (mergedTexts.Length > 0)
        {
            parts.Add(new SolutionPart { BaseText = mergedTexts.ToString() });
        }
        return parts
            .Where(static part => part.BaseText != string.Empty || part.Furigana is not null)
            .ToImmutableArray();
    }
}
