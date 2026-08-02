// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, PatchRebaser.cs, is part of Jitendex.
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
using Jitendex.Data.Home.Entities.Attribution;
using Jitendex.Data.Home.Entities.JMdict;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Process.JMdict.Services.Patching;

internal sealed class PatchRebaser(HomeContext context)
{
    public void Write(PatchData patchData, DateOnly newDate)
    {
        var author = GetAutomatedUser();

        if (QueryAnyExistingPatches(context, patchData.SequenceId, newDate, author.Id))
            return;

        var comment = $"Rebasing patch #{patchData.Id} onto new sequence version from date {newDate}";

        var patch = new Patch()
        {
            Id = default,
            SequenceId = patchData.SequenceId,
            SequenceDate = newDate,
            CreatedAt = DateTime.UtcNow,
            AuthorId = author.Id,
            AuthorComment = comment,
            PreviousPatchId = null,
            Author = author,
        };

        if (patchData.Revision is not null)
        {
            patch.Revision = new()
            {
                PatchId = patch.Id,
                JsonDiff = patchData.Revision,
                Patch = patch,
            };
        }

        int i = 0;
        foreach (var graphic in patchData.Graphics)
        {
            patch.Graphics.Add(new()
            {
                PatchId = patch.Id,
                Order = i++,
                Operation = graphic.Operation,
                SenseOrder = graphic.SenseOrder,
                GraphicId = graphic.GraphicId,
                Patch = patch,
            });
        }

        context.JMdictPatches.Add(patch);
        context.SaveChanges();
    }

    private User GetAutomatedUser()
    {
        const string userName = "PatchRebaser";

        var user = context.Users
            .Where(static u => u.Name == userName)
            .FirstOrDefault();

        if (user is not null)
            return user;

        user = new User()
        {
            Id = default,
            Name = userName,
        };

        context.Users.Add(user);

        return user;
    }

    private static readonly Func<HomeContext, int, DateOnly, int, bool> QueryAnyExistingPatches
        = EF.CompileQuery(
            static (HomeContext ctx, int sequenceId, DateOnly sequenceDate, int authorId) =>
                ctx.JMdictPatches
                    .Where(p => p.SequenceId == sequenceId)
                    .Where(p => p.SequenceDate == sequenceDate)
                    .Where(p => p.AuthorId == authorId)
                    .Any());
}
