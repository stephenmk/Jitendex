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

using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Jitendex.Forks.JMdict.Models;
using Jitendex.Forks.JMdict.Tables.Kanwa;

namespace Jitendex.Forks.JMdict.Services.Kanwa;

internal sealed class CharacterAnalyzer
(
    JMdictForkContext forkContext,
    HomeContext homeContext,
    CharacterTable table
)
{
    public void Analyze()
    {
        var allRunes = homeContext.Characters
            .Select(static x => x.Value)
            .ToHashSet();

        var allKanjiFormTexts = forkContext.KanjiForms
            .Select(static x => x.Text);

        var allCompoundTexts = forkContext.Compounds
            .Select(static x => x.Text);

        foreach (var text in allKanjiFormTexts.Concat(allCompoundTexts))
        {
            foreach (var rune in text.EnumerateRunes())
            {
                allRunes.Add(rune.Value);
            }
        }

        var rows = allRunes.Select(static x => new CharacterRow(x));
        table.InsertItems(forkContext, rows);
    }
}
