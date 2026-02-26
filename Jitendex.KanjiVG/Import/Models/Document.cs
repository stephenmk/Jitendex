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

namespace Jitendex.KanjiVG.Import.Models;

internal sealed class Document
{
    public HashSet<int> Kanjis { get; init; }
    public Dictionary<(int, int), VariantElement> Variants { get; init; }

    #region Component Group
    public Dictionary<(int, int), ComponentGroupElement> ComponentGroups { get; init; }
    public Dictionary<(int, int, int), ComponentElement> Components { get; init; }
    public Dictionary<(int, int, int), StrokeElement> Strokes { get; init; }
    #endregion

    #region Stroke Number Group
    public Dictionary<(int, int), StrokeNumberGroupElement> StrokeNumberGroups { get; init; }
    public Dictionary<(int, int, int), StrokeNumberElement> StrokeNumbers { get; init; }
    #endregion

    #region Lookups
    public Dictionary<string, int> VariantTypes { get; init; } = [];
    public Dictionary<string, int> Comments { get; init; } = [];
    public Dictionary<string, int> ComponentGroupStyles { get; init; } = [];
    public Dictionary<string, int> StrokeNumberGroupStyles { get; init; } = [];
    public Dictionary<string, int> ComponentCharacters { get; init; } = [];
    public Dictionary<string, int> ComponentOriginals { get; init; } = [];
    public Dictionary<string, int> ComponentPositions { get; init; } = [];
    public Dictionary<string, int> ComponentRadicals { get; init; } = [];
    public Dictionary<string, int> ComponentPhons { get; init; } = [];
    public Dictionary<string, int> StrokeTypes { get; init; } = [];
    #endregion

    public Document(int expectedEntryCount = 7_000)
    {
        Kanjis = new(expectedEntryCount);
        Variants = new(expectedEntryCount * 2);
        ComponentGroups = new(expectedEntryCount * 2);
        Components = new(expectedEntryCount * 13);
        Strokes = new(expectedEntryCount * 22);
        StrokeNumberGroups = new(expectedEntryCount * 2);
        StrokeNumbers = new(expectedEntryCount * 22);
    }

    public IEnumerable<KanjiElement> GetKanjis()
        => Kanjis.Select(id => new KanjiElement(id));

    public IEnumerable<VariantTypeElement> GetVariantTypes()
        => VariantTypes.Select(kvp => new VariantTypeElement(kvp.Value, kvp.Key));

    public IEnumerable<CommentElement> GetComments()
        => Comments.Select(kvp => new CommentElement(kvp.Value, kvp.Key));

    public IEnumerable<ComponentGroupStyleElement> GetComponentGroupStyles()
        => ComponentGroupStyles.Select(kvp => new ComponentGroupStyleElement(kvp.Value, kvp.Key));

    public IEnumerable<StrokeNumberGroupStyleElement> GetStrokeNumberGroupStyles()
        => StrokeNumberGroupStyles.Select(kvp => new StrokeNumberGroupStyleElement(kvp.Value, kvp.Key));

    public IEnumerable<ComponentCharacterElement> GetComponentCharacters()
        => ComponentCharacters.Select(kvp => new ComponentCharacterElement(kvp.Value, kvp.Key));

    public IEnumerable<ComponentOriginalElement> GetComponentOriginals()
        => ComponentOriginals.Select(kvp => new ComponentOriginalElement(kvp.Value, kvp.Key));

    public IEnumerable<ComponentPositionElement> GetComponentPositions()
        => ComponentPositions.Select(kvp => new ComponentPositionElement(kvp.Value, kvp.Key));

    public IEnumerable<ComponentRadicalElement> GetComponentRadicals()
        => ComponentRadicals.Select(kvp => new ComponentRadicalElement(kvp.Value, kvp.Key));

    public IEnumerable<ComponentPhonElement> GetComponentPhons()
        => ComponentPhons.Select(kvp => new ComponentPhonElement(kvp.Value, kvp.Key));

    public IEnumerable<StrokeTypeElement> GetStrokeTypes()
        => StrokeTypes.Select(kvp => new StrokeTypeElement(kvp.Value, kvp.Key));
}
