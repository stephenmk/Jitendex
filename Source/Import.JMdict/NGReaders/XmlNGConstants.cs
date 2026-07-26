// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, XmlNGConstants.cs, is part of Jitendex.
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

namespace Jitendex.Import.JMdict.NGReaders;

internal static class XmlNGTagName
{
    public const string EntryNote = "info";
}

internal static class XmlNGAttributeName
{
    public const string CrossReferenceType = "type";
    public const string CrossReferenceSequence = "seq";
    public const string CrossReferenceCorpus = "dict";
    public const string CrossReferenceSenseNumber = "sno";
    public const string CrossReferenceKanjiForm = "xk";
    public const string CrossReferenceReading = "xr";
}
