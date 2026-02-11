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

internal sealed record RestrictionUpdate(int EntryId, int ReadingOrder, int Order, int KanjiFormOrder);
internal sealed record ReadingRestrictionUpdate(int EntryId, int SenseOrder, int Order, int ReadingOrder);
internal sealed record KanjiFormRestrictionUpdate(int EntryId, int SenseOrder, int Order, int KanjiFormOrder);
internal sealed record KanjiFormBridgeElement(int EntryId, int ReadingOrder, int KanjiFormOrder);

internal sealed record ParsedReferenceText(string Text1, string? Text2, int SenseNumber);
internal sealed record CrossReferenceUpdate(
    int EntryId,
    int SenseOrder,
    int Order,
    int? RefEntryId,
    int? RefReadingOrder,
    int? RefKanjiFormOrder,
    int? RefSenseOrder,
    bool? IsAmbiguous);
