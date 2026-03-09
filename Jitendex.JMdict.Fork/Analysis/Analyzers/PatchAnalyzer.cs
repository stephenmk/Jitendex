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
        var seqToLatestFile = GetSequenceToLatestFile();
        var sequences = DtoMapper.LoadSequencesWithoutRevisions(jmdictContext, patchStacks.Keys);

        foreach (var (seqId, stack) in patchStacks)
        {
            var sequence = sequences[seqId];
            var latestFileId = seqToLatestFile[seqId];
            var patchedSequence = ApplyPatchStack(stack, sequence, latestFileId);
            // TODO: Write new sequence to DB.
        }
    }

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
                        LogInvalidFileDate(patch.Id, patch.SequenceDate);
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

    private SequenceDto? ApplyPatchStack(Stack<PatchData> stack, SequenceDto sequence, int fileId)
    {
        while (stack.Count > 0)
        {
            var patch = stack.Pop();

            if (patch.FileId != fileId)
            {
                LogOutdatedPatch(patch.Id, sequence.Id, patch.FileId, fileId);
            }

            var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<SequenceDto>>(patch.Json);
            if (patchDoc is null)
            {
                LogDeserializationError(patch.Id);
                return null;
            }

            var patchFailure = false;
            patchDoc.ApplyTo(sequence, jsonPatchError =>
            {
                patchFailure = true;
                LogPatchFailure(patch.Id, jsonPatchError.ErrorMessage);
            });

            if (patchFailure)
            {
                return null;
            }
        }
        // TODO: Write new patch to home data if old one was outdated.
        return sequence;
    }

    [LoggerMessage(LogLevel.Error, "Invalid sequence date {Date} in patch ID #{Id}")]
    private partial void LogInvalidFileDate(int id, DateOnly date);

    [LoggerMessage(LogLevel.Warning,
    "Patch #{PatchId} for sequence #{SeqId} targets file version #{PatchFileId}, but the the current version is #{SeqFileId}")]
    private partial void LogOutdatedPatch(int patchId, int seqId, int patchFileId, int seqFileId);

    [LoggerMessage(LogLevel.Warning, "Unable to apply patch ID #{Id}: `{Message}`")]
    private partial void LogPatchFailure(int id, string message);

    [LoggerMessage(LogLevel.Error, "Unable to deserialize patch ID #{Id}")]
    private partial void LogDeserializationError(int id);
}
