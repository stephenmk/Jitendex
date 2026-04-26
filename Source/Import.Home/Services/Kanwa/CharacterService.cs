// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, CharacterService.cs, is part of Jitendex.
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

using System.Collections.Frozen;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Jitendex.Data.Home;
using Jitendex.Data.Home.Entities.Kanwa;
using Jitendex.Import.Home.TableRows;
using Jitendex.Import.Home.Tables.Kanwa;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Import.Home.Services.Kanwa;

internal sealed class CharacterService
(
    HomeContext context,
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
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonObject>>(stream, ReadOptions) ?? [];

        var characterRows = new List<CharacterRow>();
        var readingRows = new List<CharacterReadingRow>();

        foreach (var (key, obj) in data)
        {
            var characterValue = key.EnumerateRunes().First().Value;
            characterRows.Add(new(characterValue));

            foreach (var (typeName, node) in obj)
            {
                var readings = (JsonArray)node!;
                var typeId = TypeNameToId[typeName];
                foreach (var reading in readings)
                {
                    const string hyphen = "-";
                    const char delimiter = '.';
                    var text = (string)reading!;

                    if (text.Equals(hyphen, StringComparison.Ordinal))
                    {
                        readingRows.Add(new(characterValue, text, false, false, null, (int)typeId));
                        continue;
                    }

                    var split = text.Replace(hyphen, string.Empty).Split(delimiter);
                    readingRows.Add(new
                    (
                        CharacterValue: characterValue,
                        Text: split[0],
                        IsPrefix: text.EndsWith('-'),
                        IsSuffix: text.StartsWith('-'),
                        Okurigana: split.Length == 2 ? split[1] : null,
                        ReadingTypeId: (int)typeId
                    ));
                }
            }
        }

        typeTable.InsertItems(context, GetTypeRows());
        characterTable.InsertItems(context, characterRows);
        readingTable.InsertItems(context, readingRows);
    }

    public async Task ExportAsync()
    {
        var data = context.Characters
            .AsNoTracking()
            .AsSplitQuery()
            .Include(static x => x.Readings)
            .ThenInclude(static x => x.Type)
            .Where(static x => x.Readings.Any())
            .OrderBy(static x => x.Value)
            .ToDictionary
            (
                keySelector: static x => new Rune(x.Value).ToString(),
                elementSelector: static x => x.Readings
                    .OrderBy(static x => x.TypeId)
                    .GroupBy(static x => x.Type.Name.ToLower())
                    .ToDictionary
                    (
                        keySelector: static x => x.Key,
                        elementSelector: static x => x
                            .Select(static x => x.ToString())
                            .Order()
                            .ToArray()
                    )
            );

        var filePath = GetJsonFilePath();
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, data, WriteOptions);
    }

    private string GetJsonFilePath()
        => Path.Join
        (
            options.GetKanwaDirectory().FullName,
            "characters.json"
        );

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static IEnumerable<CharacterReadingTypeRow> GetTypeRows()
        => TypeNameToId.Select(static x => new CharacterReadingTypeRow((int)x.Value, x.Key));

    private static readonly FrozenDictionary<string, CharacterReadingTypeId> TypeNameToId = Enum
        .GetValues<CharacterReadingTypeId>()
        .Select(static type => new
        {
            Key = type.ToString().ToLower(),
            Value = type,
        })
        .ToFrozenDictionary(static x => x.Key, static x => x.Value);
}
