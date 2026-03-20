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
        var compounds = context.Compounds
            .Select(static compound => new
            {
                compound.Text,
                Readings = compound.Readings
                    .OrderBy(static r => r.Text)
                    .Select(static r => new
                    {
                        TypeName = r.Type.Name,
                        r.Text,
                    })
            })
            .AsEnumerable()
            .OrderBy(static x => x.Text, StringComparer);

        var dictionary = new Dictionary<string, Dictionary<string, List<string>>>();

        foreach (var compound in compounds)
        {
            var subdictionary = new Dictionary<string, List<string>>();
            foreach (var reading in compound.Readings)
            {
                if (subdictionary.TryGetValue(reading.TypeName, out var texts))
                {
                    texts.Add(reading.Text);
                }
                else
                {
                    subdictionary[reading.TypeName] = [reading.Text];
                }
            }
            dictionary[compound.Text] = subdictionary;
        }

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
