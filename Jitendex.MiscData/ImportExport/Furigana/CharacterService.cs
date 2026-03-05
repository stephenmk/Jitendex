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

using System.Text.Encodings.Web;
using System.Text.Json;
using Jitendex.MiscData.ImportExport.Furigana.Models;
using Jitendex.MiscData.ImportExport.Furigana.Tables;

namespace Jitendex.MiscData.ImportExport.Furigana;

internal sealed class CharacterService
(
    MiscDataContext context,
    ServiceOptions options,
    CharacterTable characterTable,
    CharacterReadingTable readingTable,
    CharacterReadingTypeTable typeTable
)
{
    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, CharacterReadingsObject>>(stream, ReadOptions) ?? [];

        var characterRows = new List<CharacterRow>();
        var readingRows = new List<CharacterReadingRow>();

        foreach (var (key, value) in data)
        {
            var characterValue = key.EnumerateRunes().First().Value;
            characterRows.Add(new(characterValue));
            readingRows.AddRange(value.ToReadingRows(characterValue));
        }

        typeTable.InsertItems(context, GetTypeRows());
        characterTable.InsertItems(context, characterRows);
        readingTable.InsertItems(context, readingRows);
    }

    public async Task ExportAsync()
    {
        var dictionary = context.Characters
            .OrderBy(static x => x.Value)
            .ToDictionary
            (
                keySelector: static x => x.Value,
                elementSelector: static x => x.Readings
                    .Select(static r => r.ToString())
                    .Order()
                    .ToArray()
            );

        var filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        await using var stream = File.OpenWrite(filePath);
        await JsonSerializer.SerializeAsync(stream, dictionary, WriteOptions);
    }

    private string GetJsonFilePath()
        => Path.Join
        (
            options.GetFuriganaDirectory().FullName,
            "characters.json"
        );

    private readonly static JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly static JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static IEnumerable<CharacterReadingTypeRow> GetTypeRows()
    {
        foreach (var type in Enum.GetValues<ReadingType>())
        {
            yield return new CharacterReadingTypeRow((int)type, type.ToString());
        }
    }
}
