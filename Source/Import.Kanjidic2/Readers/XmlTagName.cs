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

namespace Jitendex.Import.Kanjidic2.Readers;

internal static class XmlTagName
{
    public const string Kanjidic2 = "kanjidic2";
    public const string Entry = "character";
    public const string EntryCharacter = "literal";

    public const string Header = "header";
    public const string FileVersion = "file_version";
    public const string DatabaseVersion = "database_version";
    public const string CreationDate = "date_of_creation";

    public const string CodepointGroup = "codepoint";
    public const string DictionaryGroup = "dic_number";
    public const string MiscGroup = "misc";
    public const string QueryCodeGroup = "query_code";
    public const string RadicalGroup = "radical";
    public const string ReadingMeaningGroup = "reading_meaning";

    public const string Grade = "grade";
    public const string Frequency = "freq";
    public const string Jlpt = "jlpt";

    public const string Codepoint = "cp_value";
    public const string Dictionary = "dic_ref";
    public const string Nanori = "nanori";
    public const string QueryCode = "q_code";
    public const string Radical = "rad_value";
    public const string RadicalName = "rad_name";
    public const string ReadingMeaning = "rmgroup";
    public const string StrokeCount = "stroke_count";
    public const string Variant = "variant";

    public const string Meaning = "meaning";
    public const string Reading = "reading";
}
