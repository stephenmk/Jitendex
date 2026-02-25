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

using Microsoft.Data.Sqlite;
using Jitendex.SQLite;
using Jitendex.Tatoeba.Entities;
using Jitendex.Tatoeba.Import.Models;

namespace Jitendex.Tatoeba.Import.Tables;

internal sealed class SegmentationTable : Table<SegmentationElement>
{
    protected override string Name => nameof(Segmentation);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(Segmentation.ExampleId),
        nameof(Segmentation.Order),
        nameof(Segmentation.TranslationId),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(Segmentation.ExampleId),
        nameof(Segmentation.Order),
    ];

    protected override SqliteParameter[] Parameters(SegmentationElement sentence) =>
    [
        new("@0", sentence.ExampleId),
        new("@1", sentence.Order),
        new("@2", sentence.TranslationId),
    ];
}
