// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 03-JMdictPatchApprovalService.cs, is part of Jitendex.
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

// using System.Globalization;
using Jitendex.Data.Home;
using Jitendex.Import.Home.TableRows;
using Jitendex.Import.Home.Tables.JMdict;

namespace Jitendex.Import.Home.Services.JMdict;

internal sealed class JMdictPatchApprovalService
(
    HomeContext context,
    ServiceOptions options,
    JMdictPatchApprovalTable approvalTable
)
{
    public async Task ImportAsync()
    {
        var filePath = GetTsvFilePath();
        using var reader = File.OpenText(filePath);
        var rows = new List<JMdictPatchApprovalRow>();

        while (await reader.ReadLineAsync() is string line)
        {
            var split = line.Split('\t');
            rows.Add(new(
                PatchId: int.Parse(split[2]),
                ApproverId: int.Parse(split[1]),
                CreatedAt: DateTime.Parse(split[0])
            ));
        }

        approvalTable.InsertItems(context, rows);
    }

    public async Task ExportAsync()
    {
        var query = context.JMdictPatchApprovals
            .OrderBy(static x => x.CreatedAt)
            .Select(static x => new
            {
                x.PatchId,
                x.ApproverId,
                x.CreatedAt
            });

        var filePath = GetTsvFilePath();
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await using var writer = new StreamWriter(stream);

        foreach (var x in query)
            writer.WriteLine($"{x.CreatedAt:o}\t{x.ApproverId}\t{x.PatchId}");
    }

    private string GetTsvFilePath()
        => Path.Join
        (
            options.GetDirectory(DataDirectory.JMdict).CreateSubdirectory("patches").FullName,
            "approvals.tsv"
        );

    // private DateTime ParseUtcDateTime(string text)
    //     => DateTime.Parse
    //     (
    //         text,
    //         CultureInfo.InvariantCulture,
    //         DateTimeStyles.AssumeUniversal
    //     );
}
