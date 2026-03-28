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
using Jitendex.Import.Home.Models;
using Jitendex.Import.Home.Tables.JMdict;

namespace Jitendex.Import.Home.Services.JMdict;

internal sealed class JMdictPatchService
(
    HomeContext context,
    ServiceOptions options,
    JMdictPatchTable table
)
{
    public async Task ImportAsync()
    {
        var patches = new Dictionary<int, string>();

        foreach (var directory in GetPatchDirectory().EnumerateDirectories())
        {
            var idRange = 1000 * int.Parse(directory.Name);
            foreach (var file in directory.EnumerateFiles())
            {
                var extIndex = file.Name.IndexOf('.');
                var id = idRange + int.Parse(file.Name[..extIndex]);
                using var text = file.OpenText();
                patches[id] = await text.ReadToEndAsync();
            }
        }

        var metadataPath = GetMetadataFilePath();
        await using var stream = File.OpenRead(metadataPath);
        var metadataDictionary = await JsonSerializer.DeserializeAsync<Dictionary<int, PatchMetadata>>(stream) ?? [];

        var rows = new List<JMdictPatchRow>();

        foreach (var (patchId, metadata) in metadataDictionary)
        {
            rows.Add(new
            (
                Id: patchId,
                metadata.SequenceId,
                metadata.SequenceDate,
                metadata.CreatedAt,
                metadata.AuthorId,
                metadata.AuthorComment,
                metadata.PreviousPatchId,
                Json: patches[patchId]
            ));
        }

        table.InsertItems(context, rows);
    }

    public async Task ExportAsync()
    {
        await ExportMetadataAsync();
        await ExportDataAsync();
    }

    private async Task ExportMetadataAsync()
    {
        var metadata = context.JMdictPatches
            .OrderBy(static x => x.Id)
            .Select(static x => new
            {
                Key = x.Id,
                Value = new PatchMetadata
                (
                    x.SequenceId,
                    x.SequenceDate,
                    x.CreatedAt,
                    x.AuthorId,
                    x.AuthorComment,
                    x.PreviousPatchId
                )
            })
            .ToDictionary(static x => x.Key, static x => x.Value);

        var metadataPath = GetMetadataFilePath();
        await using var stream = new FileStream(metadataPath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, metadata, JsonSerializerOptions);
    }

    private async Task ExportDataAsync()
    {
        var data = context.JMdictPatches
            .OrderBy(static x => x.Id)
            .Select(static x => new { Key = x.Id, Value = x.Json })
            .ToDictionary(static x => x.Key, static x => x.Value);

        var patchDir = GetPatchDirectory();
        if (patchDir.Exists)
        {
            patchDir.Delete(recursive: true);
        }
        patchDir.Create();

        int directoryCount = (data.Count / 1000) + 1;
        for (int i = 0; i < directoryCount; i++)
        {
            patchDir.CreateSubdirectory($"{i}");
        }

        foreach (var (id, json) in data)
        {
            var subdir = $"{id / 1000}";
            var filename = $"{id % 1000:D3}.json";
            var filepath = Path.Join(patchDir.FullName, subdir, filename);
            await using var stream = File.OpenWrite(filepath);
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);
        }
    }

    private string GetMetadataFilePath()
        => Path.Join
        (
            options.GetJMdictDirectory().FullName,
            "patch_metadata.json"
        );

    private DirectoryInfo GetPatchDirectory()
        => options.GetJMdictDirectory().CreateSubdirectory("patches");

    private readonly static JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.Default,
    };

    private sealed record PatchMetadata
    (
        int SequenceId,
        DateOnly SequenceDate,
        DateTime CreatedAt,
        int AuthorId,
        string AuthorComment,
        int? PreviousPatchId
    );
}
