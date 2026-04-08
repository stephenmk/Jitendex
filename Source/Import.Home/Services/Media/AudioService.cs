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

using Jitendex.Data.Home;
using Jitendex.Import.Home.RowModels;
using Jitendex.Import.Home.Tables.Media;

namespace Jitendex.Import.Home.Services.Media;

internal sealed class AudioService
(
    HomeContext context,
    ServiceOptions options,
    KanjiAliveAudioTable table
)
{
    public async Task ImportAsync()
    {
        var dataDirPath = GetDataDirectoryPath();
        var dataDir = new DirectoryInfo(dataDirPath);
        var fileData = new Dictionary<string, byte[]>();
        foreach (var subdir in dataDir.EnumerateDirectories())
        {
            foreach (var file in subdir.EnumerateFiles())
            {
                fileData[file.Name] = File.ReadAllBytes(file.FullName);
            }
        }

        var filePath = GetCsvFilePath();
        await using var stream = File.OpenRead(filePath);
        using var reader = new StreamReader(stream);

        var rows = new List<KanjiAliveAudioRow>();

        while (await reader.ReadLineAsync() is string line)
        {
            var split = line.Split(',');
            rows.Add(new
            (
                Filename: split[0],
                EntryId: int.Parse(split[1]),
                ReadingText: split[2],
                KanjiFormText: split[3],
                Suffix: string.IsNullOrEmpty(split[4]) ? null : split[4],
                PitchAccent: string.IsNullOrEmpty(split[5]) ? null : int.Parse(split[5]),
                FileData: fileData[split[0]]
            ));
        }

        table.InsertItems(context, rows);
    }

    public async Task ExportAsync()
    {
        var data = context.KanjiAliveAudios
            .Select(static a => new
            {
                a.Filename,
                a.EntryId,
                a.ReadingText,
                a.KanjiFormText,
                a.Suffix,
                a.PitchAccent,
            });

        var filePath = GetCsvFilePath();
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(stream);

        foreach (var x in data)
        {
            var line = $"{x.Filename},{x.EntryId},{x.ReadingText},{x.KanjiFormText},{x.Suffix},{x.PitchAccent}";
            await writer.WriteLineAsync(line);
        }
    }

    private string GetCsvFilePath()
        => Path.Join
        (
            options.GetAudioDirectory().FullName,
            "kanjialive.csv"
        );

    private string GetDataDirectoryPath()
    => Path.Join
    (
        options.GetAudioDirectory().FullName,
        "kanjialive"
    );
}
