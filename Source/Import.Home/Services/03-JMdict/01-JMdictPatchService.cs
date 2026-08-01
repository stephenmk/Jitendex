// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 01-JMdictPatchService.cs, is part of Jitendex.
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
using Jitendex.Import.Home.TableRows;
using Jitendex.Import.Home.Tables.JMdict;

namespace Jitendex.Import.Home.Services.JMdict;

internal sealed class JMdictPatchService
(
    HomeContext context,
    ServiceOptions options,
    JMdictPatchTable patchTable
)
{
    public async Task ImportAsync()
    {
        var indexPath = GetIndexFilePath();
        await using var stream = File.OpenRead(indexPath);
        var metadataDictionary = await JsonSerializer.DeserializeAsync<Dictionary<int, PatchMetadata>>(stream) ?? [];

        var patchRows = new List<JMdictPatchRow>();

        foreach (var (patchId, metadata) in metadataDictionary)
        {
            patchRows.Add(new
            (
                Id: patchId,
                metadata.SequenceId,
                metadata.SequenceDate,
                metadata.CreatedAt,
                metadata.AuthorId,
                metadata.AuthorComment,
                metadata.PreviousPatchId
            ));
        }

        patchTable.InsertItems(context, patchRows);
    }

    public async Task ExportAsync()
    {
        var index = context.JMdictPatches
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

        var metadataPath = GetIndexFilePath();
        await using var stream = new FileStream(metadataPath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, index, JsonSerializerOptions);
        stream.Write("\n"u8);
    }

    private string GetIndexFilePath()
        => Path.Join
        (
            options.GetDirectory(DataDirectory.JMdict).CreateSubdirectory("patches").FullName,
            "index.json"
        );

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
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
