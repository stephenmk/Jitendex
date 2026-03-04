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
using Jitendex.JapaneseTextUtils;
using Jitendex.MiscData.ImportExport.Models;
using Jitendex.MiscData.ImportExport.Tables.JMdict;

namespace Jitendex.MiscData.ImportExport.JMdict;

internal sealed class CrossReferenceDataService
(
    MiscDataContext context,
    ServiceOptions options,
    CrossReferenceSequenceTable crossReferenceSequenceTable
)
{
    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, int?>>(stream) ?? [];

        var rows = new List<CrossReferenceSequenceRow>();

        foreach (var (key, value) in data)
        {
            rows.Add(ParseData(key, value));
        }

        crossReferenceSequenceTable.InsertItems(context, rows);
    }

    public async Task ExportAsync()
    {
        var dictionary = context.CrossReferenceSequences
            .ToDictionary
            (
                keySelector: static x => x.ToExportKey(),
                elementSelector: static x => x.RefEntryId
            );

        var filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        await using var stream = File.OpenWrite(filePath);
        await JsonSerializer.SerializeAsync(stream, dictionary, JsonSerializerOptions);
    }

    private string GetJsonFilePath()
        => Path.Join
        (
            options.GetJMdictDirectory().FullName,
            "cross_reference_sequences.json"
        );

    private readonly static JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private CrossReferenceSequenceRow ParseData(string key, int? value)
    {
        var split = key.Split('・');
        if (split is { Length: < 3 or > 5 })
        {
            throw new Exception($"Malformatted key: `{key}`");
        }

        var entryId = int.Parse(split[0]);
        var senseNumber = int.Parse(split[1]);
        string? refKanjiFormText = null;
        string? refReadingText = null;
        int? refSenseNumber = null;

        if (split.Length == 3)
        {
            if (split[2].IsAllKana())
            {
                refReadingText = split[2];
            }
            else
            {
                refKanjiFormText = split[2];
            }
        }
        else if (split.Length == 4)
        {
            if (int.TryParse(split[3], out var number))
            {
                if (split[2].IsAllKana())
                {
                    refReadingText = split[2];
                }
                else
                {
                    refKanjiFormText = split[2];
                }
                refSenseNumber = number;
            }
            else
            {
                refKanjiFormText = split[2];
                refReadingText = split[3];
            }
        }
        else if (split.Length == 5)
        {
            refKanjiFormText = split[2];
            refReadingText = split[3];
            refSenseNumber = int.Parse(split[4]);
        }

        return new CrossReferenceSequenceRow
        (
            EntryId: entryId,
            SenseNumber: senseNumber,
            RefKanjiFormText: refKanjiFormText,
            RefReadingText: refReadingText,
            RefSenseNumber: refSenseNumber,
            RefEntryId: value
        );
    }
}
