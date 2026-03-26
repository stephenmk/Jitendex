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
using HomeTypeId = Jitendex.Data.Home.Entities.Kanwa.CharacterReadingTypeId;
using ForkTypeId = Jitendex.Data.JMdict.ForkEntities.Kanwa.CharacterReadingTypeId;

namespace Jitendex.Forks.JMdict.Services.Kanwa;

internal sealed class CharacterReadingService
(
    JMdictForkContext forkContext,
    HomeContext homeContext,
    CharacterReadingTable table,
    CharacterReadingTypeTable typeTable,
    CharacterReadingOkuriganaTable okuriganaTable
)
{
    public void Write()
    {
        WriteTypes();
        WriteReadings();
        WriteOkurigana();
    }

    private void WriteTypes()
    {
        var typeRows = homeContext.CharacterReadingTypes
            .Select(static x => ConvertTypeId(x.Id))
            .Select(static id => new CharacterReadingTypeRow((int)id, id.ToString()));

        typeTable.InsertItems(forkContext, typeRows);
    }

    private void WriteReadings()
    {
        var rows = homeContext.CharacterReadings
            .GroupBy(static x => new { x.CharacterValue, x.TypeId, x.Text, x.IsPrefix, x.IsSuffix })
            .Select(static g => new CharacterReadingRow
            (
                CharacterValue: g.Key.CharacterValue,
                TypeId: (int)ConvertTypeId(g.Key.TypeId),
                Text: g.Key.Text,
                IsPrefix: g.Key.IsPrefix,
                IsSuffix: g.Key.IsSuffix
            ));

        table.InsertItems(forkContext, rows);
    }

    private void WriteOkurigana()
    {
        var readingIds = forkContext.CharacterReadings
            .Select(static r => new
            {
                Key = new { r.CharacterValue, r.TypeId, r.Text },
                Value = r.Id,
            })
            .ToDictionary
            (
                static x => (x.Key.CharacterValue, x.Key.TypeId, x.Key.Text),
                static x => x.Value
            );

        var readings = homeContext.CharacterReadings
            .Where(static r => r.Okurigana != null)
            .GroupBy(static r => new { r.CharacterValue, r.TypeId, r.Text })
            .Select(static g => new
            {
                g.Key.CharacterValue,
                g.Key.TypeId,
                g.Key.Text,
                Okuriganas = g.Select(static r => (string)r.Okurigana!)
            });

        var rows = new List<CharacterReadingOkuriganaRow>();

        foreach (var reading in readings)
        {
            var readingId = readingIds[(reading.CharacterValue, ConvertTypeId(reading.TypeId), reading.Text)];
            foreach (var okurigana in reading.Okuriganas)
            {
                rows.Add(new(readingId, okurigana));
            }
        }

        okuriganaTable.InsertItems(forkContext, rows);
    }

    #pragma warning disable format
    private static ForkTypeId ConvertTypeId(HomeTypeId id) => id switch
    {
        HomeTypeId.Onyomi       => ForkTypeId.Onyomi,
        HomeTypeId.Kunyomi      => ForkTypeId.Kunyomi,
        HomeTypeId.Chinese      => ForkTypeId.Chinese,
        HomeTypeId.Korean       => ForkTypeId.Korean,
        HomeTypeId.Kana         => ForkTypeId.Kana,
        HomeTypeId.Alphanumeric => ForkTypeId.Alphanumeric,
        HomeTypeId.Symbol       => ForkTypeId.Symbol,
        HomeTypeId.Unknown      => ForkTypeId.Unknown,
        _                       => throw new ArgumentOutOfRangeException(nameof(id))
    };
    #pragma warning restore format
}
