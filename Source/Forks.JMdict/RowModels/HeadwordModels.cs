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

namespace Jitendex.Forks.JMdict.RowModels;

internal sealed record HeadwordRow
(
    int Id,
    string Surface,
    string? Reading,
    int EntryId,
    int? ReadingOrder,
    int? KanjiFormOrder,
    int Score
);

internal sealed record HeadwordNumberRow
(
    int HeadwordId,
    int Number
);

internal sealed record HeadwordRedirectRow
(
    int HeadwordId,
    int RedirectId
);

internal sealed record HeadwordReferenceRow
(
    int EntryId,
    int SenseOrder,
    int CrossReferenceOrder,
    int HeadwordId
);

internal sealed record HeadwordRuleRow
(
    int HeadwordId,
    string Name
);

internal sealed record HeadwordSenseRow
(
    int HeadwordId,
    int Number,
    int EntryId,
    int SenseOrder
);

internal sealed record HeadwordTagRow
(
    int HeadwordId,
    string Name
);
