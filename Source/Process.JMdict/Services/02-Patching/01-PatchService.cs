// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 01-PatchService.cs, is part of Jitendex.
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

using System.Text.Json;
using Jitendex.Data.Home.Entities.JMdict;
using Jitendex.Data.JMdict;
using Jitendex.Data.JMdict.Mappers;
using Jitendex.Dto.JMdict;
using Jitendex.Process.JMdict.Services.Patching.Helpers;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Process.JMdict.Services.Patching;

internal class PatchService
(
    JMdictContext jmdictContext,
    JMdictForkContext forkContext,
    PatchDateChecker dateChecker,
    PatchStacker patchStacker,
    PatchStackSquasher stackSquasher,
    PatchRebaser patchRebaser
)
{
    public void Write()
    {
        foreach (var stack in patchStacker.EnumeratePatchStacks())
        {
            var newPatchDate = dateChecker.CheckPatchDate(stack.Peek());

            if (GetNormalizedPatch(stack) is not PatchData patch)
                continue;

            if (newPatchDate.HasValue)
            {
                // Patch is outdated and needs to be reapproved.
                patchRebaser.Write(patch, newPatchDate.Value);
                continue;
            }

            WriteRevision(patch);
            WriteGraphics(patch);
        }

        forkContext.SaveChanges();
    }

    /// <summary>
    /// Validate and squash the patch sequence into a single patch.
    /// </summary>
    private PatchData? GetNormalizedPatch(Stack<PatchData> stack)
    {
        const string emptyErrorMessage
            = $"All collections enumerated by `{nameof(patchStacker)}` are expected to have at least one item.";

        if (!stack.Any())
            throw new InvalidOperationException(emptyErrorMessage);

        // Squash even if there's only one patch, because this also validates the patch.
        else if (stackSquasher.Squash(stack) is PatchData squashedPatch)
            return squashedPatch;

        else
            return null;
    }

    private void WriteRevision(PatchData patch)
    {
        if (patch.Revision is null)
            return;

        var sequence = SequenceLoader.LoadSequence(jmdictContext, patch.SequenceId);
        var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<SequenceDto>>(patch.Revision)!;
        patchDoc.ApplyTo(sequence);

        var patchedEntry = sequence.Entry?.ToEntry(patch.SequenceId);

        forkContext.Entries
            .Where(e => e.Id == patch.SequenceId)
            .ExecuteDelete();

        var seq = forkContext.Sequences
            .Where(s => s.Id == patch.SequenceId)
            .First();

        seq.Entry = patchedEntry;

        seq.Patch = new()
        {
            SequenceId = patch.SequenceId,
            PatchId = patch.Id,
        };
    }

    private void WriteGraphics(PatchData patch)
    {
        int i = 0;
        foreach (var graphic in patch.Graphics)
        {
            // All of the "remove" operations should have been
            // squashed in the patch squasher.
            if (graphic.Operation is not PatchGraphicOperation.Add)
                throw new InvalidDataException();

            forkContext.SenseGraphics.Add(new()
            {
                EntryId = patch.SequenceId,
                Order = i++,
                SenseOrder = graphic.SenseOrder,
                GraphicId = graphic.GraphicId,
            });
        }
    }
}
