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

using Jitendex.Chise.Import.Models;
using Jitendex.Chise.Import.Tables;

namespace Jitendex.Chise.Import;

internal sealed class DocumentDatabase(ChiseContext context)
{
    private readonly DescriptionSequenceTable DescriptionSequenceTable = new();
    private readonly ComponentPositionTable ComponentPositionTable = new();
    private readonly UnicodeCharacterTable UnicodeCharacterTable = new();

    private readonly CodepointTable CodepointTable = new();
    private readonly ComponentTable ComponentTable = new();
    private readonly SequenceComponentTable ComponentSequenceTable = new();

    public void Initialize(Document document)
    {
        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        DescriptionSequenceTable.InsertItems(context, document.GetDescriptionSequences());
        ComponentPositionTable.InsertItems(context, document.GetComponentPositions());
        UnicodeCharacterTable.InsertItems(context, document.GetUnicodeCharacters());

        CodepointTable.InsertItems(context, document.GetCodepoints());
        ComponentTable.InsertItems(context, document.Components);
        ComponentSequenceTable.InsertItems(context, document.ComponentSequences);

        transaction.Commit();
        context.ExecuteVacuum();
    }
}
