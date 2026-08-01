// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 02-JMdictPatchDataService.cs, is part of Jitendex.
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
using System.Text.Json.Nodes;
using Jitendex.Data.Home;
using Jitendex.Data.Home.Entities.JMdict;
using Jitendex.Import.Home.TableRows;
using Jitendex.Import.Home.Tables.JMdict;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Import.Home.Services.JMdict;

internal sealed class JMdictPatchDataService
(
    HomeContext context,
    ServiceOptions options,
    JMdictPatchRevisionTable revisionTable,
    JMdictPatchGraphicTable graphicTable
) :
    IServiceUnit
{
    public async Task ImportAsync()
    {
        var patches = new Dictionary<int, byte[]>();

        foreach (var directory in GetPatchDirectory().EnumerateDirectories())
        {
            var idRange = 1000 * int.Parse(directory.Name);
            foreach (var file in directory.EnumerateFiles())
            {
                var basename = Path.GetFileNameWithoutExtension(file.Name);
                var id = idRange + int.Parse(basename);
                patches[id] = await File.ReadAllBytesAsync(file.FullName);
            }
        }

        var revisionRows = new List<JMdictPatchRevisionRow>();
        var graphicRows = new List<JMdictPatchGraphicRow>();

        foreach (var (patchId, bytes) in patches)
        {
            var data = (JsonObject)JsonNode.Parse(bytes)!;

            if (data.TryGetPropertyValue("revision", out var patch))
            {
                revisionRows.Add(new(
                    patchId,
                    JsonDiff: JsonSerializer.SerializeToUtf8Bytes(patch)
                ));
            }

            if (data.TryGetPropertyValue("graphics", out var graphics))
            {
                int i = 0;
                foreach (var node in (JsonArray)graphics!)
                {
                    var graphic = (JsonObject)node!;
                    graphicRows.Add(new(
                        patchId,
                        Order: i++,
                        Operation: (int)Enum.Parse(typeof(PatchGraphicOperation), (string)graphic["op"]!, ignoreCase: true),
                        SenseOrder: (int)graphic["sense"]!,
                        GraphicId: (int)graphic["id"]!
                    ));
                }
            }
        }

        revisionTable.InsertItems(context, revisionRows);
        graphicTable.InsertItems(context, graphicRows);
    }

    public async Task ExportAsync()
    {
        var patchDir = GetPatchDirectory();
        int directoryCount = (context.JMdictPatches.Count() / 1000) + 1;

        for (int i = 0; i < directoryCount; i++)
            patchDir.CreateSubdirectory($"{i}");

        var query = context.JMdictPatches
            .AsSplitQuery()
            .OrderBy(static x => x.Id)
            .Select(static x => new
            {
                PatchId = x.Id,
                Revision = x.Revision != null
                    ? x.Revision.JsonDiff
                    : null,
                Graphics = x.GraphicPatches
                    .OrderBy(static g => g.Order)
                    .Select(static g => new
                    {
                        g.Operation,
                        g.SenseOrder,
                        g.GraphicId,
                    })
            });

        foreach (var x in query)
        {
            var data = new JsonObject();

            if (x.Revision is not null)
                data["revision"] = JsonNode.Parse(x.Revision);

            var graphics = new JsonArray();
            foreach (var graphic in x.Graphics)
            {
                graphics.Add(new JsonObject()
                {
                    ["op"] = graphic.Operation.ToString().ToLower(),
                    ["sense"] = graphic.SenseOrder,
                    ["id"] = graphic.GraphicId,
                });
            }
            if (graphics.Any())
                data["graphics"] = graphics;

            var filepath = GetDataFilePath(patchDir.FullName, x.PatchId);
            await using var stream = File.OpenWrite(filepath);
            JsonSerializer.Serialize(stream, data, JsonSerializerOptions);
            stream.Write("\n"u8);
        }
    }

    private DirectoryInfo GetPatchDirectory()
        => options.GetDirectory(DataDirectory.JMdict).CreateSubdirectory("patches");

    private string GetDataFilePath(string parentPath, int patchId)
    {
        var subdir = $"{patchId / 1000}";
        var filename = $"{patchId % 1000:D3}.json";
        return Path.Join(parentPath, subdir, filename);
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
}
