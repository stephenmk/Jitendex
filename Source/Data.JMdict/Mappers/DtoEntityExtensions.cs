// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DtoEntityExtensions.cs, is part of Jitendex.
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

using Jitendex.Data.JMdict.Entities;
using Jitendex.Data.JMdict.Entities.EntryChildren;
using Jitendex.Data.JMdict.Entities.EntryChildren.KanjiFormChildren;
using Jitendex.Data.JMdict.Entities.EntryChildren.ReadingChildren;
using Jitendex.Data.JMdict.Entities.EntryChildren.SenseChildren;
using Jitendex.Dto.JMdict;

namespace Jitendex.Data.JMdict.Mappers;

public static class DtoEntityExtensions
{
    public static Entry ToEntry(this EntryDto entry, int id)
        => new()
        {
            Id = id,
            Readings = entry.Readings
                .Select((reading, order) => reading.ToReading(id, order))
                .ToList(),
            KanjiForms = entry.KanjiForms
                .Select((kanjiForm, order) => kanjiForm.ToKanjiForm(id, order))
                .ToList(),
            Senses = entry.Senses
                .Select((sense, order) => sense.ToSense(id, order))
                .ToList(),
        };

    private static Reading ToReading(this ReadingDto reading, int entryId, int readingOrder)
        => new()
        {
            EntryId = entryId,
            Order = readingOrder,
            Text = reading.Text,
            NoKanji = reading.NoKanji,
            Infos = reading.Infos
                .Select((info, order) => info.ToReadingInfo(entryId, readingOrder, order))
                .ToList(),
            Priorities = reading.Priorities
                .Select((info, order) => info.ToReadingPriority(entryId, readingOrder, order))
                .ToList(),
            Restrictions = reading.Restrictions
                .Select((info, order) => info.ToRestriction(entryId, readingOrder, order))
                .ToList(),
        };

    private static KanjiForm ToKanjiForm(this KanjiFormDto kanjiForm, int entryId, int kanjiFormOrder)
        => new()
        {
            EntryId = entryId,
            Order = kanjiFormOrder,
            Text = kanjiForm.Text,
            Infos = kanjiForm.Infos
                .Select((info, order) => info.ToKanjiFormInfo(entryId, kanjiFormOrder, order))
                .ToList(),
            Priorities = kanjiForm.Priorities
                .Select((info, order) => info.ToKanjiFormPriority(entryId, kanjiFormOrder, order))
                .ToList(),
        };

    private static Sense ToSense(this SenseDto sense, int entryId, int senseOrder)
        => new()
        {
            EntryId = entryId,
            Order = senseOrder,
            CrossReferences = sense.CrossReferences
                .Select((x, order) => x.ToCrossReference(entryId, senseOrder, order))
                .ToList(),
            Dialects = sense.Dialects
                .Select((x, order) => x.ToDialect(entryId, senseOrder, order))
                .ToList(),
            Fields = sense.Fields
                .Select((x, order) => x.ToField(entryId, senseOrder, order))
                .ToList(),
            Glosses = sense.Glosses
                .Select((x, order) => x.ToGloss(entryId, senseOrder, order))
                .ToList(),
            KanjiFormRestrictions = sense.KanjiFormRestrictions
                .Select((x, order) => x.ToKanjiFormRestriction(entryId, senseOrder, order))
                .ToList(),
            LanguageSources = sense.LanguageSources
                .Select((x, order) => x.ToLanguageSource(entryId, senseOrder, order))
                .ToList(),
            Miscs = sense.Miscs
                .Select((x, order) => x.ToMisc(entryId, senseOrder, order))
                .ToList(),
            Notes = sense.Notes
                .Select((x, order) => x.ToNote(entryId, senseOrder, order))
                .ToList(),
            PartsOfSpeech = sense.PartsOfSpeech
                .Select((x, order) => x.ToPartOfSpeech(entryId, senseOrder, order))
                .ToList(),
            ReadingRestrictions = sense.ReadingRestrictions
                .Select((x, order) => x.ToReadingRestriction(entryId, senseOrder, order))
                .ToList(),
        };

    private static ReadingInfo ToReadingInfo(this string tagName, int entryId, int readingOrder, int order)
        => new()
        {
            EntryId = entryId,
            ReadingOrder = readingOrder,
            Order = order,
            TagName = tagName,
        };

    private static ReadingPriority ToReadingPriority(this string tagName, int entryId, int readingOrder, int order)
        => new()
        {
            EntryId = entryId,
            ReadingOrder = readingOrder,
            Order = order,
            TagName = tagName,
        };

    private static Restriction ToRestriction(this string kanjiFormText, int entryId, int readingOrder, int order)
        => new()
        {
            EntryId = entryId,
            ReadingOrder = readingOrder,
            Order = order,
            KanjiFormText = kanjiFormText,
        };

    private static KanjiFormInfo ToKanjiFormInfo(this string tagName, int entryId, int kanjiFormOrder, int order)
        => new()
        {
            EntryId = entryId,
            KanjiFormOrder = kanjiFormOrder,
            Order = order,
            TagName = tagName,
        };

    private static KanjiFormPriority ToKanjiFormPriority(this string tagName, int entryId, int kanjiFormOrder, int order)
        => new()
        {
            EntryId = entryId,
            KanjiFormOrder = kanjiFormOrder,
            Order = order,
            TagName = tagName,
        };

    private static CrossReference ToCrossReference(this CrossReferenceDto crossReference, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            Text = crossReference.Text,
            TypeName = crossReference.TypeName,
        };

    private static Dialect ToDialect(this string tagName, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            TagName = tagName,
        };

    private static Field ToField(this string tagName, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            TagName = tagName,
        };

    private static Gloss ToGloss(this GlossDto gloss, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            Text = gloss.Text,
            Type = gloss.TypeName is null ? null : new()
            {
                EntryId = entryId,
                SenseOrder = senseOrder,
                GlossOrder = order,
                TagName = gloss.TypeName
            }
        };

    private static KanjiFormRestriction ToKanjiFormRestriction(this string kanjiFormText, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            KanjiFormText = kanjiFormText,
        };

    private static LanguageSource ToLanguageSource(this LanguageSourceDto source, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            Text = source.Text,
            TypeName = source.TypeName,
            LanguageCode = source.LanguageCode,
            IsWasei = source.IsWasei,
        };

    private static Misc ToMisc(this string tagName, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            TagName = tagName,
        };

    private static Note ToNote(this string text, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            Text = text,
        };

    private static PartOfSpeech ToPartOfSpeech(this string tagName, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            TagName = tagName,
        };

    private static ReadingRestriction ToReadingRestriction(this string readingText, int entryId, int senseOrder, int order)
        => new()
        {
            EntryId = entryId,
            SenseOrder = senseOrder,
            Order = order,
            ReadingText = readingText,
        };
}
