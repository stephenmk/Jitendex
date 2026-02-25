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

using Jitendex.KanjiVG.Models;
using Jitendex.KanjiVG.Readers.Lookups;

namespace Jitendex.KanjiVG.Readers;

internal sealed class KanjiVGReader
(
        EntriesReader entriesReader,
        VariantTypeCache variantTypeCache,
        CommentCache commentCache,
        ComponentGroupStyleCache componentGroupStyleCache,
        StrokeNumberGroupStyleCache strokeNumberGroupStyleCache,
        ComponentCharacterCache characterCache,
        ComponentOriginalCache originalCache,
        ComponentPositionCache positionCache,
        ComponentRadicalCache radicalCache,
        ComponentPhonCache phonCache,
        StrokeTypeCache strokeTypeCache
)
{
    public async Task<KanjiVGDocument> ReadAsync(FileInfo kanjivgFile)
    {
        var entries = await entriesReader.ReadAsync(kanjivgFile);

        var kanjivg = new KanjiVGDocument
        {
            Entries = entries,
            VariantTypes = [.. variantTypeCache.Values],
            Comments = [.. commentCache.Values],
            ComponentGroupStyles = [.. componentGroupStyleCache.Values],
            StrokeNumberGroupStyles = [.. strokeNumberGroupStyleCache.Values],
            ComponentCharacters = [.. characterCache.Values],
            ComponentOriginals = [.. originalCache.Values],
            ComponentPositions = [.. positionCache.Values],
            ComponentRadicals = [.. radicalCache.Values],
            ComponentPhons = [.. phonCache.Values],
            StrokeTypes = [.. strokeTypeCache.Values],
        };

        return kanjivg;
    }
}
