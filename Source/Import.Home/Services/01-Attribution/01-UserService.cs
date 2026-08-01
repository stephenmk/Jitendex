// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 01-UserService.cs, is part of Jitendex.
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

using System.Text.Encodings.Web;
using System.Text.Json;
using Jitendex.Data.Home;

namespace Jitendex.Import.Home.Services.Attribution;

internal sealed class UserService
(
    HomeContext context,
    ServiceOptions options
) :
    IServiceUnit
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
            .OrderBy(static x => x.Id)
            .Select(static x => new
            {
                Key = x.Id,
                Value = x.Name,
            })
            .ToDictionary(static x => x.Key, static x => x.Value);

        var filePath = GetJsonFilePath();
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, dictionary, JsonSerializerOptions);
    }

    private string GetJsonFilePath()
        => Path.Join
        (
            options.GetDirectory(DataDirectory.Attribution).FullName,
            "users.json"
        );

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.Default,
    };
}
