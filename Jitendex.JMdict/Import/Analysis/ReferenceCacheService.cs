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
using Jitendex.AppDirectory;

namespace Jitendex.JMdict.Import.Analysis;

internal sealed class ReferenceCacheService(JmdictContext context)
{
    public async Task<IReadOnlyDictionary<string, int?>> ImportAsync(DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir);
        await using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<Dictionary<string, int?>>(stream) ?? [];
    }

    public async Task ExportAsync(DirectoryInfo? dataDir)
    {
        var dictionary = context.CrossReferences
            .Where(static x => x.IsAmbiguous == true)
            .ToDictionary
            (
                keySelector: static x => x.ToExportKey(),
                elementSelector: static x => x.RefEntryId
            );

        var filePath = GetJsonFilePath(dataDir);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        await using var stream = File.OpenWrite(filePath);
        await JsonSerializer.SerializeAsync(stream, dictionary, GetJsonSerializerOptions());
    }

    private string GetJsonFilePath(DirectoryInfo? dataDir)
    {
        dataDir ??= DataHome.Get(DataSubdirectory.JitendexDataDirectory);
        return Path.Join(dataDir.FullName, "jmdict", "cross_reference_sequences.json");
    }

    private static JsonSerializerOptions GetJsonSerializerOptions() => new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
}
