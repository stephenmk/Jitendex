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

using System.IO.Compression;

namespace Jitendex.EdrdgDictionaryArchive.Internal;

internal static class FileInfoExtensions
{
    public static int Length(this FileInfo file)
    {
        using FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using BrotliStream bs = new(fs, CompressionMode.Decompress);
        using StreamReader sr = new(bs);
        int length = 0;
        while (sr.Read() != -1)
        {
            length++;
        }
        return length;
    }

    public static int ReadInto(this FileInfo file, Span<char> buffer)
    {
        using FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using BrotliStream bs = new(fs, CompressionMode.Decompress);
        using StreamReader sr = new(bs);
        int length = 0;
        while (sr.Read(buffer[length..]) is int charsRead and not 0)
        {
            length += charsRead;
        }
        return length;
    }

    public static void WriteCompressed(this FileInfo file, ReadOnlySpan<char> text)
    {
        using FileStream fs = new(file.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        using BrotliStream bs = new(fs, CompressionLevel.Optimal); // CompressionLevel.Fastest is 3x larger with hardly any perf increase
        using StreamWriter sw = new(bs);
        sw.Write(text);
    }
}
