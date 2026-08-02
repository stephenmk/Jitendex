// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 02-LicenseService.cs, is part of Jitendex.
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
using Jitendex.Data.JMdict;
using Jitendex.Process.JMdict.TableRows;
using Jitendex.Process.JMdict.Tables.Media;

namespace Jitendex.Process.JMdict.Services.DatabaseCopy;

internal class LicenseService
(
    HomeContext homeContext,
    JMdictForkContext forkContext,
    GraphicLicenseTable table
)
{
    public void Write()
    {
        var rows = homeContext.Licenses
            .Select(static l => new GraphicLicenseRow
            (
                l.Id,
                l.Name,
                l.InfoUrl
            ));
        table.InsertItems(forkContext, rows);
    }
}
