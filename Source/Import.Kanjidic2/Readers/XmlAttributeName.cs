// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, XmlAttributeName.cs, is part of Jitendex.
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

namespace Jitendex.Import.Kanjidic2.Readers;

internal static class XmlAttributeName
{
    public const string CodepointType = "cp_type";
    public const string DictionaryType = "dr_type";
    public const string Volume = "m_vol";
    public const string Page = "m_page";
    public const string VariantType = "var_type";
    public const string QueryCodeType = "qc_type";
    public const string MisclassificationType = "skip_misclass";
    public const string RadicalType = "rad_type";
    public const string ReadingType = "r_type";
    public const string MeaningType = "m_lang";
}
