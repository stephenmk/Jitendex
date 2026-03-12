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
using Jitendex.Forks.JMdict.Models;
using Jitendex.Forks.JMdict.Tables.Kanwa;
using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using ForkTypeId = Jitendex.Data.JMdict.ForkEntities.Kanwa.CharacterReadingTypeId;
using HomeTypeId = Jitendex.Data.Home.Entities.Furigana.CharacterReadingTypeId;
using static Jitendex.Data.Home.Entities.Furigana.CharacterReadingTypeId;

namespace Jitendex.Forks.JMdict.Services.Kanwa;

internal sealed class CharacterReadingService
(
    JMdictForkContext forkContext,
    HomeContext homeContext,
    CharacterReadingTable table,
    CharacterReadingTypeTable typeTable
)
{
    public void Write()
    {
        var typeRows = homeContext.CharacterReadingTypes
            .Select(static x => x.Id)
            .Select(static id => ConvertTypeId(id))
            .Select(static id => new CharacterReadingTypeRow((int)id, id.ToString()));

        var rows = homeContext.CharacterReadings
            .Select(static x => new CharacterReadingRow
            (
                CharacterValue: x.CharacterValue,
                TypeId: (int)ConvertTypeId(x.TypeId),
                Text: x.Text,
                Okurigana: x.Okurigana,
                IsPrefix: x.IsPrefix,
                IsSuffix: x.IsSuffix
            ));

        typeTable.InsertItems(forkContext, typeRows);
        table.InsertItems(forkContext, rows);
    }

    private static ForkTypeId ConvertTypeId(HomeTypeId id) => id switch
    {
        Onyomi       => ForkTypeId.Onyomi,
        Kunyomi      => ForkTypeId.Kunyomi,
        Chinese      => ForkTypeId.Chinese,
        Korean       => ForkTypeId.Korean,
        Kana         => ForkTypeId.Kana,
        Alphanumeric => ForkTypeId.Alphanumeric,
        Symbol       => ForkTypeId.Symbol,
        Unknown      => ForkTypeId.Unknown,
        _            => throw new ArgumentOutOfRangeException(nameof(id))
    };
}
