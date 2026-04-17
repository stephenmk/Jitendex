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
using Jitendex.Data.Tatoeba.Entities;
using Jitendex.Dto.Tatoeba;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.Tatoeba;

public static class DtoMapper
{
    public static Dictionary<int, SequenceDto> LoadSequencesWithoutRevisions(TatoebaContext context, IReadOnlySet<int> sequenceIds)
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
            Example = seq.Example == null ? null : new ExampleDto
            {
                Text = seq.Example.Text,
                Segmentations = seq.Example.Segmentations
                    .AsQueryable()
                    .OrderBy(static s => s.Order)
                    .Select(SegmentationProjection)
                    .ToList()
            }
        };

    private static Expression<Func<Segmentation, SegmentationDto>> SegmentationProjection =>
        static segmentation => new SegmentationDto
        {
            Translation = new TranslationDto(segmentation.TranslationId, segmentation.Translation.Text),
            Tokens = segmentation.Tokens
                .AsQueryable()
                .OrderBy(static t => t.Order)
                .Select(TokenProjection)
                .ToList()
        };

    private static Expression<Func<Token, TokenDto>> TokenProjection =>
        static token => new TokenDto
        (
            Headword: token.Headword,
            Reading: token.Reading,
            EntryId: token.EntryId,
            SenseNumber: token.SenseNumber,
            SentenceForm: token.SentenceForm,
            IsPriority: token.IsPriority
        );
}
