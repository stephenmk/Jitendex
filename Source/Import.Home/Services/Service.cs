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

using Jitendex.Data.Home;
using Jitendex.Import.Home.Services.Attribution;
using Jitendex.Import.Home.Services.Imagery;
using Jitendex.Import.Home.Services.JMdict;
using Jitendex.Import.Home.Services.Kanwa;
using Jitendex.Import.Home.Services.Sound;
using Jitendex.Import.Home.Services.Tatoeba;

namespace Jitendex.Import.Home.Services;

internal sealed class Service
(
    HomeContext context,

    UserService userService,
    LicenseService licenseService,

    GraphicService graphicService,

    JMdictPatchService jmdictPatchService,
    JMdictPatchApprovalService jmdictPatchApprovalService,
    JMdictPatchRecallService jmdictPatchRecallService,
    TrademarkService trademarkService,

    CharacterService characterService,
    VariantService variantService,
    CompoundService compoundService,
    ExampleFuriganaService exampleFuriganaService,
    AudioService audioService
)
{
    public async Task ImportAsync()
    {
        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        await userService.ImportAsync();
        await licenseService.ImportAsync();

        await graphicService.ImportAsync();

        await jmdictPatchService.ImportAsync();
        await jmdictPatchApprovalService.ImportAsync();
        await jmdictPatchRecallService.ImportAsync();
        await trademarkService.ImportAsync();

        await characterService.ImportAsync();
        await variantService.ImportAsync();
        await compoundService.ImportAsync();

        await exampleFuriganaService.ImportAsync();
        await audioService.ImportAsync();

        transaction.Commit();
        context.ExecuteVacuum();
    }

    public async Task ExportAsync()
    {
        await userService.ExportAsync();
        await licenseService.ExportAsync();

        await graphicService.ExportAsync();

        await jmdictPatchService.ExportAsync();
        await jmdictPatchApprovalService.ImportAsync();
        await jmdictPatchRecallService.ImportAsync();
        await trademarkService.ExportAsync();

        await characterService.ExportAsync();
        await variantService.ExportAsync();
        await compoundService.ExportAsync();

        await exampleFuriganaService.ExportAsync();
        await audioService.ExportAsync();
    }
}
