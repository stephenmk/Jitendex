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

internal class Logger
{
    private readonly Dictionary<ChiseError, List<string>> _logs = [];
    public void LogInvalidUnicodeCodepoint(in LineElements line) => LogLine(line, InvalidUnicodeCodepoint);
    public void LogUnicodeCharacterInequality(in LineElements line) => LogLine(line, UnicodeCharacterInequality);
    public void LogInsufficientIdsArgs(in LineElements line) => LogLine(line, InsufficientIdsArgs);
    public void LogInsufficientIdsOps(in LineElements line) => LogLine(line, InsufficientIdsOps);
    public void LogInsufficientAltIdsArgs(in LineElements line) => LogLine(line, InsufficientAltIdsArgs);
    public void LogInsufficientAltIdsOps(in LineElements line) => LogLine(line, InsufficientAltIdsOps);

    public void LogLineErrors(in LineElements line)
    {
        if (line.InsufficientElementsError)
        {
            LogLine(line, InsufficientLineElements);
        }
        if (line.ExcessiveElementsError)
        {
            LogLine(line, ExcessiveLineElements);
        }
        if (line.AltSequenceFormatError)
        {
            LogLine(line, AltSequenceFormatError);
        }
    }
    private void LogLine(in LineElements line, ChiseError error)
    {
        if (_logs.TryGetValue(error, out var lines))
        {
            lines.Add(line.ToString());
        }
        else
        {
            _logs[error] = [line.ToString()];
        }
    }

    public void WriteLogs()
    {
        if (_logs.Count == 0)
        {
            return;
        }

        var paths = new LogFilePaths();

        Console.Error.WriteLine($"Log directory: {paths.Directory}");

        foreach (var (error, lines) in _logs)
        {
            var path = paths.GetLogFilePath(error);
            File.WriteAllLines(path.ToString(), lines);
            Console.Error.WriteLine($"\t{Path.GetFileName(path)}\t{lines.Count:n0} logs");
        }
    }
}
