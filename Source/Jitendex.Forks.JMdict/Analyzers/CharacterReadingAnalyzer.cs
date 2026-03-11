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

using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.Models;
using Jitendex.Forks.JMdict.Tables.Furigana;
using J = Jitendex.Data.JMdict.Entities.Kanwa;
using H = Jitendex.Data.Home.Entities.Furigana;

namespace Jitendex.Forks.JMdict.Analyzers;

internal sealed class CharacterReadingAnalyzer
(
    JMdictForkContext forkContext,
    HomeContext homeContext,
    CharacterReadingTable table,
    CharacterReadingTypeTable typeTable
)
{
    public void Analyze()
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

    private static J.CharacterReadingTypeId ConvertTypeId(H.CharacterReadingTypeId id) => id switch
    {
        H.CharacterReadingTypeId.Onyomi       => J.CharacterReadingTypeId.Onyomi,
        H.CharacterReadingTypeId.Kunyomi      => J.CharacterReadingTypeId.Kunyomi,
        H.CharacterReadingTypeId.Chinese      => J.CharacterReadingTypeId.Chinese,
        H.CharacterReadingTypeId.Korean       => J.CharacterReadingTypeId.Korean,
        H.CharacterReadingTypeId.Kana         => J.CharacterReadingTypeId.Kana,
        H.CharacterReadingTypeId.Alphanumeric => J.CharacterReadingTypeId.Alphanumeric,
        H.CharacterReadingTypeId.Symbol       => J.CharacterReadingTypeId.Symbol,
        H.CharacterReadingTypeId.Unknown      => J.CharacterReadingTypeId.Unknown,
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };
}
