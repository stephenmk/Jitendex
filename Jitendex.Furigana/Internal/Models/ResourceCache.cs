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

namespace Jitendex.Furigana.Internal.Models;

internal sealed record CharacterReading(string Text, bool IsPrefix, bool IsSuffix);

// TODO: The dictionaries and lists in this class really only need to be mutable
// by the Service class. The solver classes should not be able to mutate it.

internal sealed class ResourceCache
{
    public Dictionary<string, List<string>> Compounds { get; init; } = [];
    public Dictionary<int, List<CharacterReading>> Characters { get; init; } = [];
    public Dictionary<int, List<string>> NameKanji { get; init; } = [];
    public Dictionary<int, List<string>> Hanzi { get; init; } = [];
    public Dictionary<int, List<string>> Hanja { get; init; } = [];
}
