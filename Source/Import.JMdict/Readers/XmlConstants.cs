// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, XmlConstants.cs, is part of Jitendex.
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

namespace Jitendex.Import.JMdict.Readers;

internal static class XmlTagName
{
    public const string Root = "?xml";
    public const string Jmdict = "JMdict";
    public const string Entry = "entry";
    public const string Sequence = "ent_seq";

    public const string KanjiForm = "k_ele";
    public const string KanjiFormText = "keb";
    public const string KanjiFormInfo = "ke_inf";
    public const string KanjiFormPriority = "ke_pri";

    public const string Reading = "r_ele";
    public const string ReadingText = "reb";
    public const string ReadingNoKanji = "re_nokanji";
    public const string ReadingInfo = "re_inf";
    public const string ReadingPriority = "re_pri";
    public const string ReadingRestriction = "re_restr";

    public const string Sense = "sense";
    public const string SenseNote = "s_inf";
    public const string CrossReference = "xref";
    public const string Antonym = "ant";
    public const string Dialect = "dial";
    public const string Example = "example";
    public const string Field = "field";
    public const string Gloss = "gloss";
    public const string SenseKanjiFormRestriction = "stagk";
    public const string LanguageSource = "lsource";
    public const string Misc = "misc";
    public const string PartOfSpeech = "pos";
    public const string SenseReadingRestriction = "stagr";
}

internal static class XmlAttributeName
{
    public const string GlossType = "g_type";
    public const string LanguageSourceType = "ls_type";
    public const string LanguageSourceCode = "xml:lang";
    public const string LanguageSourceWasei = "ls_wasei";
}
