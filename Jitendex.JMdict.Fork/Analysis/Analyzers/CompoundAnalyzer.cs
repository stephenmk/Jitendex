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

using Jitendex.MiscData;
using Jitendex.JMdict.Fork.Analysis.Tables;

namespace Jitendex.JMdict.Fork.Analysis.Analyzers;

internal sealed class CompoundAnalyzer
(
    JMdictForkContext forkContext,
    MiscDataContext miscContext,
    CompoundTable compoundTable,
    CompoundReadingTable readingTable
)
{
    public void Analyze()
    {
        var compoundRows = miscContext.Compounds
            .Select(static x => new CompoundRow(x.Text));

        var readingRows = miscContext.CompoundReadings
            .Select(static x => new CompoundReadingRow(x.CompoundText, x.Text));

        compoundTable.InsertItems(forkContext, compoundRows);
        readingTable.InsertItems(forkContext, readingRows);
    }
}
