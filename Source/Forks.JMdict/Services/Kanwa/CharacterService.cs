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
using Jitendex.Forks.JMdict.TableRows;
using Jitendex.Forks.JMdict.Tables.Kanwa;
using HomeTypeId = Jitendex.Data.Home.Entities.Kanwa.CompoundReadingTypeId;
using ForkTypeId = Jitendex.Data.JMdict.ForkEntities.Kanwa.CompoundReadingTypeId;

namespace Jitendex.Forks.JMdict.Services.Kanwa;

internal sealed class CharacterService
(
    JMdictForkContext forkContext,
    HomeContext homeContext,
    CompoundTable compoundTable,
    CompoundReadingTable readingTable,
    CompoundReadingTypeTable readingTypeTable,
    CharacterTable characterTable,
    CompoundCharacterTable compoundCharacterTable,
    VariantTypeTable variantTypeTable,
    VariantTable variantTable
)
{
    public void Write()
    {
        WriteCompounds();
        WriteCompoundReadingTypes();
        WriteCompoundReadings();
        WriteCharacters();
        WriteCompoundCharacters();
        WriteVariantTypes();
        WriteVariants();
    }

    private void WriteCompounds()
    {
        var rows = homeContext.Compounds
            .Select(static x => new CompoundRow(x.Id, x.Text));

        compoundTable.InsertItems(forkContext, rows);
    }

    private void WriteCompoundReadingTypes()
    {
        var rows = homeContext.CompoundReadingTypes
            .Select(static x => ConvertTypeId(x.Id))
            .Select(static id => new CompoundReadingTypeRow((int)id, id.ToString()));

        readingTypeTable.InsertItems(forkContext, rows);
    }

    private void WriteCompoundReadings()
    {
        var rows = homeContext.CompoundReadings
            .Select(static x => new CompoundReadingRow(x.CompoundId, x.Text, (int)ConvertTypeId(x.TypeId)));

        readingTable.InsertItems(forkContext, rows);
    }

    private void WriteCharacters()
    {
        var allRunes = homeContext.Characters
            .Select(static x => x.Value)
            .ToHashSet();

        var allKanjiFormTexts = forkContext.KanjiForms
            .Select(static x => x.Text);

        var allCompoundTexts = forkContext.Compounds
            .Select(static x => x.Text);

        foreach (var text in allKanjiFormTexts.Concat(allCompoundTexts))
        {
            foreach (var rune in text.EnumerateRunes())
            {
                allRunes.Add(rune.Value);
            }
        }

        var rows = allRunes.Select(static x => new CharacterRow(x));
        characterTable.InsertItems(forkContext, rows);
    }

    private void WriteCompoundCharacters()
    {
        var rows = new List<CompoundCharacterRow>();

        var compounds = forkContext.Compounds
            .Select(static x => new { x.Id, x.Text });

        foreach (var compound in compounds)
        {
            int i = 0;
            foreach (var rune in compound.Text.EnumerateRunes())
            {
                rows.Add(new(compound.Id, i++, rune.Value));
            }
        }

        compoundCharacterTable.InsertItems(forkContext, rows);
    }

    private void WriteVariantTypes()
    {
        var rows = homeContext.VariantTypes
            .Select(static id => new VariantTypeRow((int)id.Id, id.Name));

        variantTypeTable.InsertItems(forkContext, rows);
    }

    private void WriteVariants()
    {
        var rows = homeContext.Variants
            .Select(static v => new VariantRow(v.CharacterValue, v.VariantValue, (int)v.TypeId));

        variantTable.InsertItems(forkContext, rows);
    }

    #pragma warning disable format
    private static ForkTypeId ConvertTypeId(HomeTypeId id) => id switch
    {
        HomeTypeId.Alphanumeric => ForkTypeId.Alphanumeric,
        HomeTypeId.Ateji        => ForkTypeId.Ateji,
        HomeTypeId.Idiom        => ForkTypeId.Idiom,
        _                       => throw new ArgumentOutOfRangeException(nameof(id))
    };
    #pragma warning restore format
}
