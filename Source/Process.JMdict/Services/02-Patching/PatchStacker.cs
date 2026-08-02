// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, PatchStacker.cs, is part of Jitendex.
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

using Jitendex.Data.Home;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Process.JMdict.Services.Patching;

internal sealed class PatchStacker(HomeContext context)
{
    public IEnumerable<Stack<PatchData>> EnumeratePatchStacks()
    {
        var approvals = context.JMdictPatchApprovals
            .OrderByDescending(static a => a.CreatedAt)
            .Select(static a => new
            {
                a.PatchId,
                a.CreatedAt,
            });

        var seenSequenceIds = new HashSet<int>();

        foreach (var approval in approvals)
        {
            var patch = QueryPatch(context, approval.PatchId);
            var sequenceId = patch.SequenceId;

            if (!seenSequenceIds.Add(sequenceId))
                continue;

            var recalledAt = QueryLatestRecall(context, approval.PatchId);
            if (recalledAt.HasValue && approval.CreatedAt <= recalledAt.Value)
                continue;

            var stack = new Stack<PatchData>();
            while (patch is not null)
            {
                stack.Push(patch);
                patch = patch.PreviousPatchId.HasValue
                    ? QueryPatch(context, patch.PreviousPatchId.Value)
                    : null;
            }

            yield return stack;
        }
    }

    private static readonly Func<HomeContext, int, PatchData> QueryPatch
        = EF.CompileQuery(
            static (HomeContext ctx, int patchId) =>
                ctx.JMdictPatches
                    .AsSplitQuery()
                    .Where(p => p.Id == patchId)
                    .Select(static p => new PatchData
                    (
                        p.Id,
                        p.SequenceId,
                        p.SequenceDate,
                        p.PreviousPatchId,
                        Revision: p.Revision != null
                            ? p.Revision.JsonDiff
                            : null,
                        Graphics: p.Graphics
                            .OrderBy(static g => g.Order)
                            .Select(static g => new GraphicData(g.Operation, g.SenseOrder, g.GraphicId))
                            .ToList()
                    ))
                    .First());

    private static readonly Func<HomeContext, int, DateTime?> QueryLatestRecall
        = EF.CompileQuery(
            static (HomeContext ctx, int patchId) =>
                ctx.JMdictPatchRecalls
                    .Where(r => r.PatchId == patchId)
                    .Max(r => (DateTime?)r.CreatedAt));
}
