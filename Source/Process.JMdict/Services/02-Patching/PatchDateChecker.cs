// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, PatchDateChecker.cs, is part of Jitendex.
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

using Jitendex.Data.JMdict;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jitendex.Process.JMdict.Services.Patching;

internal sealed partial class PatchDateChecker
(
    ILogger<PatchDateChecker> logger,
    JMdictContext context
)
{
    /// <summary>
    /// Check to see if the sequence date targeted by a patch is equal to the latest revision date in the database.
    /// </summary>
    /// <returns>The new sequence date if the patch date is outdated. Otherwise, null.</returns>
    public DateOnly? CheckPatchDate(PatchData patch)
    {
        var sequenceDate = QueryLatestRevisionDate(context, patch.SequenceId);
        if (sequenceDate.Equals(patch.Date))
        {
            return null;
        }
        {
            LogOutdatedPatch(patch.Id, patch.SequenceId, patch.Date, sequenceDate);
            return sequenceDate;
        }
    }

    private static readonly Func<JMdictContext, int, DateOnly> QueryLatestRevisionDate
        = EF.CompileQuery(
            static (JMdictContext ctx, int sequenceId) => ctx.Sequences
                .AsSplitQuery()
                .Where(s => s.Id == sequenceId)
                .Select(static s => s.Revisions
                    .Select(static r => r.FileHeader.Date)
                    .Append(s.OriginFile.Date)
                    .Max())
                .First());

    [LoggerMessage(LogLevel.Warning,
    "Patch ID {PatchId} for sequence #{SeqId} targets file version {PatchDate}, but the the current version is {SeqDate}")]
    partial void LogOutdatedPatch(int patchId, int seqId, DateOnly patchDate, DateOnly seqDate);
}
