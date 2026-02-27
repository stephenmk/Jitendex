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

namespace Jitendex.Chise.Import.Models;

internal sealed class Document
{
    public HashSet<string> SequenceTexts { get; init; } = [];
    public HashSet<int> UnicodeCharacters { get; init; } = [];

    public Dictionary<string, CodepointElement> Codepoints { get; init; } = [];
    public List<CodepointElement> DiscoveredCodepoints { get; init; } = [];
    public HashSet<ComponentElement> Components { get; init; } = [];
    public HashSet<SequenceComponentElement> ComponentSequences { get; init; } = [];

    public void AddParsedSequence(ParserState? parsedSequence)
    {
        if (parsedSequence is null)
        {
            return;
        }
        SequenceTexts.UnionWith(parsedSequence.SequenceTexts);
        UnicodeCharacters.UnionWith(parsedSequence.UnicodeCharacters);
        DiscoveredCodepoints.AddRange(parsedSequence.Codepoints);
        Components.UnionWith(parsedSequence.Components);
        ComponentSequences.UnionWith(parsedSequence.ComponentSequences);
    }

    public IEnumerable<SequenceElement> GetSequences()
        => SequenceTexts.Select(text => new SequenceElement(text));

    public IEnumerable<UnicodeCharacterElement> GetUnicodeCharacters()
        => UnicodeCharacters.Select(i => new UnicodeCharacterElement(i));

    public IEnumerable<ComponentPositionElement> GetComponentPositions()
    {
        foreach (ComponentPositionId id in Enum.GetValues(typeof(ComponentPositionId)))
        {
            yield return new ComponentPositionElement((int)id, id.ToName());
        }
    }

    public IEnumerable<CodepointElement> GetCodepoints()
    {
        foreach (var discoveredCodepoint in DiscoveredCodepoints)
        {
            if (!Codepoints.ContainsKey(discoveredCodepoint.Id))
            {
                Codepoints.Add(discoveredCodepoint.Id, discoveredCodepoint);
            }
        }
        return Codepoints.Values;
    }
}
