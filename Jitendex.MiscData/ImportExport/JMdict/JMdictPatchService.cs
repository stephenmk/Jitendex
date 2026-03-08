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
using Jitendex.MiscData.ImportExport.JMdict.Models;
using Jitendex.MiscData.ImportExport.JMdict.Tables;

namespace Jitendex.MiscData.ImportExport.JMdict;

internal sealed class JMdictPatchService
(
    MiscDataContext context,
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
                SequenceId: metadata.SequenceId,
                SequenceDate: metadata.SequenceDate,
                CreatedAt: metadata.CreatedAt,
                AuthorId: metadata.AuthorId,
                AuthorComment: metadata.AuthorComment,
                PreviousPatchId: metadata.PreviousPatchId,
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
            .AsNoTracking()
            .OrderBy(static x => x.Id)
            .ToDictionary
            (
                keySelector: static x => x.Id,
                elementSelector: static x => new PatchMetadata
                (
                    SequenceId: x.SequenceId,
                    SequenceDate: x.SequenceDate,
                    CreatedAt: x.CreatedAt,
                    AuthorId: x.AuthorId,
                    AuthorComment: x.AuthorComment,
                    PreviousPatchId: x.PreviousPatchId
                )
            );

        var metadataPath = GetMetadataFilePath();
        if (File.Exists(metadataPath))
        {
            File.Delete(metadataPath);
        }

        await using var stream = File.OpenWrite(metadataPath);
        await JsonSerializer.SerializeAsync(stream, metadata, JsonSerializerOptions);
    }

    private async Task ExportDataAsync()
    {
        var data = context.JMdictPatches
            .AsNoTracking()
            .OrderBy(static x => x.Id)
            .ToDictionary(static x => x.Id, static x => x.Json);

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
