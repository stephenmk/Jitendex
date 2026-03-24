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

using Jitendex.Data;
using Jitendex.Data.ChiseIds.Entities;
using Jitendex.Import.ChiseIds.Models;

namespace Jitendex.Import.ChiseIds.Tables;

internal sealed class DescriptionSequenceTable : Table<DescriptionSequenceElement>
{
    protected override string Name => nameof(DescriptionSequence);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(DescriptionSequence.Text)
    ];

    protected override IReadOnlyList<string> KeyColNames => ColumnNames;

    protected override object?[] ParameterValues(DescriptionSequenceElement sequence) =>
    [
        sequence.Text
    ];
}
