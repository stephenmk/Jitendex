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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Jitendex.Data.JMdict.Mappers;
using Jitendex.Dto.JMdict;

namespace Jitendex.Forks.JMdict.Services.Patching;

internal partial class PatchService
(
    ILogger<PatchService> logger,
    JMdictContext jmdictContext,
    JMdictForkContext forkContext,
    HomeContext homeContext,
    PatchRebaser rebaser
)
{
    private sealed record PatchData
    (
        int Id,
        DateOnly Date,
        string Json
    );

    public void Write()
    {
        var patchStacks = GetPatchStacks();
        var seqToLatestRevisionDate = GetSequenceIdToLatestRevisionDate(patchStacks.Keys);
        var sequences = SequenceDictionaryLoader.Load(jmdictContext, patchStacks.Keys);

        foreach (var (seqId, stack) in patchStacks)
        {
            var sequence = sequences[seqId];
            var date = seqToLatestRevisionDate[seqId];
            if (ApplyPatchStack(ref sequence, date, stack) is int patchId)
            {
                var patchedEntry = sequence.Entry?.ToEntry(seqId);
                forkContext.Entries
                    .Where(e => e.Id == seqId)
                    .ExecuteDelete();
                var seq = forkContext.Sequences
                    .Where(s => s.Id == seqId)
                    .First();
                seq.Entry = patchedEntry;
                seq.Patch = new() { SequenceId = seqId, PatchId = patchId };
            }
        }

        forkContext.SaveChanges();
    }

    private Dictionary<int, Stack<PatchData>> GetPatchStacks()
    {
        var sequenceIdToStack = new Dictionary<int, Stack<PatchData>>();

        var validDates = forkContext.FileHeaders
            .Select(static f => f.Date)
            .ToFrozenSet();

        var patches = homeContext.JMdictPatches
            .Select(static p => new
            {
                Key = p.Id,
                Value = new { p.Id, p.SequenceId, p.SequenceDate, p.Json, p.PreviousPatchId }
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        var recalledPatches = homeContext.JMdictPatchRecalls
            .GroupBy(static r => r.PatchId)
            .Select(static group => new
            {
                group.Key,
                Value = group.Max(static r => r.CreatedAt),
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        var patchApprovals = homeContext.JMdictPatchApprovals
            .OrderByDescending(static a => a.CreatedAt)
            .Select(static a => new { a.PatchId, a.CreatedAt });

        foreach (var approval in patchApprovals)
        {
            if (recalledPatches.TryGetValue(approval.PatchId, out var recalledAt))
            {
                if (approval.CreatedAt < recalledAt)
                {
                    continue;
                }
            }
            var patch = patches[approval.PatchId];
            var sequenceId = patch.SequenceId;
            if (sequenceIdToStack.ContainsKey(sequenceId))
            {
                continue;
            }
            var stack = new Stack<PatchData>();
            while (patch is not null)
            {
                if (validDates.Contains(patch.SequenceDate) is false)
                {
                    LogInvalidFileDate(patch.Id, patch.SequenceDate);
                    return [];
                }
                var patchData = new PatchData(patch.Id, patch.SequenceDate, patch.Json);
                stack.Push(patchData);
                patch = patch.PreviousPatchId.HasValue
                    ? patches[patch.PreviousPatchId.Value]
                    : null;
            }
            sequenceIdToStack[sequenceId] = stack;
        }

        return sequenceIdToStack;
    }

    private FrozenDictionary<int, DateOnly> GetSequenceIdToLatestRevisionDate(IEnumerable<int> sequenceIds)
        => forkContext.Sequences
            .Where(s => sequenceIds.Contains(s.Id))
            .Select(static s => new
            {
                Key = s.Id,
                DefaultValue = s.OriginFile.Date,
                Value = s.Revisions.Max(static r => (DateOnly?)r.FileHeader.Date)
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value ?? x.DefaultValue);

    private int? ApplyPatchStack(ref SequenceDto sequence, DateOnly sequenceDate, Stack<PatchData> stack)
    {
        bool outdated = false;
        int finalPatchId = stack.Peek().Id;
        while (stack.Count > 0)
        {
            var patch = stack.Pop();
            finalPatchId = patch.Id;

            if (!sequenceDate.Equals(patch.Date))
            {
                outdated = true;
                LogOutdatedPatch(patch.Id, sequence.Id, patch.Date, sequenceDate);
            }

            var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<SequenceDto>>(patch.Json);
            if (patchDoc is null)
            {
                LogDeserializationError(patch.Id);
                return null;
            }

            var patchError = false;
            patchDoc.ApplyTo(sequence, logErrorAction: action =>
            {
                patchError = true;
                LogPatchError(patch.Id, action.ErrorMessage);
            });

            if (patchError)
            {
                return null;
            }
        }
        if (outdated)
        {
            rebaser.Write(sequence, sequenceDate);
        }
        return finalPatchId;
    }

    [LoggerMessage(LogLevel.Error, "Invalid sequence date {Date} in patch ID #{Id}")]
    private partial void LogInvalidFileDate(int id, DateOnly date);

    [LoggerMessage(LogLevel.Warning,
    "Patch ID {PatchId} for sequence #{SeqId} targets file version {PatchDate}, but the the current version is {SeqDate}")]
    private partial void LogOutdatedPatch(int patchId, int seqId, DateOnly patchDate, DateOnly seqDate);

    [LoggerMessage(LogLevel.Warning, "Unable to apply patch ID #{Id}: `{Message}`")]
    private partial void LogPatchError(int id, string message);

    [LoggerMessage(LogLevel.Error, "Unable to deserialize patch ID {Id}")]
    private partial void LogDeserializationError(int id);
}
