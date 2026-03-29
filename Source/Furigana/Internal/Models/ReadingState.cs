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

namespace Jitendex.Furigana.Internal.Models;

internal readonly ref struct ReadingState
{
    public readonly ReadOnlySpan<char> FullText;
    public readonly ReadOnlySpan<char> PriorText;
    public readonly ReadOnlySpan<char> RemainingText;
    public readonly char FirstRemainingChar;

    public readonly ReadOnlySpan<char> FullTextNormalized;
    public readonly ReadOnlySpan<char> PriorTextNormalized;
    public readonly ReadOnlySpan<char> RemainingTextNormalized;
    public readonly char FirstRemainingNormalizedChar;

    public ReadingState(in Entry entry, int readingIndex)
    {
        FullText = entry.Reading;
        PriorText = FullText[..readingIndex];
        RemainingText = FullText[readingIndex..];
        FirstRemainingChar = RemainingText.IsEmpty ? default : RemainingText[0];

        FullTextNormalized = entry.NormalizedReading;
        PriorTextNormalized = FullTextNormalized[..readingIndex];
        RemainingTextNormalized = FullTextNormalized[readingIndex..];
        FirstRemainingNormalizedChar = RemainingTextNormalized.IsEmpty ? default : RemainingTextNormalized[0];
    }
}
