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

using System.Text;
using Jitendex.Furigana;

namespace Jitendex.JMdict.Fork.Analysis.Services;

internal sealed class FuriganaSolverService(JMdictForkContext context)
{
    public IFuriganaService LoadFuriganaService()
    {
        var service = FuriganaServiceProvider.GetFuriganaService();

        var characters = context.CharacterReadings
            .Select(static g => new
            {
                Rune = new Rune(g.CharacterValue),
                DerivedReadings = g.DerivedReadings
                    .Select(static x => new { x.Text, x.IsPrefix, x.IsSuffix })
            });

        foreach (var character in characters)
        {
            foreach (var reading in character.DerivedReadings)
            {
                service.AddCharacterReading(character.Rune, reading.Text, reading.IsPrefix, reading.IsSuffix);
            }
        }

        var compoundReadings = context.CompoundReadings
            .Select(static c => new
            {
                c.CompoundText,
                c.Text,
            });

        foreach (var reading in compoundReadings)
        {
            service.AddCompoundReading(reading.CompoundText, reading.Text);
        }

        return service;
    }
}
