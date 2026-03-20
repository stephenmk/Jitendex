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

using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Jitendex.Data.Home;
using Jitendex.Data.Home.Entities.Kanwa;
using Jitendex.Import.Home.Models;
using Jitendex.Import.Home.Tables.Kanwa;

namespace Jitendex.Import.Home.Services.Kanwa;

internal sealed class CompoundService
(
    HomeContext context,
    ServiceOptions options,
    CompoundTable compoundTable,
    CompoundReadingTable readingTable,
    CompoundReadingTypeTable typeTable
)
{
    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, CompoundReadingsObject>>(stream, ReadOptions) ?? [];

        var compoundRows = data.Keys.Select(static k => new CompoundRow(k));
        compoundTable.InsertItems(context, compoundRows);

        var compoundToId = context.Compounds
            .Select(static c => new { Key = c.Text, Value = c.Id })
            .ToDictionary(static x => x.Key, x => x.Value);

        typeTable.InsertItems(context, GetTypeRows());

        var readingRows = new List<CompoundReadingRow>();
        foreach (var (key, value) in data)
        {
            var id = compoundToId[key];
            readingRows.AddRange(value.ToReadingRows(id));
        }
        readingTable.InsertItems(context, readingRows);
    }

    public async Task ExportAsync()
    {
        var dictionary = context.Compounds
            .AsNoTracking()
            .Include(static c => c.Readings)
            .ThenInclude(static r => r.Type)
            .AsEnumerable()
            .OrderBy(static x => x.Text, StringComparer)
            .ToDictionary
            (
                keySelector: static compound => compound.Text,
                elementSelector: static compound => compound.Readings
                    .OrderBy(static r => r.Type.Name)
                    .GroupBy(static r => r.Type.Name)
                    .ToDictionary
                    (
                        keySelector: static x => x.Key,
                        elementSelector: static subgroup => subgroup
                            .Select(static r => r.Text)
                            .ToArray()
                    )
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
            options.GetKanwaDirectory().FullName,
            "compounds.json"
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

    private static IEnumerable<CompoundReadingTypeRow> GetTypeRows()
    {
        foreach (var type in Enum.GetValues<CompoundReadingTypeId>())
        {
            yield return new CompoundReadingTypeRow((int)type, type.ToString().ToLower());
        }
    }

    private readonly static StringComparer StringComparer =
        StringComparer.Create(new CultureInfo("ja-JP"), CompareOptions.NumericOrdering);
}
