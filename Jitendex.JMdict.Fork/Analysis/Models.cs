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

namespace Jitendex.JMdict.Fork.Analysis;

internal sealed record ParsedReferenceText
(
    string Text1,
    string? Text2,
    int SenseNumber
);

internal sealed record RestrictionRow
(
    int EntryId,
    int ReadingOrder,
    int Order,
    int KanjiFormOrder
);

internal sealed record ReadingRestrictionRow
(
    int EntryId,
    int SenseOrder,
    int Order,
    int ReadingOrder
);

internal sealed record KanjiFormRestrictionRow
(
    int EntryId,
    int SenseOrder,
    int Order,
    int KanjiFormOrder
);

internal sealed record FuriganaSegmentRow
(
    int EntryId,
    int ReadingOrder,
    int KanjiFormOrder,
    int Order,
    string BaseText,
    string? Furigana,
    string? TypeName
);

internal sealed record KanjiFormBridgeRow
(
    int EntryId,
    int ReadingOrder,
    int KanjiFormOrder
);

internal sealed record CrossReferenceRow
(
    int EntryId,
    int SenseOrder,
    int Order,
    int? RefEntryId,
    int? RefReadingOrder,
    int? RefKanjiFormOrder,
    int? RefSenseOrder,
    bool? IsAmbiguous
);

internal sealed record CompoundRow(string Text);

internal sealed record CompoundReadingRow
(
    string CompoundText,
    string Text
);

internal sealed record CharacterRow(int Value);

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

