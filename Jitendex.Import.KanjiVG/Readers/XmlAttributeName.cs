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

namespace Jitendex.Import.KanjiVG.Readers;

internal static class XmlAttributeName
{
    public const string Id = "id";
    public const string Style = "style";

    public const string Width = "width";
    public const string Height = "height";
    public const string ViewBox = "viewBox";
    public const string Transform = "transform";
    public const string PathData = "d";

    public const string KvgStrokePathsPrefix = "kvg:StrokePaths";
    public const string KvgStrokeNumbersPrefix = "kvg:StrokeNumbers";

    public const string XmlNamespace = "xmlns";
    public const string KvgNamespace = "xmlns:kvg";
    public const string KvgType = "kvg:type";
    public const string KvgElement = "kvg:element";
    public const string KvgVariant = "kvg:variant";
    public const string KvgPartial = "kvg:partial";
    public const string KvgOriginal = "kvg:original";
    public const string KvgPart = "kvg:part";
    public const string KvgNumber = "kvg:number";
    public const string KvgTradForm = "kvg:tradForm";
    public const string KvgRadicalForm = "kvg:radicalForm";
    public const string KvgPosition = "kvg:position";
    public const string KvgRadical = "kvg:radical";
    public const string KvgPhon = "kvg:phon";
}
