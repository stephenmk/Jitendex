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
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using Jitendex.Data.Home;
using Jitendex.Import.Home.Models;
using Jitendex.Import.Home.Tables.Furigana;

namespace Jitendex.Import.Home.Services.Furigana;

internal sealed class CompoundService
(
    HomeContext context,
    ServiceOptions options,
    CompoundTable compoundTable,
    CompoundReadingTable compoundReadingTable
)
{
    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, string[]>>(stream) ?? [];

        var compoundRows = new List<CompoundRow>();
        var readingRows = new List<CompoundReadingRow>();

        foreach (var (key, values) in data)
        {
            compoundRows.Add(new(key));
        }

        compoundTable.InsertItems(context, compoundRows);

        var compoundToId = context.Compounds
            .Select(static c => new {Key = c.Text, Value = c.Id})
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        foreach (var (key, values) in data)
        {
            foreach (var value in values)
            {
                readingRows.Add(new(compoundToId[key], value));
            }
        }

        compoundReadingTable.InsertItems(context, readingRows);
    }

    public async Task ExportAsync()
    {
        var dictionary = context.Compounds
            .Select(static x => new
            {
                Key = x.Text,
                Value = x.Readings
                    .Select(static r => r.Text)
                    .Order()
                    .ToArray()
            })
            .AsEnumerable()
            .OrderBy(static x => x.Key, StringComparer)
            .ToDictionary(static x => x.Key, static x => x.Value);

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
            "compounds.json"
        );

    private readonly static JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly static StringComparer StringComparer =
        StringComparer.Create(new CultureInfo("ja-JP"), CompareOptions.NumericOrdering);
}
