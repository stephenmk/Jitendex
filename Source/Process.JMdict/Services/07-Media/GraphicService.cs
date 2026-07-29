// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, GraphicService.cs, is part of Jitendex.
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

using System.Collections.Frozen;
using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Jitendex.Process.JMdict.TableRows;
using Jitendex.Process.JMdict.Tables.Media;
using Microsoft.Extensions.Logging;

namespace Jitendex.Process.JMdict.Services.Media;

internal partial class GraphicService
(
    ILogger<GraphicService> logger,
    HomeContext homeContext,
    JMdictForkContext forkContext,
    GraphicTable graphicTable,
    GraphicLicenseTable licenseTable,
    SenseGraphicTable senseTable
)
{
    public void Write()
    {
        var currentSequenceDates = forkContext.Sequences
            .Select(static s => new
            {
                Key = s.Id,
                Value = s.Revisions.Max(static r => (DateOnly?)r.FileHeader.Date),
                DefaultValue = s.OriginFile.Date,
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value ?? x.DefaultValue);

        var currentPatchIds = forkContext.Patches
            .Select(static p => new { Key = p.SequenceId, Value = p.PatchId })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

        var graphics = homeContext.Graphics
            .Select(static g => new
            {
                g.Id,
                g.LicenceId,
                g.Cropped,
                g.PageUrl,
                g.FileUrl,
                g.Author,
                g.AuthorUrl,
                g.Title,
                Senses = g.Senses
                    .Select(static s => new
                    {
                        s.SequenceId,
                        s.SenseOrder,
                        s.Order,
                        s.SequenceDate,
                        s.PatchId,
                        s.GraphicId,
                    })
            });

        var graphicRows = new List<GraphicRow>();
        var senseRows = new List<SenseGraphicRow>();

        foreach (var graphic in graphics)
        {
            graphicRows.Add(new
            (
                graphic.Id,
                (int)graphic.LicenceId,
                graphic.Cropped,
                graphic.PageUrl,
                graphic.FileUrl,
                graphic.Author,
                graphic.AuthorUrl,
                graphic.Title
            ));
            foreach (var sense in graphic.Senses)
            {
                var revisionDate = currentSequenceDates[sense.SequenceId];
                if (!sense.SequenceDate.Equals(revisionDate))
                {
                    LogOutdatedRevision(graphic.Id, sense.SequenceId, revisionDate, sense.SequenceDate);
                }
                var targetPatchId = currentPatchIds.TryGetValue(sense.SequenceId, out var x) ? (int?)x : null;
                if (sense.PatchId != targetPatchId)
                {
                    LogOutdatedPatch(graphic.Id, sense.SequenceId, sense.PatchId, targetPatchId);
                }
                senseRows.Add(new
                (
                    sense.SequenceId,
                    sense.SenseOrder,
                    sense.Order,
                    sense.GraphicId
                ));
            }
        }

        var licenseRows = homeContext.GraphicLicenses
            .Select(static l => new GraphicLicenseRow((int)l.Id, l.Name, l.InfoUrl));

        licenseTable.InsertItems(forkContext, licenseRows);
        graphicTable.InsertItems(forkContext, graphicRows);
        senseTable.InsertItems(forkContext, senseRows);
    }

    [LoggerMessage(LogLevel.Warning,
    "Graphic ID {GraphicId} targets entry ID {EntryId} on date {OldDate}, but the current revision date is {NewDate}")]
    partial void LogOutdatedRevision(int graphicId, int entryId, DateOnly oldDate, DateOnly newDate);

    [LoggerMessage(LogLevel.Warning,
    "Graphic ID {GraphicId} targets entry ID {EntryId} with patch {OldPatch}, but the current patch is {NewPatch}")]
    partial void LogOutdatedPatch(int graphicId, int entryId, int? oldPatch, int? newPatch);
}
