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

using System.Collections.Frozen;
using System.Text.Json;
using Jitendex.Data.Home;
using Jitendex.Data.Home.Entities.Graphics;
using Jitendex.Import.Home.Models;
using Jitendex.Import.Home.Tables.Graphics;
using static Jitendex.Data.Home.Entities.Graphics.GraphicLicenceId;

namespace Jitendex.Import.Home.Services.Graphics;

internal sealed class GraphicService
(
    HomeContext context,
    ServiceOptions options,
    GraphicTable graphicTable,
    SenseGraphicTable senseTable,
    GraphicLicenseTable licenseTable
)
{
    private sealed record GraphicObject
    (
        string? Title,
        string PageUrl,
        string FileUrl,
        string Author,
        string? AuthorUrl,
        string License,
        EntryData[] Entries
    );

    private sealed record EntryData
    (
        int EntryId,
        int SenseOrder,
        int Order,
        DateOnly SequenceDate,
        int? PatchId
    );

    public async Task ImportAsync()
    {
        var filePath = GetJsonFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<int, GraphicObject>>(stream, ReadOptions) ?? [];

        var graphicRows = new List<GraphicRow>();
        var senseRows = new List<SenseGraphicRow>();

        foreach (var (id, obj) in data)
        {
            graphicRows.Add(new
            (
                Id: id,
                LicenseId: LicenseNameToId[obj.License],
                Cropped: false,
                PageUrl: obj.PageUrl,
                FileUrl: obj.FileUrl,
                Author: obj.Author,
                AuthorUrl: obj.AuthorUrl,
                Title: obj.Title
            ));
            foreach (var entry in obj.Entries)
            {
                senseRows.Add(new
                (
                    SequenceId: entry.EntryId,
                    SenseOrder: entry.SenseOrder,
                    Order: entry.Order,
                    SequenceDate: entry.SequenceDate,
                    PatchId: entry.PatchId,
                    GraphicId: id
                ));
            }
        }

        licenseTable.InsertItems(context, GetLicenseRows());
        graphicTable.InsertItems(context, graphicRows);
        senseTable.InsertItems(context, senseRows);
    }

    public async Task ExportAsync()
    {
        var data = context.Graphics
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
                    License: g.License.Name,
                    Entries: g.Senses
                        .Select(static s => new EntryData
                        (
                            EntryId: s.SequenceId,
                            SenseOrder: s.SenseOrder,
                            Order: s.Order,
                            SequenceDate: s.SequenceDate,
                            PatchId: s.PatchId
                        ))
                        .ToArray()
                )
            })
            .ToDictionary(static x => x.Key, static x => x.Value);

        var filePath = GetJsonFilePath();
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, data, WriteOptions);
    }

    private readonly static JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly static JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
    };

    private string GetJsonFilePath()
        => Path.Join
        (
            options.GetGraphicDirectory().FullName,
            "index.json"
        );

    private static IEnumerable<GraphicLicenseRow> GetLicenseRows()
    {
        foreach (var id in Enum.GetValues<GraphicLicenceId>())
        {
            yield return new GraphicLicenseRow((int)id, IdToLicenseName(id), IdToLicenseUrl(id));
        }
    }

    #pragma warning disable format
    private static string IdToLicenseName(GraphicLicenceId id) => id switch
    {
        PublicDomain                   => "public domain",
        CreativeCommonsZero1_0         => "CC0 1.0",
        CreativeCommonsBy1_0           => "CC BY 1.0",
        CreativeCommonsBy2_0           => "CC BY 2.0",
        CreativeCommonsBy3_0           => "CC BY 3.0",
        CreativeCommonsBy4_0           => "CC BY 4.0",
        CreativeCommonsByShareAlike1_0 => "CC BY-SA 1.0",
        CreativeCommonsByShareAlike2_0 => "CC BY-SA 2.0",
        CreativeCommonsByShareAlike2_5 => "CC BY-SA 2.5",
        CreativeCommonsByShareAlike3_0 => "CC BY-SA 3.0",
        CreativeCommonsByShareAlike4_0 => "CC BY-SA 4.0",
        _                              => throw new ArgumentOutOfRangeException(nameof(id))
    };

    private static string IdToLicenseUrl(GraphicLicenceId id) => id switch
    {
        PublicDomain                   => "https://en.wikipedia.org/wiki/Public_domain",
        CreativeCommonsZero1_0         => "https://creativecommons.org/publicdomain/zero/1.0/deed.en",
        CreativeCommonsBy1_0           => "https://creativecommons.org/licenses/by/1.0/deed.en",
        CreativeCommonsBy2_0           => "https://creativecommons.org/licenses/by/2.0/deed.en",
        CreativeCommonsBy3_0           => "https://creativecommons.org/licenses/by/3.0/deed.en",
        CreativeCommonsBy4_0           => "https://creativecommons.org/licenses/by/4.0/deed.en",
        CreativeCommonsByShareAlike1_0 => "https://creativecommons.org/licenses/by-sa/1.0/",
        CreativeCommonsByShareAlike2_0 => "https://creativecommons.org/licenses/by-sa/2.0/",
        CreativeCommonsByShareAlike2_5 => "https://creativecommons.org/licenses/by-sa/2.5/",
        CreativeCommonsByShareAlike3_0 => "https://creativecommons.org/licenses/by-sa/3.0/",
        CreativeCommonsByShareAlike4_0 => "https://creativecommons.org/licenses/by-sa/4.0/",
        _                              => throw new ArgumentOutOfRangeException(nameof(id))
    };
    #pragma warning restore format

    private static readonly FrozenDictionary<string, int> LicenseNameToId = Enum
        .GetValues<GraphicLicenceId>()
        .Select(static id => new
        {
            Key = IdToLicenseName(id),
            Value = (int)id,
        })
        .ToFrozenDictionary(static x => x.Key, static x => x.Value);
}
