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
using Microsoft.EntityFrameworkCore;
using Jitendex.Data.Home;
using Jitendex.Import.Home.JMdict.Tables;

namespace Jitendex.Import.Home.JMdict;

internal sealed class CrossReferenceDataService
(
    HomeDataContext context,
    ServiceOptions options,
    CrossReferenceSequenceTable table
)
{
    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, int?>>(stream) ?? [];

        var rows = new List<CrossReferenceSequenceRow>();

        foreach (var (key, value) in data)
        {
            var split = key.Split('・');
            rows.Add(new
            (
                EntryId: int.Parse(split[0]),
                SenseNumber: int.Parse(split[1]),
                Text: string.Join('・', split[2..]),
                RefEntryId: value
            ));
        }

        table.InsertItems(context, rows);
    }

    public async Task ExportAsync()
    {
        var dictionary = context.CrossReferenceSequences
            .AsNoTracking()
            .OrderBy(static x => x.Text)
            .OrderBy(static x => x.SenseNumber)
            .OrderBy(static x => x.EntryId)
            .ToDictionary
            (
                keySelector: static x => x.ToExportKey(),
                elementSelector: static x => x.RefEntryId
            );

        var filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        await using var stream = File.OpenWrite(filePath);
        await JsonSerializer.SerializeAsync(stream, dictionary, JsonSerializerOptions);
    }

    private string GetJsonFilePath()
        => Path.Join
        (
            options.GetJMdictDirectory().FullName,
            "cross_reference_sequences.json"
        );

    private readonly static JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
}
