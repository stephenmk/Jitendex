/*
Copyright (c) 2026 Stephen Kraus
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

using Jitendex.Data.Home;
using Jitendex.Import.Home.Services.Furigana;
using Jitendex.Import.Home.Services.JMdict;

namespace Jitendex.Import.Home.Services;

internal sealed class Service
(
    HomeDataContext context,
    CharacterService characterService,
    CompoundService compoundService,
    CrossReferenceDataService crossReferenceDataService,
    UserService userService,
    JMdictPatchService jmdictPatchService,
    JMdictPatchApprovalService jmdictPatchApprovalService
)
{
    public async Task ImportAsync()
    {
        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        await characterService.ImportAsync();
        await compoundService.ImportAsync();
        await crossReferenceDataService.ImportAsync();

        await userService.ImportAsync();
        await jmdictPatchService.ImportAsync();
        await jmdictPatchApprovalService.ImportAsync();

        transaction.Commit();
        context.ExecuteVacuum();
    }

    public async Task ExportAsync()
    {
        await characterService.ExportAsync();
        await compoundService.ExportAsync();
        await crossReferenceDataService.ExportAsync();

        await userService.ExportAsync();
        await jmdictPatchService.ExportAsync();
        await jmdictPatchApprovalService.ExportAsync();
    }
}
