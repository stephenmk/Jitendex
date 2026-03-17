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

namespace Jitendex.Forks.JMdict.Models;

internal sealed record CompoundRow
(
    int Id,
    string Text
);

internal sealed record CompoundReadingRow
(
    int CompoundId,
    string Text
);

internal sealed record CompoundCharacterRow
(
    int CompoundId,
    int Order,
    int CharacterValue
);

internal sealed record CharacterRow
(
    int Value
);

internal sealed record CharacterReadingRow
(
    int CharacterValue,
    int TypeId,
    string Text,
    string? Okurigana,
    bool IsPrefix,
    bool IsSuffix
);

internal sealed record CharacterReadingTypeRow
(
    int Id,
    string Name
);

internal sealed record DerivedCharacterReadingRow
(
    int ReadingId,
    string Text,
    bool IsPrefix,
    bool IsSuffix,
    int TypeId
);

internal sealed record DerivedCharacterReadingTypeRow
(
    int Id,
    string Name
);
