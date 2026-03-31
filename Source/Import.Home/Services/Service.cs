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
using Jitendex.Import.Home.Services.JMdict;
using Jitendex.Import.Home.Services.Kanwa;
using Jitendex.Import.Home.Services.Media;
using Jitendex.Import.Home.Services.Tatoeba;

namespace Jitendex.Import.Home.Services;

internal sealed class Service
(
    HomeContext context,
    CharacterService characterService,
    VariantService variantService,
    CompoundService compoundService,
    CrossReferenceDataService crossReferenceDataService,
    UserService userService,
    JMdictPatchService jmdictPatchService,
    ExampleFuriganaService exampleFuriganaService,
    GraphicService graphicService,
    AudioService audioService
)
{
    public async Task ImportAsync()
    {
        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        await characterService.ImportAsync();
        await variantService.ImportAsync();
        await compoundService.ImportAsync();
        await crossReferenceDataService.ImportAsync();

        await userService.ImportAsync();
        await jmdictPatchService.ImportAsync();
        await exampleFuriganaService.ImportAsync();
        await graphicService.ImportAsync();
        await audioService.ImportAsync();

        transaction.Commit();
        context.ExecuteVacuum();
    }

    public async Task ExportAsync()
    {
        await characterService.ExportAsync();
        await variantService.ExportAsync();
        await compoundService.ExportAsync();
        await crossReferenceDataService.ExportAsync();

        await userService.ExportAsync();
        await jmdictPatchService.ExportAsync();
        await exampleFuriganaService.ExportAsync();
        await graphicService.ExportAsync();
        await audioService.ExportAsync();
    }
}
