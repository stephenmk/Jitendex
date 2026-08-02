// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, PatchStackSquasher.cs, is part of Jitendex.
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

using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using Jitendex.Data.Home.Entities.JMdict;
using Jitendex.Data.JMdict;
using Jitendex.Data.JMdict.Mappers;
using Jitendex.Dto.JMdict;
using Jitendex.MinimalJsonDiff;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.Extensions.Logging;

namespace Jitendex.Process.JMdict.Services.Patching;

internal sealed partial class PatchStackSquasher
(
    ILogger<PatchStackSquasher> logger,
    JMdictContext context
)
{
    public PatchData? SquashStack(Stack<PatchData> stack)
    {
        var sequence = SequenceLoader.LoadSequence(context, stack.Peek().SequenceId);
        var senseToGraphicIds = new Dictionary<int, List<int>>();
        var finalPatch = stack.Peek();

        while (stack.TryPop(out var patch))
        {
            finalPatch = patch;

            if (ApplyRevisionPatch(sequence, patch) is SequenceDto revisedSequence)
                sequence = revisedSequence;
            else
                return null;

            foreach (var g in patch.Graphics)
            {
                if (!senseToGraphicIds.TryGetValue(g.SenseOrder, out var ids))
                {
                    ids = [];
                    senseToGraphicIds.Add(g.SenseOrder, ids);
                }
                switch (g.Operation)
                {
                    case PatchGraphicOperation.Add:
                        ids.Add(g.GraphicId);
                        break;
                    case PatchGraphicOperation.Remove:
                        ids.Remove(g.GraphicId);
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
        }

        var oldSequence = SequenceLoader.LoadSequence(context, finalPatch.SequenceId);
        var revision = JsonDiffer.DiffToUtf8Bytes(oldSequence, sequence, JsonSerializerOptions);

        var graphics = new List<GraphicData>();
        var senseCount = sequence.Entry?.Senses.Count ?? 0;
        foreach (var (senseOrder, graphicIds) in senseToGraphicIds)
        {
            if (senseOrder >= senseCount)
            {
                LogGraphicError(finalPatch.Id, senseOrder + 1, sequence.Id);
                return null;
            }
            foreach (var graphicId in graphicIds)
            {
                graphics.Add(new(PatchGraphicOperation.Add, senseOrder, graphicId));
            }
        }

        return new
        (
            finalPatch.Id,
            finalPatch.SequenceId,
            finalPatch.Date,
            PreviousPatchId: null,
            revision,
            graphics
        );
    }

    private SequenceDto? ApplyRevisionPatch(SequenceDto sequence, PatchData patch)
    {
        if (patch.Revision is null)
            return sequence;

        var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<SequenceDto>>(patch.Revision);
        if (patchDoc is null)
        {
            LogDeserializationError(patch.Id);
            return null;
        }

        var patchError = false;
        patchDoc.ApplyTo(sequence, logErrorAction: action =>
        {
            patchError = true;
            LogRevisionError(patch.Id, action.ErrorMessage);
        });

        if (patchError)
            return null;

        return sequence;
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions
        = new()
        {
            WriteIndented = true,
            IndentSize = 4,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

    [LoggerMessage(LogLevel.Warning,
    "Unable to apply revision from patch ID #{Id}: `{Message}`")]
    partial void LogRevisionError(int id, string message);

    [LoggerMessage(LogLevel.Warning,
    "Patch ID #{Id} targets sense number {SenseNumber} in sequence {SequenceId} that does not exist.")]
    partial void LogGraphicError(int id, int senseNumber, int sequenceId);

    [LoggerMessage(LogLevel.Error, "Unable to deserialize patch ID {Id}")]
    partial void LogDeserializationError(int id);
}
