// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, TermChildRows.cs, is part of Jitendex.
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

internal sealed record TermRedirectRow
(
    int HeadwordId,
    int TermGroupId,
    int RedirectHeadwordId,
    int RedirectTermGroup
);

internal sealed record TermRuleRow
(
    int HeadwordId,
    int TermGroupId,
    string Name
);

internal sealed record TermNumberRow
(
    int HeadwordId,
    int TermGroupId,
    int Value,
    int Total
);

internal sealed record TermTagRow
(
    int HeadwordId,
    int TermGroupId,
    int TypeId
);

internal sealed record TermTagTypeRow
(
    int Id,
    string Name
);

internal sealed record TermGlossaryRow
(
    int HeadwordId,
    int TermGroupId,
    byte[] Json
);
