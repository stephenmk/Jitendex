// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Service.cs, is part of Jitendex.
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

using Jitendex.Data.Tatoeba;
using Jitendex.Forks.Tatoeba.Services;
using Microsoft.Extensions.Logging;

namespace Jitendex.Forks.Tatoeba;

internal sealed class Service
(
    ILogger<Service> logger,
    TatoebaForkContext forkContext,
    DatabaseCopyService databaseCopier,
    EntryLinkService entryLinkService,
    ExampleFuriganaService furiganaService
)
{
    public void Run()
    {
        forkContext.RecreateDatabase();

        using var forkTransaction = forkContext.Database.BeginTransaction();

        logger.LogInformation("Copying data from the Tatoeba database file.");
        databaseCopier.CopyDataFromTatoeba();

        logger.LogInformation("Linking examples to JMdict entries.");
        entryLinkService.Write();

        logger.LogInformation("Adding example sentence furigana.");
        furiganaService.Write();

        forkTransaction.Commit();

        forkContext.ExecuteVacuum();
    }
}
