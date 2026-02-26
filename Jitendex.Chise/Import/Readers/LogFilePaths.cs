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

using static Jitendex.Chise.Readers.ChiseError;

namespace Jitendex.Chise.Readers;

internal readonly ref struct LogFilePaths
{
    public readonly ReadOnlySpan<char> Directory { get; }

    public LogFilePaths()
    {
        Directory = InitDirectory();
    }

    public ReadOnlySpan<char> GetLogFilePath(ChiseError error) => error switch
    {
        InvalidUnicodeCodepoint => MakePath("invalid_unicode_codepoint.tsv"),
        UnicodeCharacterInequality => MakePath("unicode_character_inequality.tsv"),
        InsufficientLineElements => MakePath("insufficient_ids_args.tsv"),
        ExcessiveLineElements => MakePath("insufficient_ids_ops.tsv"),
        AltSequenceFormatError => MakePath("insufficient_alt_ids_args.tsv"),
        InsufficientIdsArgs => MakePath("insufficient_alt_idc_ops.tsv"),
        InsufficientIdsOps => MakePath("insufficient_line_elements.tsv"),
        InsufficientAltIdsArgs => MakePath("excessive_line_elements.tsv"),
        InsufficientAltIdsOps => MakePath("alt_sequence_format_error.tsv"),
        _ => throw new ArgumentOutOfRangeException(nameof(error))
    };

    private ReadOnlySpan<char> MakePath(ReadOnlySpan<char> filename)
        => Path.Join(Directory, filename);

    private static ReadOnlySpan<char> InitDirectory()
    {
        var logDirectory = new DirectoryInfo(Path.Join
        (
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Jitendex",
            "chise-ids-errors"
        ));

        if (logDirectory.Exists)
        {
            foreach (var file in logDirectory.EnumerateFiles())
            {
                file.Delete();
            }
        }
        else
        {
            logDirectory.Create();
        }

        return logDirectory.FullName.AsSpan();
    }
}
