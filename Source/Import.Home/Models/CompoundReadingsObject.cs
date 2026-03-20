/*
Copyright (c) 2026 Stephen Kraus
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

using System.Collections.Immutable;
using Jitendex.Data.Home.Entities.Kanwa;

namespace Jitendex.Import.Home.Models;

internal sealed record CompoundReadingsObject
{
    public ImmutableArray<string> Unknown { get; init; } = [];
    public ImmutableArray<string> Alphanumeric { get; init; } = [];
    public ImmutableArray<string> Ateji { get; init; } = [];
    public ImmutableArray<string> Idiom { get; init; } = [];
    public ImmutableArray<string> Nonstandard { get; init; } = [];

    public List<CompoundReadingRow> ToReadingRows(int compoundId)
    {
        var readingRows = new List<CompoundReadingRow>();

        readingRows.AddRange(Unknown.Select(x =>
            ToReadingRow(compoundId, x, CompoundReadingTypeId.Unknown)));
        readingRows.AddRange(Alphanumeric.Select(x =>
            ToReadingRow(compoundId, x, CompoundReadingTypeId.Alphanumeric)));
        readingRows.AddRange(Ateji.Select(x =>
            ToReadingRow(compoundId, x, CompoundReadingTypeId.Ateji)));
        readingRows.AddRange(Idiom.Select(x =>
            ToReadingRow(compoundId, x, CompoundReadingTypeId.Idiom)));
        readingRows.AddRange(Nonstandard.Select(x =>
            ToReadingRow(compoundId, x, CompoundReadingTypeId.Nonstandard)));

        return readingRows;
    }

    private static CompoundReadingRow ToReadingRow(int compoundId, string text, CompoundReadingTypeId typeId)
        => new(compoundId, text, (int)typeId);
}
