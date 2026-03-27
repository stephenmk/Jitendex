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
using Jitendex.Import.Home.Models;
using Jitendex.Import.Home.Tables.Tatoeba;

namespace Jitendex.Import.Home.Services.Tatoeba;

internal sealed class ExampleFuriganaService
(
    HomeContext context,
    ServiceOptions options,
    ExampleTable exampleTable,
    ExampleFuriganaTable furiganaTable
)
{
    public async Task ImportAsync()
    {
        var exampleRows = new List<ExampleRow>();
        var furiganaRows = new List<ExampleFuriganaRow>();

        var filePath = GetCsvFilePath();
        await using var stream = File.OpenRead(filePath);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync() is string line)
        {
            int i = line.IndexOf(',');
            int exampleId = int.Parse(line.AsSpan(..i));
            exampleRows.Add(new(exampleId));
            furiganaRows.AddRange(ParseLine(exampleId, line.AsSpan(i + 1)));
        }

        exampleTable.InsertItems(context, exampleRows);
        furiganaTable.InsertItems(context, furiganaRows);
    }

    private List<ExampleFuriganaRow> ParseLine(int exampleId, ReadOnlySpan<char> line)
    {
        var rows = new List<ExampleFuriganaRow>();

        var ranges = new List<ExampleRange>();
        var currentType = RangeType.Default;
        int start = 0;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c is '[' or '|' or ']')
            {
                if (start < i)
                {
                    ranges.Add(new(currentType, new(start, i)));
                }
                start = i + 1;
            }

            currentType = c switch
            {
                '[' => RangeType.Base,
                '|' => RangeType.Ruby,
                ']' => RangeType.Default,
                _ => currentType
            };
        }

        ranges.Add(new(currentType, new(start, line.Length)));

        for (int i = 0; i < ranges.Count; i++)
        {
            var baseText = new string(line[ranges[i].Value]);
            if (ranges[i].Type == RangeType.Default)
            {
                rows.Add(new(exampleId, rows.Count, baseText, null));
                continue;
            }
            var rubyText = new string(line[ranges[++i].Value]);
            rows.Add(new(exampleId, rows.Count, baseText, rubyText));
        }

        return rows;
    }

    private enum RangeType
    {
        Default,
        Base,
        Ruby,
    }

    private readonly record struct ExampleRange(RangeType Type, Range Value);

    public async Task ExportAsync()
    {
        var filePath = GetCsvFilePath();

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        await using var stream = File.OpenWrite(filePath);
        using var writer = new StreamWriter(stream);

        var query = context.Examples
            .OrderBy(static e => e.Id)
            .Select(static e => new
            {
                e.Id,
                Parts = e.Furigana
                    .OrderBy(static f => f.Order)
                    .Select(static f => new { f.BaseText, f.RubyText })
            });

        foreach (var line in query)
        {
            writer.Write(line.Id);
            writer.Write(',');
            foreach (var part in line.Parts)
            {
                if (part.RubyText is null)
                {
                    writer.Write(part.BaseText);
                }
                else
                {
                    writer.Write($"[{part.BaseText}|{part.RubyText}]");
                }
            }
            writer.Write('\n');
        }
    }

    private string GetCsvFilePath()
        => Path.Join
        (
            options.GetTatoebaDirectory().FullName,
            "example_sentence_furigana.csv"
        );
}
