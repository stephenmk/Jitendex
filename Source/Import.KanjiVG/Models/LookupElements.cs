// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, LookupElements.cs, is part of Jitendex.
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

namespace Jitendex.Import.KanjiVG.Models;

internal interface ILookupElement
{
    int Id { get; init; }
    string Text { get; init; }
}

internal sealed record VariantTypeElement(int Id, string Text) : ILookupElement;
internal sealed record CommentElement(int Id, string Text) : ILookupElement;
internal sealed record ComponentGroupStyleElement(int Id, string Text) : ILookupElement;
internal sealed record StrokeNumberGroupStyleElement(int Id, string Text) : ILookupElement;
internal sealed record ComponentCharacterElement(int Id, string Text) : ILookupElement;
internal sealed record ComponentOriginalElement(int Id, string Text) : ILookupElement;
internal sealed record ComponentPositionElement(int Id, string Text) : ILookupElement;
internal sealed record ComponentRadicalElement(int Id, string Text) : ILookupElement;
internal sealed record ComponentPhonElement(int Id, string Text) : ILookupElement;
internal sealed record StrokeTypeElement(int Id, string Text) : ILookupElement;
