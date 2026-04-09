/*
Copyright (c) 2025-2026 Stephen Kraus
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

using Microsoft.Extensions.Logging;
using Jitendex.Data.Export;
using Jitendex.Export.Base.Services;

namespace Jitendex.Export.Base;

internal sealed class Service
(
    ILogger<Service> logger,
    ExportContext context,
    HeadwordService headwordService,
    TermService termService,
    JMdictTermService jmdictTermService
)
{
    public void Run()
    {
        logger.LogInformation("Initializing database file.");
        context.RecreateDatabase();

        using var exportTransaction = context.Database.BeginTransaction();

        logger.LogInformation("Importing headwords.");
        headwordService.Write();

        logger.LogInformation("Importing Terms.");
        termService.Write();

        logger.LogInformation("Importing JMdict term data.");
        jmdictTermService.Write();

        exportTransaction.Commit();

        logger.LogInformation("Finished.");
    }
}
