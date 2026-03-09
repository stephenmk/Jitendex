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

namespace Jitendex.Import.ChiseIds.Models;

internal sealed class ParserState
{
    public Stack<CodepointElement> Stack { get; } = [];

    public List<string> SequenceTexts { get; } = [];
    public List<int> UnicodeCharacters { get; } = [];

    public List<CodepointElement> Codepoints { get; } = [];
    public List<ComponentElement> Components { get; } = [];
    public List<SequenceComponentElement> ComponentSequences { get; } = [];
}
