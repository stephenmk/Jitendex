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

using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Jitendex.Dto.JMnedict;
using Jitendex.JMnedict.Entities;
using Jitendex.JMnedict.Entities.EntryItems;

namespace Jitendex.JMnedict;

public static class DtoMapper
{
    public static Dictionary<int, SequenceDto> LoadSequencesWithoutRevisions(JMnedictContext context, IReadOnlySet<int> sequenceIds)
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
                KanjiForms = seq.Entry.KanjiForms
                    .AsQueryable()
                    .OrderBy(static kanjiForm => kanjiForm.Order)
                    .Select(KanjiFormProjection)
                    .ToImmutableArray(),
                Readings = seq.Entry.Readings
                    .AsQueryable()
                    .OrderBy(static reading => reading.Order)
                    .Select(ReadingProjection)
                    .ToImmutableArray(),
                Translations = seq.Entry.Translations
                    .AsQueryable()
                    .OrderBy(static translation => translation.Order)
                    .Select(TranslationProjection)
                    .ToImmutableArray()
            }
        };

    private static Expression<Func<KanjiForm, KanjiFormDto>> KanjiFormProjection =>
        static kanjiForm => new KanjiFormDto
        {
            Text = kanjiForm.Text,
            Infos = kanjiForm.Infos
                .OrderBy(static info => info.Order)
                .Select(static info => info.TagName)
                .ToImmutableArray(),
            Priorities = kanjiForm.Priorities
                .OrderBy(static prio => prio.Order)
                .Select(static prio => prio.TagName)
                .ToImmutableArray(),
        };

    private static Expression<Func<Reading, ReadingDto>> ReadingProjection =>
        static reading => new ReadingDto
        {
            Text = reading.Text,
            Infos = reading.Infos
                .OrderBy(static info => info.Order)
                .Select(static info => info.TagName)
                .ToImmutableArray(),
            Priorities = reading.Priorities
                .OrderBy(static prio => prio.Order)
                .Select(static prio => prio.TagName)
                .ToImmutableArray(),
            Restrictions = reading.Restrictions
                .OrderBy(static rstr => rstr.Order)
                .Select(static rstr => rstr.KanjiFormText)
                .ToImmutableArray(),
        };

    private static Expression<Func<Translation, TranslationDto>> TranslationProjection =>
        static translation => new TranslationDto
        {
            CrossReferences = translation.CrossReferences
                .OrderBy(static x => x.Order)
                .Select(static x => x.Text)
                .ToImmutableArray(),
            Details = translation.Details
                .OrderBy(static detail => detail.Order)
                .Select(static detail => new DetailDto(detail.Text, detail.LanguageName))
                .ToImmutableArray(),
            NameTypes = translation.NameTypes
                .OrderBy(static m => m.Order)
                .Select(static m => m.TagName)
                .ToImmutableArray(),
        };
}
