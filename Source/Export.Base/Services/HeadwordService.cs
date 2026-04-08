/*
Copyright (c) 2026 Stephen Kraus
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Jitendex.Data.Export;
using Jitendex.Data.JMdict;
using Jitendex.Export.Base.TableRows;
using Jitendex.Export.Base.Tables;

namespace Jitendex.Export.Base.Services;

internal partial class HeadwordService
(
    ILogger<HeadwordService> logger,
    ExportContext context,
    JMdictForkContext jmdictContext,
    HeadwordTable headwordTable,
    HeadwordFuriganaTable headwordFuriganaTable
)
{
    public void Write()
    {
        WriteHeadwords();
        WriteHeadwordFurigana();
    }

    private void WriteHeadwords()
    {
        var rows = jmdictContext.Headwords
            .OrderBy(static h => h.Surface)
            .ThenBy(static h => h.Reading)
            .Select(static h => new HeadwordRow(h.Surface, h.Reading));

        headwordTable.InsertOrIgnoreItems(context, rows);
    }

    private void WriteHeadwordFurigana()
    {
        var headwordToId = context.Headwords
            .Where(static h => h.Reading != null)
            .Select(static h => new { h.Id, h.Surface, h.Reading })
            .ToFrozenDictionary
            (
                static x => (x.Surface, x.Reading!),
                static x => x.Id
            );

        var headwords = jmdictContext.Headwords
            .AsSplitQuery()
            .Where(static h => h.ReadingKanjiFormBridge != null)
            .Where(static h => h.Reading != null)
            .Select(static h => new
            {
                h.Surface,
                Reading = h.Reading!,
                Furigana = h.ReadingKanjiFormBridge!.FuriganaSegments
                    .Select(static s => new
                    {
                        s.Order,
                        s.BaseText,
                        s.Furigana,
                    })
            });

        var seenHeadwords = new HashSet<int>(headwordToId.Count);
        var rows = new List<HeadwordFuriganaRow>();

        foreach (var headword in headwords)
        {
            if (!headwordToId.TryGetValue((headword.Surface, headword.Reading), out var id))
            {
                LogMissingFurigana(headword.Surface, headword.Reading);
                continue;
            }
            if (!seenHeadwords.Add(id))
            {
                continue;
            }
            foreach (var segment in headword.Furigana)
            {
                rows.Add(new(id, segment.Order, segment.BaseText, segment.Furigana));
            }
        }

        headwordFuriganaTable.InsertItems(context, rows);
    }

    [LoggerMessage(LogLevel.Warning, "No ID found for headword {Reading}【{Surface}】")]
    partial void LogMissingFurigana(string surface, string reading);
}
