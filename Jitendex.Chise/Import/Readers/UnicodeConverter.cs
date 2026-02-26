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

internal static class UnicodeConverter
{
    public static int? ScalarValue(in ReadOnlySpan<char> character) => character switch
    {
        { Length: 1 } => character[0],
        { Length: 2 } when char.IsHighSurrogate(character[0])
                        && char.IsLowSurrogate(character[1])
                        => char.ConvertToUtf32(character[0], character[1]),
        _ => null,
    };

    public static ReadOnlySpan<char> GetLongCodepointId(int scalarValue) => $"&U-{scalarValue:X8};";
    public static ReadOnlySpan<char> GetShortCodepointId(int scalarValue) => $"&U+{scalarValue:X};";
}
