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
using Jitendex.Dto.JMdict;
using Jitendex.JMdict.Entities;
using Jitendex.JMdict.Entities.EntryItems;

namespace Jitendex.JMdict;

public static class DtoMapper
{
    public static Dictionary<int, SequenceDto> LoadSequencesWithoutRevisions(JmdictContext context, IEnumerable<int> sequenceIds)
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
                Senses = seq.Entry.Senses
                    .AsQueryable()
                    .OrderBy(static sense => sense.Order)
                    .Select(SenseProjection)
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
            NoKanji = reading.NoKanji,
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

    private static Expression<Func<Sense, SenseDto>> SenseProjection =>
        static sense => new SenseDto
        {
            CrossReferences = sense.CrossReferences
                .OrderBy(static x => x.Order)
                .Select(static x => new CrossReferenceDto(x.TypeName, x.Text))
                .ToImmutableArray(),
            Dialects = sense.Dialects
                .OrderBy(static dia => dia.Order)
                .Select(static dia => dia.TagName)
                .ToImmutableArray(),
            Fields = sense.Fields
                .OrderBy(static fld => fld.Order)
                .Select(static fld => fld.TagName)
                .ToImmutableArray(),
            Glosses = sense.Glosses
                .OrderBy(static gloss => gloss.Order)
                .Select(static gloss => new GlossDto(gloss.TypeName, gloss.Text))
                .ToImmutableArray(),
            KanjiFormRestrictions = sense.KanjiFormRestrictions
                .OrderBy(static rstr => rstr.Order)
                .Select(static rstr => rstr.KanjiFormText)
                .ToImmutableArray(),
            LanguageSources = sense.LanguageSources
                .OrderBy(static l => l.Order)
                .Select(static l => new LanguageSourceDto(l.Text, l.LanguageCode, l.TypeName, l.IsWasei))
                .ToImmutableArray(),
            Miscs = sense.Miscs
                .OrderBy(static m => m.Order)
                .Select(static m => m.TagName)
                .ToImmutableArray(),
            Notes = sense.Notes
                .OrderBy(static n => n.Order)
                .Select(static n => n.Text)
                .ToImmutableArray(),
            PartsOfSpeech = sense.PartsOfSpeech
                .OrderBy(static pos => pos.Order)
                .Select(static pos => pos.TagName)
                .ToImmutableArray(),
            ReadingRestrictions = sense.ReadingRestrictions
                .OrderBy(static rstr => rstr.Order)
                .Select(static restr => restr.ReadingText)
                .ToImmutableArray(),
        };
}
