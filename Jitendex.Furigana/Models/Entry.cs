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

namespace Jitendex.Furigana.Models;

public abstract class Entry
{
    public string KanjiFormText { get; }
    internal ImmutableArray<Rune> KanjiFormRunes { get; }
    internal ImmutableArray<Rune> NormalizedKanjiFormRunes { get; }

    public string ReadingText { get; }
    internal string NormalizedReadingText { get; }

    public Entry(string kanjiFormText, string readingText)
    {
        if (readingText.Any(char.IsSurrogate))
        {
            throw new ArgumentException
            (
                message: "Reading text must not contain characters with surrogate code units.",
                paramName: nameof(readingText)
            );
        }

        KanjiFormText = kanjiFormText;
        KanjiFormRunes = [.. kanjiFormText.EnumerateRunes()];
        NormalizedKanjiFormRunes = [.. KanjiFormRunes.IterationMarksToKanji()];

        ReadingText = readingText;
        NormalizedReadingText = readingText.KatakanaToHiragana();
    }

    public override abstract bool Equals(object? obj);

    public override abstract int GetHashCode();

    public override string ToString() =>
        $"{GetType()}: {ReadingText}【{KanjiFormText}】";
}
