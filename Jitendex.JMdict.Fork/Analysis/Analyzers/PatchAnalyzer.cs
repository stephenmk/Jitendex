/*
Copyright (c) 2025-2026 Stephen Kraus
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
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.Extensions.Logging;
using Jitendex.HomeData;
using Jitendex.Dto.JMdict;

namespace Jitendex.JMdict.Fork.Analysis.Analyzers;

internal partial class PatchAnalyzer
(
    ILogger<PatchAnalyzer> logger,
    JmdictContext jmdictContext,
    JMdictForkContext forkContext,
    HomeDataContext homeContext
)
{
    private sealed record PatchData(int Id, int FileId, string Json);

    public void Analyze()
    {
        var patchStacks = GetPatchStacks();
        var x = GetSequenceToLatestFile();
        var sequences = DtoMapper.LoadSequencesWithoutRevisions(jmdictContext, patchStacks.Keys);

        foreach (var (id, patchStack) in patchStacks)
        {
            var sequence = sequences[id];
            var latestFileId = x[id];

            while (patchStack.Count > 0)
            {
                var patch = patchStack.Pop();

                if (patch.FileId != latestFileId)
                {
                    // TODO: Warn
                }

                var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<SequenceDto>>(patch.Json);
                if (patchDoc is null)
                {
                    logger.LogError("Unable to deserialize patch ID {Id}", patch.Id);
                    return;
                }
                patchDoc.ApplyTo(sequence);
            }
        }
    }

    private FrozenDictionary<int, int> GetSequenceToLatestFile()
        => forkContext.Sequences
            .Select(static s => new
            {
                Key = s.Id,
                DefaultValue = s.OriginFileId,
                Value = s.Revisions
                    .OrderByDescending(static r => r.FileHeader.Date)
                    .Select(static r => (int?)r.FileHeaderId)
                    .FirstOrDefault()
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value ?? x.DefaultValue);

    private Dictionary<int, Stack<PatchData>> GetPatchStacks()
    {
        var sequences = new Dictionary<int, Stack<PatchData>>();

        var dateToFileId = forkContext.FileHeaders
            .Select(static f => new { Key = f.Date, Value = f.Id })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        var patches = homeContext.JMdictPatches
            .Select(static p => new
            {
                Key = p.Id,
                Value = new { p.Id, p.SequenceId, p.SequenceDate, p.Json, p.PreviousPatchId }
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        var approvedPatchIds = homeContext.JMdictPatchApprovals
            .OrderByDescending(static a => a.CreatedAt)
            .Select(static a => a.PatchId);

        foreach (var patchId in approvedPatchIds)
        {
            var patch = patches[patchId];
            var sequenceId = patch.SequenceId;

            if (!sequences.ContainsKey(sequenceId))
            {
                var stack = new Stack<PatchData>();
                while (patch is not null)
                {
                    if (!dateToFileId.TryGetValue(patch.SequenceDate, out var fileId))
                    {
                        logger.LogError("Invalid sequence date {Date}", patch.SequenceDate);
                        return [];
                    }
                    var patchData = new PatchData(patch.Id, fileId, patch.Json);
                    stack.Push(patchData);
                    patch = patch.PreviousPatchId.HasValue
                        ? patches[patch.PreviousPatchId.Value]
                        : null;
                }
                sequences[sequenceId] = stack;
            }
        }

        return sequences;
    }
}
