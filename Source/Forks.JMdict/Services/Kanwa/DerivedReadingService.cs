// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DerivedReadingService.cs, is part of Jitendex.
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
using Jitendex.Forks.JMdict.TableRows;
using Jitendex.Forks.JMdict.Tables.Kanwa;
using Jitendex.JapaneseTextUtils;
using static Jitendex.Data.JMdict.ForkEntities.Kanwa.CharacterReadingTypeId;

namespace Jitendex.Forks.JMdict.Services.Kanwa;

internal sealed class DerivedReadingService
(
    JMdictForkContext context,
    DerivedCharacterReadingTable table
)
{
    private sealed record ReadingData
    (
        int Id,
        string Text,
        bool IsPrefix,
        bool IsSuffix,
        CharacterReadingTypeId TypeId,
        ImmutableArray<string> Okuriganas
    );

    public void Write()
    {
        var entries = context.CharacterReadings
            .GroupBy(static r => r.TypeId)
            .Select(static g => new
            {
                TypeId = g.Key,
                Readings = g.Select(static r => new ReadingData
                (
                    Id: r.Id,
                    Text: r.Text,
                    IsPrefix: r.IsPrefix,
                    IsSuffix: r.IsSuffix,
                    TypeId: r.TypeId,
                    Okuriganas: r.Okuriganas
                        .Select(static o => o.Text)
                        .ToImmutableArray()
                ))
            });

        var rows = new List<DerivedCharacterReadingRow>();

        foreach (var entry in entries)
        {
            #pragma warning disable format
            var newRows = entry.TypeId switch
            {
                Onyomi  => entry.Readings.SelectMany(GetDerivedOnReadings),
                Kunyomi => entry.Readings.SelectMany(GetDerivedKunReadings),
                _       => entry.Readings.SelectMany(GetDerivedReadings),
            };
            #pragma warning restore format
            rows.AddRange(newRows);
        }

        var distinctRows = rows
            .DistinctBy(static r => new { r.ReadingId, r.Text });

        table.InsertItems(context, distinctRows);
    }

    private IEnumerable<DerivedCharacterReadingRow> GetDerivedOnReadings(ReadingData rdg)
    {
        var normalizedReading = rdg.Text.KatakanaToHiragana();

        yield return new
        (
            rdg.Id,
            normalizedReading,
            rdg.IsPrefix,
            rdg.IsSuffix,
            (int)DerivedCharacterReadingTypeId.Onyomi
        );

        if (!rdg.IsPrefix && normalizedReading.ToSokuonForm() is string sokuonReading)
        {
            yield return new
            (
                rdg.Id,
                sokuonReading,
                IsPrefix: true,
                rdg.IsSuffix,
                (int)DerivedCharacterReadingTypeId.OnyomiSokuon
            );

            if (!rdg.IsSuffix)
            {
                foreach (string rendakuReading in sokuonReading.ToRendakuForms())
                {
                    yield return new
                    (
                        rdg.Id,
                        rendakuReading,
                        IsPrefix: true,
                        IsSuffix: true,
                        (int)DerivedCharacterReadingTypeId.OnyomiSokuonRendaku
                    );
                }
            }
        }

        if (!rdg.IsSuffix)
        {
            foreach (string rendakuReading in normalizedReading.ToRendakuForms())
            {
                yield return new
                (
                    rdg.Id,
                    rendakuReading,
                    rdg.IsPrefix,
                    IsSuffix: true,
                    (int)DerivedCharacterReadingTypeId.OnyomiRendaku
                );
            }
        }
    }

    private IEnumerable<DerivedCharacterReadingRow> GetDerivedKunReadings(ReadingData rdg)
    {
        if (rdg.Okuriganas is [])
        {
            return GetDerivedKunStems(rdg);
        }
        else
        {
            return GetDerivedSuffixedKunReadings(rdg);
        }
    }

    private IEnumerable<DerivedCharacterReadingRow> GetDerivedKunStems(ReadingData rdg)
    {
        var normalizedReading = rdg.Text.KatakanaToHiragana();

        yield return new DerivedCharacterReadingRow
        (
            rdg.Id,
            normalizedReading,
            rdg.IsPrefix,
            rdg.IsSuffix,
            (int)DerivedCharacterReadingTypeId.Kunyomi
        );

        foreach (string rendakuReading in normalizedReading.ToRendakuForms())
        {
            yield return new
            (
                rdg.Id,
                rendakuReading,
                IsPrefix: rdg.IsPrefix,
                IsSuffix: true,
                (int)DerivedCharacterReadingTypeId.KunyomiRendaku
            );
        }
    }

    private IEnumerable<DerivedCharacterReadingRow> GetDerivedSuffixedKunReadings(ReadingData rdg)
    {
        foreach (var stem in GetDerivedKunStems(rdg))
        {
            yield return stem;

            foreach (var okurigana in rdg.Okuriganas)
            {
                for (int i = 0; i < okurigana.Length; i++)
                {
                    yield return new
                    (
                        rdg.Id,
                        Text: string.Concat(stem.Text, okurigana[..(i + 1)]),
                        IsPrefix: false,
                        IsSuffix: stem.IsSuffix,
                        TypeId: stem.TypeId == (int)DerivedCharacterReadingTypeId.Kunyomi
                            ? (int)DerivedCharacterReadingTypeId.KunyomiOkurigana
                            : (int)DerivedCharacterReadingTypeId.KunyomiRendakuOkurigana
                    );
                }

                var fullText = string.Concat(stem.Text, okurigana);

                if (fullText.VerbToMasuStem() is string masuStem)
                {
                    yield return new
                    (
                        rdg.Id,
                        Text: masuStem,
                        IsPrefix: false,
                        IsSuffix: stem.IsSuffix,
                        TypeId: stem.TypeId == (int)DerivedCharacterReadingTypeId.Kunyomi
                            ? (int)DerivedCharacterReadingTypeId.KunyomiMasu
                            : (int)DerivedCharacterReadingTypeId.KunyomiRendakuMasu
                    );
                }

                if (okurigana.EndsWith('す') || okurigana.EndsWith('ず'))
                {
                    // Masu stems and Te stems are identical.
                    continue;
                }

                if (fullText.VerbToTeStem() is string teStem)
                {
                    yield return new
                    (
                        rdg.Id,
                        Text: teStem,
                        IsPrefix: false,
                        IsSuffix: stem.IsSuffix,
                        TypeId: stem.TypeId == (int)DerivedCharacterReadingTypeId.Kunyomi
                            ? (int)DerivedCharacterReadingTypeId.KunyomiTe
                            : (int)DerivedCharacterReadingTypeId.KunyomiRendakuTe
                    );
                }
            }
        }
    }

    private IEnumerable<DerivedCharacterReadingRow> GetDerivedReadings(ReadingData rdg)
    {
        yield return new
        (
            rdg.Id,
            rdg.Text.KatakanaToHiragana(),
            rdg.IsPrefix,
            rdg.IsSuffix,
            (int)GetDerivedReadingTypeId(rdg.TypeId)
        );
    }

    #pragma warning disable format
    private static DerivedCharacterReadingTypeId GetDerivedReadingTypeId(CharacterReadingTypeId id) => id switch
    {
        Chinese      => DerivedCharacterReadingTypeId.Chinese,
        Korean       => DerivedCharacterReadingTypeId.Korean,
        Kana         => DerivedCharacterReadingTypeId.Kana,
        Alphanumeric => DerivedCharacterReadingTypeId.Alphanumeric,
        Symbol       => DerivedCharacterReadingTypeId.Symbol,
        Unknown      => DerivedCharacterReadingTypeId.Unknown,
        _            => throw new ArgumentOutOfRangeException(nameof(id)),
    };
    #pragma warning restore format
}
