// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, HeadwordRows.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

namespace Jitendex.Process.JMdict.TableRows;

internal sealed record HeadwordRow
(
    int EntryId,
    int Order,
    int Score,
    string Surface,
    string? Reading,
    int? ReadingOrder,
    int? KanjiFormOrder
);

internal sealed record HeadwordRedirectRow
(
    int EntryId,
    int HeadwordOrder,
    int RedirectHeadwordOrder
);

internal sealed record HeadwordReferenceRow
(
    int EntryId,
    int SenseOrder,
    int Order,
    int RefEntryId,
    int RefHeadwordOrder,
    int RefSenseOrder
);

internal sealed record HeadwordRuleRow
(
    int EntryId,
    int HeadwordOrder,
    string Name
);

internal sealed record HeadwordSenseRow
(
    int EntryId,
    int HeadwordOrder,
    int Order,
    int SenseOrder
);

internal sealed record HeadwordTagRow
(
    int EntryId,
    int HeadwordOrder,
    string Name
);
