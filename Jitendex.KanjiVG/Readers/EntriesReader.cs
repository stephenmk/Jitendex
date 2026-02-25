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

using Microsoft.Extensions.Logging;
using Jitendex.KanjiVG.Models;

namespace Jitendex.KanjiVG.Readers;

internal partial class EntriesReader
(
    ILogger<EntriesReader> logger,
    KanjiFiles kanjiFiles,
    EntryReader entryReader
)
{
    public async Task<List<Entry>> ReadAsync()
    {
        var entries = new List<Entry>(12_000);

        await foreach (var (fileName, xmlReader) in kanjiFiles.EnumerateAsync())
        {
            var entry = await entryReader.ReadAsync(fileName, xmlReader);
            if (entry is not null)
            {
                CheckStrokeNumberCount(entry);
                entries.Add(entry);
            }
        }

        return entries;
    }

    private void CheckStrokeNumberCount(Entry entry)
    {
        int componentStrokeCount = entry.ComponentGroup.StrokeCount();
        int strokeNumberCount = entry.StrokeNumberGroup.StrokeNumbers.Count;
        if (componentStrokeCount != strokeNumberCount)
        {
            LogStrokeCountMismatch(entry.FileName(), componentStrokeCount, strokeNumberCount);
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "File `{File}` contains {ComponentStrokes} stroke paths and {StrokeNumbers} stroke numbers")]
    partial void LogStrokeCountMismatch(string file, int componentStrokes, int strokeNumbers);
}
