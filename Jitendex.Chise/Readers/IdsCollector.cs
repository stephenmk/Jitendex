/*
Copyright (c) 2025 Stephen Kraus
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

using Jitendex.Chise.Entities;

namespace Jitendex.Chise.Readers;

internal class IdsCollector
{
    private readonly Logger _logger;
    public Dictionary<string, Codepoint> Codepoints { get; } = [];
    public Dictionary<(string CodepointId, ComponentPositionId PositionId), Component> Components = [];
    public Dictionary<ComponentPositionId, ComponentPosition> ComponentPositions = [];
    public Dictionary<string, Sequence> Sequences { get; } = [];
    public Dictionary<int, UnicodeCharacter> UnicodeCharacters { get; } = [];

    public IdsCollector(Logger logger)
    {
        _logger = logger;
    }

    public void AddCodepoint(Codepoint codepoint, bool topLevel = true)
    {
        if (topLevel || !Codepoints.ContainsKey(codepoint.Id))
        {
            Codepoints[codepoint.Id] = codepoint;
        }

        AddUnicodeCharacter(codepoint.UnicodeCharacter);
        AddSequence(codepoint.Sequence);
        AddSequence(codepoint.AltSequence);
    }

    private void AddUnicodeCharacter(UnicodeCharacter? character)
    {
        if (character is null)
        {
            return;
        }
        if (!UnicodeCharacters.TryGetValue(character.ScalarValue, out var oldCharacter))
        {
            UnicodeCharacters[character.ScalarValue] = character;
        }
        else if (character.CodepointId != oldCharacter.CodepointId)
        {
            throw new Exception();
        }
    }

    private void AddSequence(Sequence? sequence)
    {
        if (sequence is null)
        {
            return;
        }
        if (!Sequences.ContainsKey(sequence.Text))
        {
            Sequences[sequence.Text] = sequence;
        }
        foreach (var component in sequence.Components)
        {
            AddComponent(component);
        }
    }

    private void AddComponent(Component component)
    {
        var key = (component.CodepointId, component.PositionId);

        if (Components.TryGetValue(key, out var oldComponent))
        {
            foreach (var sequence in component.Sequences)
            {
                if (!oldComponent.Sequences.Any(s => s.Text == sequence.Text))
                {
                    oldComponent.Sequences.Add(sequence);
                }
            }
        }
        else
        {
            Components[key] = component;
        }

        if (!ComponentPositions.ContainsKey(component.PositionId))
        {
            ComponentPositions[component.PositionId] = component.Position;
        }

        AddCodepoint(component.Codepoint, topLevel: false);
    }
}
