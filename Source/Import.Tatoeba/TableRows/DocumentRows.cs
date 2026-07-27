// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DocumentRows.cs, is part of Jitendex.
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

namespace Jitendex.Import.Tatoeba.TableRows;

internal sealed record HeaderRow
(
    DateOnly Date
);

internal sealed record RevisionRow
(
    int SequenceId,
    int FileHeaderId,
    bool IsPriority,
    byte[] DiffJson
);

internal sealed record SequenceRow
(
    int Id,
    int FileHeaderId
);

internal sealed record ExampleRow
(
    int Id,
    string Text
);
