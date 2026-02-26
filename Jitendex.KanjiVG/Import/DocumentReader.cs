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

using System.Xml;
using Jitendex.KanjiVG.Import.Models;
using Jitendex.KanjiVG.Import.Readers;

namespace Jitendex.KanjiVG.Import;

internal sealed class DocumentReader(EntryReader entryReader)
{
    public async Task<Document> ReadAsync(DirectoryInfo kanjiDirectory)
    {
        var document = new Document();

        await foreach (var (xmlReader, fileName) in EnumerateAsync(kanjiDirectory))
        {
            await entryReader.ReadAsync(xmlReader, document, fileName);
        }

        return document;
    }

    public async IAsyncEnumerable<(XmlReader, string)> EnumerateAsync(DirectoryInfo kanjiDirectory)
    {
        foreach (var file in kanjiDirectory.EnumerateFiles())
        {
            await using var stream = file.OpenRead();
            using var xmlReader = XmlReader.Create(stream, XmlReaderSettings);
            yield return (xmlReader, file.Name);
        }
    }

    private static readonly XmlReaderSettings XmlReaderSettings = new()
    {
        Async = true,
        DtdProcessing = DtdProcessing.Parse,
        MaxCharactersFromEntities = long.MaxValue,
        MaxCharactersInDocument = long.MaxValue,
    };
}
