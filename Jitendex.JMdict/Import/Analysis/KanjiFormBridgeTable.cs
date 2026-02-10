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
using Jitendex.JMdict.Entities.EntryItems.ReadingItems;

namespace Jitendex.JMdict.Import.Analysis;

internal sealed record KanjiFormBridgeElement(int EntryId, int ReadingOrder, int KanjiFormOrder);

internal sealed class KanjiFormBridgeTable : Table<KanjiFormBridgeElement>
{
    protected override string Name => nameof(KanjiFormBridge);

    protected override IReadOnlyList<string> ColumnNames =>
    [
        nameof(KanjiFormBridge.EntryId),
        nameof(KanjiFormBridge.ReadingOrder),
        nameof(KanjiFormBridge.KanjiFormOrder),
    ];

    protected override IReadOnlyList<string> KeyColNames =>
    [
        nameof(KanjiFormBridge.EntryId),
        nameof(KanjiFormBridge.ReadingOrder),
        nameof(KanjiFormBridge.KanjiFormOrder),
    ];

    protected override SqliteParameter[] Parameters(KanjiFormBridgeElement bridge) =>
    [
        new("@0", bridge.EntryId),
        new("@1", bridge.ReadingOrder),
        new("@2", bridge.KanjiFormOrder),
    ];
}
