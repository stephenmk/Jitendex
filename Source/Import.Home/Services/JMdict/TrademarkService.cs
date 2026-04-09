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
using Jitendex.Data.Home;
using Jitendex.Import.Home.TableRows;
using Jitendex.Import.Home.Tables.JMdict;

namespace Jitendex.Import.Home.Services.JMdict;

internal sealed class TrademarkService
(
    HomeContext context,
    ServiceOptions options,
    TrademarkGlossTable table
)
{
    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream) ?? [];
        var rows = data.Select(static x => new TrademarkGlossRow(x.Key, x.Value));
        table.InsertItems(context, rows);
    }

    public async Task ExportAsync()
    {
        var data = context.TrademarkGlosses
            .OrderBy(static g => g.OriginalText.ToLower())
            .Select(static g => new { Key = g.OriginalText, Value = g.ReplacementText })
            .ToDictionary(static x => x.Key, static x => x.Value);

        var filePath = GetJsonFilePath();
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, data, WriteOptions);
    }

    private string GetJsonFilePath()
        => Path.Join
        (
            options.GetJMdictDirectory().FullName,
            "trademarks.json"
        );

    private readonly static JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
}
