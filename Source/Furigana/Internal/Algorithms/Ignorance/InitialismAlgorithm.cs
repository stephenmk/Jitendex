// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, InitialismAlgorithm.cs, is part of Jitendex.
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

namespace Jitendex.Furigana.Internal.Algorithms.Ignorance;

/// <summary>
/// Solves compounds consisting of fullwidth alphabetical characters and readings
/// consisting of alphabetical pronunciations separated by interpuncts.
/// </summary>
internal sealed class InitialismAlgorithm : CharacterAlgorithm
{
    public override ImmutableArray<ImmutableArray<SolutionPart>> Solve(in TextSlice textSlice, in ReadingState readingState)
    {
        if (!textSlice.Runes.AreAllFullwidthAlphanumeric())
            return [];

        var reading = GetInitialismReading(textSlice);

        if (!readingState.RemainingTextNormalized.StartsWith(reading, StringComparison.Ordinal))
            return [];

        var baseText = textSlice.RawRunes.FastToString();
        var furigana = new string(readingState.RemainingText[..reading.Length]);

        return [[new SolutionPart(baseText, furigana)]];
    }

    private static string GetInitialismReading(in TextSlice textSlice)
    {
        var readingParts = new string[textSlice.Runes.Length];

        for (int i = 0; i < readingParts.Length; i++)
        {
            readingParts[i] = FullwidthAlphanumericToReading(textSlice.Runes[i].Value);
        }

        return string.Join("・", readingParts);
    }

    private static string FullwidthAlphanumericToReading(int val) => val switch
    {
        '０' => "ぜろ",
        '１' => "わん",
        '２' => "つー",
        '３' => "すりー",
        '４' => "ふぉー",
        '５' => "ふぁいぶ",
        '６' => "しっくす",
        '７' => "せぶん",
        '８' => "えいと",
        '９' => "ないん",
        'Ａ' or 'ａ' => "えー",
        'Ｂ' or 'ｂ' => "びー",
        'Ｃ' or 'ｃ' => "しー",
        'Ｄ' or 'ｄ' => "でぃー",
        'Ｅ' or 'ｅ' => "いー",
        'Ｆ' or 'ｆ' => "えふ",
        'Ｇ' or 'ｇ' => "じー",
        'Ｈ' or 'ｈ' => "えいち",
        'Ｉ' or 'ｉ' => "あい",
        'Ｊ' or 'ｊ' => "じぇー",
        'Ｋ' or 'ｋ' => "けー",
        'Ｌ' or 'ｌ' => "える",
        'Ｍ' or 'ｍ' => "えむ",
        'Ｎ' or 'ｎ' => "えぬ",
        'Ｏ' or 'ｏ' => "おー",
        'Ｐ' or 'ｐ' => "ぴー",
        'Ｑ' or 'ｑ' => "きゅー",
        'Ｒ' or 'ｒ' => "あーる",
        'Ｓ' or 'ｓ' => "えす",
        'Ｔ' or 'ｔ' => "てぃー",
        'Ｕ' or 'ｕ' => "ゆー",
        'Ｖ' or 'ｖ' => "ぶい",
        'Ｗ' or 'ｗ' => "だぶりゅー",
        'Ｘ' or 'ｘ' => "えっくす",
        'Ｙ' or 'ｙ' => "わい",
        'Ｚ' or 'ｚ' => "ぜっと",
        _ => throw new ArgumentOutOfRangeException(nameof(val))
    };
}
