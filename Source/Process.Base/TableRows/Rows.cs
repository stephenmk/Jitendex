// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Rows.cs, is part of Jitendex.
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

namespace Jitendex.Process.Base.TableRows;

internal sealed record HeadwordRow
(
    string Surface,
    string? Reading
);

internal sealed record HeadwordFuriganaRow
(
    int HeadwordId,
    int Order,
    string BaseText,
    string? RubyText
);

internal sealed record TermGroupRow
(
    int Id
);

internal sealed record JMdictEntryRow
(
    int Id,
    int GroupId
);

internal sealed record TermRow
(
    int HeadwordId,
    int GroupId,
    int Score
);
