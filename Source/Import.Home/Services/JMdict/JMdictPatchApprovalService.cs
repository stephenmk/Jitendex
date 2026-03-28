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

internal sealed class JMdictPatchApprovalService
(
    HomeContext context,
    ServiceOptions options,
    JMdictPatchApprovalTable table
)
{
    public async Task ImportAsync()
    {
        var filePath = GetFilePath();
        await using var stream = File.OpenRead(filePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<int, PatchApprovalData[]>>(stream) ?? [];

        var rows = new List<JMdictPatchApprovalRow>();

        foreach (var (patchId, approvals) in data)
        {
            foreach (var approval in approvals)
            {
                rows.Add(new(patchId, approval.ApproverId, approval.CreatedAt));
            }
        }

        table.InsertItems(context, rows);
    }

    public async Task ExportAsync()
    {
        var data = context.JMdictPatchApprovals
            .OrderBy(static x => x.PatchId)
            .GroupBy(static x => x.PatchId)
            .Select(static group => new
            {
                group.Key,
                Value = group
                    .OrderBy(static x => x.CreatedAt)
                    .Select(static x => new PatchApprovalData(x.ApproverId, x.CreatedAt))
                    .ToArray()
            })
            .ToDictionary(static x => x.Key, static x => x.Value);

        var filepath = GetFilePath();
        await using var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, data, JsonSerializerOptions);
    }

    private string GetFilePath()
        => Path.Join
        (
            options.GetJMdictDirectory().FullName,
            "patch_approvals.json"
        );

    private readonly static JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        Encoder = JavaScriptEncoder.Default,
    };

    private readonly record struct PatchApprovalData
    (
        int ApproverId,
        DateTime CreatedAt
    );
}
