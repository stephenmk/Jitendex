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

internal sealed class VariantService
(
    HomeContext context,
    ServiceOptions options,
    CharacterTable characterTable,
    VariantTable variantTable,
    VariantTypeTable typeTable
)
{
    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonObject>>(stream) ?? [];

        var characterRows = new List<CharacterRow>();
        var rows = new List<VariantRow>();

        foreach (var (key, obj) in data)
        {
            var character = key.EnumerateRunes().First();
            characterRows.Add(new(character.Value));
            foreach (var (typeName, value) in obj)
            {
                var typeId = TypeNameToId[typeName];
                foreach (var node in (JsonArray)value!)
                {
                    var variant = ((string)node!).EnumerateRunes().First();
                    rows.Add(new(character.Value, variant.Value, (int)typeId));
                    characterRows.Add(new(variant.Value));
                }
            }
        }

        characterTable.InsertOrIgnoreItems(context, characterRows);
        typeTable.InsertItems(context, GetTypeRows());
        variantTable.InsertItems(context, rows);
    }

    public async Task ExportAsync()
    {
        var data = context.Variants
            .AsNoTracking()
            .Include(static v => v.Type)
            .OrderBy(static v => v.CharacterValue)
            .GroupBy(static v => v.CharacterValue)
            .ToDictionary
            (
                static group => new Rune(group.Key).ToString(),
                static group => group
                    .OrderBy(static v => v.TypeId)
                    .GroupBy(static v => v.TypeId)
                    .ToDictionary
                    (
                        static gg => gg.Key.ToString(),
                        static gg => gg
                            .OrderBy(static z => z.VariantValue)
                            .Select(static z => new Rune(z.VariantValue).ToString())
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
            "variants.json"
        );

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static IEnumerable<VariantTypeRow> GetTypeRows() => Enum
        .GetValues<VariantTypeId>()
        .Select(static id => new VariantTypeRow((int)id, id.ToString()));

    private static readonly FrozenDictionary<string, VariantTypeId> TypeNameToId = Enum
        .GetValues<VariantTypeId>()
        .Select(static id => new { Key = id.ToString(), Value = id })
        .ToFrozenDictionary(static x => x.Key, static x => x.Value);
}
