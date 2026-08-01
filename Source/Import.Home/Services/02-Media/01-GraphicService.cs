// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 01-GraphicService.cs, is part of Jitendex.
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
using System.Text.Encodings.Web;
using System.Text.Json;
using Jitendex.Data.Home;
using Jitendex.Import.Home.TableRows;
using Jitendex.Import.Home.Tables.Media;

namespace Jitendex.Import.Home.Services.Media;

internal sealed class GraphicService
(
    HomeContext context,
    ServiceOptions options,
    GraphicTable graphicTable
) :
    IServiceUnit
{
    private sealed record GraphicObject
    (
        string? Title,
        string PageUrl,
        string FileUrl,
        string Author,
        string? AuthorUrl,
        string License
    );

    public async Task ImportAsync()
    {
        var index = LoadIndex();
        var fileData = LoadFileData();
        var licenseNameToId = context.Licenses
            .Select(static l => new { l.Id, l.Name })
            .ToFrozenDictionary(static x => x.Name, static x => x.Id);

        var graphicRows = new List<GraphicRow>();

        foreach (var (id, obj) in index)
        {
            graphicRows.Add(new
            (
                Id: id,
                LicenseId: licenseNameToId[obj.License],
                Cropped: false,
                PageUrl: obj.PageUrl,
                FileUrl: obj.FileUrl,
                Author: obj.Author,
                AuthorUrl: obj.AuthorUrl,
                Title: obj.Title,
                FileData: fileData[id]
            ));
        }

        graphicTable.InsertItems(context, graphicRows);
    }

    private Dictionary<int, GraphicObject> LoadIndex()
    {
        var filePath = GetJsonFilePath();
        using var stream = File.OpenRead(filePath);
        return JsonSerializer.Deserialize<Dictionary<int, GraphicObject>>(stream, ReadOptions) ?? [];
    }

    private Dictionary<int, byte[]> LoadFileData()
    {
        var fileData = new Dictionary<int, byte[]>();
        foreach (var dir in options.GetDirectory(DataDirectory.Graphics).EnumerateDirectories())
        {
            var idRange = 1000 * int.Parse(dir.Name);
            foreach (var file in dir.EnumerateFiles())
            {
                var basename = Path.GetFileNameWithoutExtension(file.Name);
                var id = idRange + int.Parse(basename);
                fileData.Add(id, File.ReadAllBytes(file.FullName));
            }
        }
        return fileData;
    }

    public async Task ExportAsync()
    {
        var data = context.Graphics
            .OrderBy(static g => g.Id)
            .Select(static g => new
            {
                Key = g.Id,
                Value = new GraphicObject
                (
                    Title: g.Title,
                    PageUrl: g.PageUrl,
                    FileUrl: g.FileUrl,
                    Author: g.Author,
                    AuthorUrl: g.AuthorUrl,
                    License: g.License.Name
                )
            })
            .ToDictionary(static x => x.Key, static x => x.Value);

        var filePath = GetJsonFilePath();
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, data, WriteOptions);
        stream.Write("\n"u8);
    }

    private static readonly JsonSerializerOptions ReadOptions
        = new()
        {
            PropertyNameCaseInsensitive = true
        };

    private static readonly JsonSerializerOptions WriteOptions
        = new()
        {
            WriteIndented = true,
            IndentSize = 4,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

    private string GetJsonFilePath()
        => Path.Join
        (
            options.GetDirectory(DataDirectory.Graphics).FullName,
            "index.json"
        );
}
