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

using System.Linq;
using System.Text.Json.Nodes;

namespace Jitendex.Dto.JMdict;

public static class DtoJsonExtensions
{
    public static JsonNode ToJsonNode(this SequenceDto sequence)
        => new JsonObject()
        {
            [nameof(SequenceDto.Id)] = sequence.Id,
            [nameof(SequenceDto.Entry)] = sequence.Entry?.ToJsonObject(),
        };

    private static JsonObject ToJsonObject(this EntryDto entry)
        => new()
        {
            [nameof(EntryDto.Readings)] = new JsonArray(entry.Readings.Select(ToJsonObject).ToArray()),
            [nameof(EntryDto.KanjiForms)] = new JsonArray(entry.KanjiForms.Select(ToJsonObject).ToArray()),
            [nameof(EntryDto.Senses)] = new JsonArray(entry.Senses.Select(ToJsonObject).ToArray()),
        };

    private static JsonObject ToJsonObject(this ReadingDto reading)
        => new()
        {
            [nameof(ReadingDto.Text)] = reading.Text,
            [nameof(ReadingDto.NoKanji)] = reading.NoKanji,
            [nameof(ReadingDto.Infos)] = reading.Infos.ToJsonNodes(),
            [nameof(ReadingDto.Priorities)] = reading.Priorities.ToJsonNodes(),
            [nameof(ReadingDto.Restrictions)] = reading.Restrictions.ToJsonNodes(),
        };

    private static JsonObject ToJsonObject(this KanjiFormDto kanjiForm)
        => new()
        {
            [nameof(KanjiFormDto.Text)] = kanjiForm.Text,
            [nameof(KanjiFormDto.Infos)] = kanjiForm.Infos.ToJsonNodes(),
            [nameof(KanjiFormDto.Priorities)] = kanjiForm.Priorities.ToJsonNodes(),
        };

    private static JsonObject ToJsonObject(this SenseDto sense)
        => new()
        {
            [nameof(SenseDto.CrossReferences)] = new JsonArray(sense.CrossReferences.Select(ToJsonObject).ToArray()),
            [nameof(SenseDto.Dialects)] = sense.Dialects.ToJsonNodes(),
            [nameof(SenseDto.Fields)] = sense.Fields.ToJsonNodes(),
            [nameof(SenseDto.Glosses)] = new JsonArray(sense.Glosses.Select(ToJsonObject).ToArray()),
            [nameof(SenseDto.KanjiFormRestrictions)] = sense.KanjiFormRestrictions.ToJsonNodes(),
            [nameof(SenseDto.LanguageSources)] = new JsonArray(sense.LanguageSources.Select(ToJsonObject).ToArray()),
            [nameof(SenseDto.Miscs)] = sense.Miscs.ToJsonNodes(),
            [nameof(SenseDto.Notes)] = sense.Notes.ToJsonNodes(),
            [nameof(SenseDto.PartsOfSpeech)] = sense.PartsOfSpeech.ToJsonNodes(),
            [nameof(SenseDto.ReadingRestrictions)] = sense.ReadingRestrictions.ToJsonNodes(),
        };

    private static JsonObject ToJsonObject(this CrossReferenceDto crossReferenceDto)
        => new()
        {
            [nameof(CrossReferenceDto.TypeName)] = crossReferenceDto.TypeName,
            [nameof(CrossReferenceDto.Text)] = crossReferenceDto.Text,
        };

    private static JsonObject ToJsonObject(this GlossDto glossDto)
        => new()
        {
            [nameof(GlossDto.TypeName)] = glossDto.TypeName,
            [nameof(GlossDto.Text)] = glossDto.Text,
        };

    private static JsonObject ToJsonObject(this LanguageSourceDto languageSourceDto)
        => new()
        {
            [nameof(LanguageSourceDto.Text)] = languageSourceDto.Text,
            [nameof(LanguageSourceDto.LanguageCode)] = languageSourceDto.LanguageCode,
            [nameof(LanguageSourceDto.TypeName)] = languageSourceDto.TypeName,
            [nameof(LanguageSourceDto.IsWasei)] = languageSourceDto.IsWasei,
        };

    private static JsonArray ToJsonNodes(this List<string> list)
        => [.. list.Select(static x => (JsonNode)x)];
}
