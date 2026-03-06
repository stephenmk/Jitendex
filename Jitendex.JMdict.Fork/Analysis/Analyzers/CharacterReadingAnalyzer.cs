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
using F = Jitendex.JMdict.Fork.Entities.EntryItems.Furigana;
using M = Jitendex.MiscData.Entities.Furigana;

namespace Jitendex.JMdict.Fork.Analysis.Analyzers;

internal sealed class CharacterReadingAnalyzer
(
    JMdictForkContext forkContext,
    MiscDataContext miscContext,
    CharacterReadingTable table,
    CharacterReadingTypeTable typeTable
)
{
    public void Analyze()
    {
        var typeRows = miscContext.CharacterReadingTypes
            .Select(static x => x.Id)
            .Select(static id => ConvertTypeId(id))
            .Select(static id => new CharacterReadingTypeRow((int)id, id.ToString()));

        var rows = miscContext.CharacterReadings
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

    private static F.CharacterReadingTypeId ConvertTypeId(M.CharacterReadingTypeId id) => id switch
    {
        M.CharacterReadingTypeId.Onyomi       => F.CharacterReadingTypeId.Onyomi,
        M.CharacterReadingTypeId.Kunyomi      => F.CharacterReadingTypeId.Kunyomi,
        M.CharacterReadingTypeId.Chinese      => F.CharacterReadingTypeId.Chinese,
        M.CharacterReadingTypeId.Korean       => F.CharacterReadingTypeId.Korean,
        M.CharacterReadingTypeId.Kana         => F.CharacterReadingTypeId.Kana,
        M.CharacterReadingTypeId.Alphanumeric => F.CharacterReadingTypeId.Alphanumeric,
        M.CharacterReadingTypeId.Symbol       => F.CharacterReadingTypeId.Symbol,
        M.CharacterReadingTypeId.Unknown      => F.CharacterReadingTypeId.Unknown,
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };
}
