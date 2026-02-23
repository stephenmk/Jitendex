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

namespace Jitendex.JMnedict.Import.Parsing;

internal static class XmlTagName
{
    public const string Root = "?xml";
    public const string JMnedict = "JMnedict";
    public const string Entry = "entry";
    public const string Sequence = "ent_seq";

    public const string KanjiForm = "k_ele";
    public const string KanjiFormText = "keb";
    public const string KanjiFormInfo = "ke_inf";
    public const string KanjiFormPriority = "ke_pri";

    public const string Reading = "r_ele";
    public const string ReadingText = "reb";
    public const string ReadingInfo = "re_inf";
    public const string ReadingPriority = "re_pri";
    public const string ReadingRestriction = "re_restr";

    public const string Translation = "trans";
    public const string CrossReference = "xref";
    public const string Detail = "trans_det";
    public const string NameType = "name_type";
}
