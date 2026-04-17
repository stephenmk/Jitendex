/*
Copyright (c) 2026 Stephen Kraus
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

using System.Linq.Expressions;
using Jitendex.Data.Kanjidic2.Entities;
using Jitendex.Data.Kanjidic2.Entities.GroupItems;
using Jitendex.Data.Kanjidic2.Entities.Groups;
using Jitendex.Dto.Kanjidic2;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.Kanjidic2;

public static class DtoMapper
{
    public static Dictionary<int, SequenceDto> LoadRevisionlessSequences(Kanjidic2Context context, IReadOnlySet<int> sequenceIds)
        => context.Sequences
            .AsSplitQuery()
            .Where(seq => sequenceIds.Contains(seq.Id))
            .Select(RevisionlessSequenceProjection)
            .ToDictionary(static dto => dto.Id);

    private static Expression<Func<Sequence, SequenceDto>> RevisionlessSequenceProjection =>
        static seq => new SequenceDto
        {
            Id = seq.Id,
            CreatedDate = seq.OriginFile.Date,
            Entry = seq.Entry == null ? null : new EntryDto
            {
                CodepointGroups = seq.Entry.CodepointGroups
                    .AsQueryable()
                    .OrderBy(static g => g.Order)
                    .Select(CodepointGroupProjection)
                    .ToList(),
                DictionaryGroups = seq.Entry.DictionaryGroups
                    .AsQueryable()
                    .OrderBy(static g => g.Order)
                    .Select(DictionaryGroupProjection)
                    .ToList(),
                MiscGroups = seq.Entry.MiscGroups
                    .AsQueryable()
                    .OrderBy(static g => g.Order)
                    .Select(MiscGroupProjection)
                    .ToList(),
                QueryCodeGroups = seq.Entry.QueryCodeGroups
                    .AsQueryable()
                    .OrderBy(static g => g.Order)
                    .Select(QueryCodeGroupProjection)
                    .ToList(),
                RadicalGroups = seq.Entry.RadicalGroups
                    .AsQueryable()
                    .OrderBy(static g => g.Order)
                    .Select(RadicalGroupProjection)
                    .ToList(),
                ReadingMeaningGroups = seq.Entry.ReadingMeaningGroups
                    .AsQueryable()
                    .OrderBy(static g => g.Order)
                    .Select(ReadingMeaningGroupProjection)
                    .ToList()
            }
        };

    private static Expression<Func<CodepointGroup, CodepointGroupDto>> CodepointGroupProjection =>
        static group => new CodepointGroupDto
        {
            Codepoints = group.Codepoints
                .OrderBy(static c => c.Order)
                .Select(static c => new CodepointDto(c.Text, c.TypeName))
                .ToList()
        };

    private static Expression<Func<DictionaryGroup, DictionaryGroupDto>> DictionaryGroupProjection =>
        static group => new DictionaryGroupDto
        {
            Dictionaries = group.Dictionaries
                .OrderBy(static d => d.Order)
                .Select(static d => new DictionaryDto(d.Text, d.TypeName, d.Volume, d.Page))
                .ToList()
        };

    private static Expression<Func<MiscGroup, MiscGroupDto>> MiscGroupProjection =>
        static group => new MiscGroupDto
        {
            Grade = group.Grade,
            Frequency = group.Frequency,
            JlptLevel = group.JlptLevel,
            RadicalNames = group.RadicalNames
                .OrderBy(static n => n.Order)
                .Select(static n => n.Text)
                .ToList(),
            StrokeCounts = group.StrokeCounts
                .OrderBy(static s => s.Order)
                .Select(static s => s.Value)
                .ToList(),
            Variants = group.Variants
                .OrderBy(static v => v.Order)
                .Select(static v => new VariantDto(v.Text, v.TypeName))
                .ToList()
        };

    private static Expression<Func<QueryCodeGroup, QueryCodeGroupDto>> QueryCodeGroupProjection =>
        static group => new QueryCodeGroupDto
        {
            QueryCodes = group.QueryCodes
                .OrderBy(static qc => qc.Order)
                .Select(static qc => new QueryCodeDto(qc.Text, qc.TypeName, qc.Misclassification))
                .ToList()
        };

    private static Expression<Func<RadicalGroup, RadicalGroupDto>> RadicalGroupProjection =>
        static group => new RadicalGroupDto
        {
            Radicals = group.Radicals
                .OrderBy(static r => r.Order)
                .Select(static r => new RadicalDto(r.Number, r.TypeName))
                .ToList()
        };

    private static Expression<Func<ReadingMeaningGroup, ReadingMeaningGroupDto>> ReadingMeaningGroupProjection =>
        static group => new ReadingMeaningGroupDto
        {
            ReadingMeanings = group.ReadingMeanings
                .AsQueryable()
                .OrderBy(static rm => rm.Order)
                .Select(ReadingMeaningProjection)
                .ToList(),
            Nanoris = group.Nanoris
                .OrderBy(static n => n.Order)
                .Select(static n => n.Text)
                .ToList()
        };

    private static Expression<Func<ReadingMeaning, ReadingMeaningDto>> ReadingMeaningProjection =>
        static group => new ReadingMeaningDto
        {
            IsKokuji = group.IsKokuji,
            IsGhost = group.IsGhost,
            Meanings = group.Meanings
                .OrderBy(static m => m.Order)
                .Select(static m => m.Text)
                .ToList(),
            Readings = group.Readings
                .OrderBy(static r => r.Order)
                .Select(static r => new ReadingDto(r.Text, r.TypeName))
                .ToList()
        };
}
