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

namespace Jitendex.HomeData.ImportExport;

internal sealed class UserService(HomeDataContext context, ServiceOptions options)
{
    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<int, string>>(stream) ?? [];

        foreach (var (key, value) in data)
        {
            context.Users.Add(new()
            {
                Id = key,
                Name = value,
            });
        }

        context.SaveChanges();
    }

    public async Task ExportAsync()
    {
        var dictionary = context.Users
            .AsNoTracking()
            .OrderBy(static x => x.Id)
            .ToDictionary(static x => x.Id, static x => x.Name);

        var filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        await using var stream = File.OpenWrite(filePath);
        await JsonSerializer.SerializeAsync(stream, dictionary, JsonSerializerOptions);
    }

    private string GetJsonFilePath()
        => Path.Join(options.DataDirectory.FullName, "users.json");

    private readonly static JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.Default,
    };
}
