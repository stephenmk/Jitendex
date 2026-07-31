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

using System.Text.Json;
using Jitendex.Data.Home;
using Jitendex.Data.Home.Entities;
using Jitendex.Data.JMdict;
using Jitendex.Data.JMdict.Mappers;
using Jitendex.Dto.JMdict;
using Jitendex.MinimalJsonDiff;

namespace Jitendex.Process.JMdict.Services.Patching;

internal sealed class PatchRebaser(HomeContext context, JMdictContext jmdictContext)
{
    public void Write(SequenceDto newSequence, DateOnly sequenceDate)
    {
        var author = GetAutomatedUser();

        if (AnyExistingPatches(newSequence.Id, sequenceDate, author.Id))
        {
            return;
        }

        var oldSequences = SequenceDictionaryLoader.Load(jmdictContext, [newSequence.Id]);
        var oldSequence = oldSequences[newSequence.Id];

        var json = JsonDiffer.DiffToUtf8Bytes(oldSequence, newSequence, JsonSerializerOptions);
        var comment = $"Rebasing and squashing patches onto new sequence version from date {sequenceDate}";

        context.JMdictPatches.Add(new()
        {
            Id = default,
            SequenceId = newSequence.Id,
            SequenceDate = sequenceDate,
            CreatedAt = DateTime.UtcNow,
            AuthorId = author.Id,
            AuthorComment = comment,
            PreviousPatchId = null,
            JsonDiff = json,
            Author = author,
        });

        context.SaveChanges();
    }

    private User GetAutomatedUser()
    {
        const string userName = "PatchRebaser";

        var query = context.Users
            .Where(static u => u.Name == userName);

        if (query.FirstOrDefault() is User user)
        {
            return user;
        }

        user = new User()
        {
            Id = default,
            Name = userName,
        };

        context.Users.Add(user);

        return user;
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
    };

    private bool AnyExistingPatches(int sequenceId, DateOnly sequenceDate, int authorId)
        => context.JMdictPatches
            .Where(p => p.SequenceId == sequenceId)
            .Where(p => p.SequenceDate == sequenceDate)
            .Where(p => p.AuthorId == authorId)
            .Any();
}
