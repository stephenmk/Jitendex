/*
Copyright (c) 2025 Stephen Kraus
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

namespace Jitendex.Chise.Readers;

internal readonly ref struct LineElements
{
    private const string apparent = "@apparent=";
    private readonly ReadOnlySpan<char> Filename;
    private readonly int LineNumber;
    private readonly ReadOnlySpan<char> Line;

    public readonly ReadOnlySpan<char> Codepoint;
    public readonly ReadOnlySpan<char> Character;
    public readonly ReadOnlySpan<char> Sequence;
    public readonly ReadOnlySpan<char> AltSequence;

    public readonly bool AltSequenceFormatError;
    public readonly bool InsufficientElementsError;
    public readonly bool ExcessiveElementsError;

    public LineElements(ReadOnlySpan<char> filename, int number, ReadOnlySpan<char> line)
    {
        Filename = filename;
        LineNumber = number;
        Line = line;

        int idx = 0;

        foreach (var range in line.Split('\t'))
        {
            var segment = line[range];

            if (idx == 0)
                Codepoint = $"&{segment.Trim()};";
            else if (idx == 1)
                Character = segment;
            else if (idx == 2)
                Sequence = segment[..^TagLength(segment)];
            else if (idx == 3 && segment.StartsWith(apparent))
                AltSequence = segment[apparent.Length..^TagLength(segment)];
            else if (idx == 3)
                AltSequenceFormatError = true;
            else
                ExcessiveElementsError = true;

            idx++;
        }

        if (idx < 3)
        {
            InsufficientElementsError = true;
        }
    }

    /// <summary>
    /// Length of a trailing "[U]" "[J]" "[K]" etc tag at the end of an IDS text
    /// </summary>
    private static int TagLength(ReadOnlySpan<char> sequence) => sequence switch
    {
        [.., ' ', '[', _, ']'] => 4,
        [.., '[', _, ']'] => 3,
        _ => 0,
    };

    public override string ToString() => $"{Filename}\t{LineNumber}\t{Line}";
}
