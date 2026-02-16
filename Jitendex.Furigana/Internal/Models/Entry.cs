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

namespace Jitendex.Furigana.Internal.Models;

internal sealed record VocabEntry(string KanjiFormText, string ReadingText) : Entry(KanjiFormText, ReadingText);
internal sealed record NameEntry(string KanjiFormText, string ReadingText) : Entry(KanjiFormText, ReadingText);

internal abstract record Entry
{
    public string KanjiFormText { get; }
    public ImmutableArray<Rune> KanjiFormRunes { get; }
    public ImmutableArray<Rune> NormalizedKanjiFormRunes { get; }

    public string ReadingText { get; }
    public string NormalizedReadingText { get; }

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
}
