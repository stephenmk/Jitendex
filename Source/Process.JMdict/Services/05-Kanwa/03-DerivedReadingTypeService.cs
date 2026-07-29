// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, 03-DerivedReadingTypeService.cs, is part of Jitendex.
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

using Jitendex.Data.JMdict;
using Jitendex.Data.JMdict.ForkEntities.Kanwa;
using Jitendex.Process.JMdict.TableRows;
using Jitendex.Process.JMdict.Tables.Kanwa;

namespace Jitendex.Process.JMdict.Services.Kanwa;

internal sealed class DerivedReadingTypeService
(
    JMdictForkContext forkContext,
    DerivedCharacterReadingTypeTable table
)
{
    public void Write()
    {
        table.InsertItems(forkContext, GetTypeRows());
    }

    private static IEnumerable<DerivedCharacterReadingTypeRow> GetTypeRows()
    {
        foreach (var type in Enum.GetValues<DerivedCharacterReadingTypeId>())
        {
            yield return new DerivedCharacterReadingTypeRow((int)type, type.ToString());
        }
    }
}
