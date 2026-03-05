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

using Jitendex.MiscData.ImportExport.Furigana;
using Jitendex.MiscData.ImportExport.JMdict;

namespace Jitendex.MiscData.ImportExport;

internal sealed class Service
(
    MiscDataContext context,
    CharacterService characterService,
    CompoundService compoundService,
    CrossReferenceDataService crossReferenceDataService
)
{
    public async Task ImportAsync()
    {
        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        await characterService.ImportAsync();
        await compoundService.ImportAsync();
        await crossReferenceDataService.ImportAsync();

        transaction.Commit();
        context.ExecuteVacuum();
    }

    public async Task ExportAsync()
    {
        await characterService.ExportAsync();
        await compoundService.ExportAsync();
        await crossReferenceDataService.ExportAsync();
    }
}
