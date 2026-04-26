// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DocumentReader.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using System.Xml;
using Jitendex.Import.KanjiVG.Models;
using Jitendex.Import.KanjiVG.Readers;

namespace Jitendex.Import.KanjiVG;

internal sealed class DocumentReader(KanjiReader kanjiReader)
{
    public async Task<Document> ReadAsync(DirectoryInfo kanjivgDirectory)
    {
        var document = new Document();

        foreach (var file in kanjivgDirectory.CreateSubdirectory("kanji").EnumerateFiles())
        {
            await using var stream = file.OpenRead();
            using var xmlReader = XmlReader.Create(stream, XmlReaderSettings);
            await kanjiReader.ReadAsync(xmlReader, document, file.Name);
        }

        return document;
    }

    private static readonly XmlReaderSettings XmlReaderSettings = new()
    {
        Async = true,
        DtdProcessing = DtdProcessing.Parse,
        MaxCharactersFromEntities = long.MaxValue,
        MaxCharactersInDocument = long.MaxValue,
    };
}
