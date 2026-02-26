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

using Jitendex.KanjiVG.Import.Tables;
using Jitendex.KanjiVG.Import.Models;

namespace Jitendex.KanjiVG.Import;

internal sealed class DocumentDatabase(KanjiVGContext context)
{
    private readonly static EntryTable EntryTable = new();
    private readonly static VariantTable VariantTable = new();
    private readonly static VariantCommentTable VariantCommentTable = new();
    private readonly static ComponentGroupTable ComponentGroupTable = new();
    private readonly static ComponentTable ComponentTable = new();
    private readonly static StrokeNumberGroupTable StrokeNumberGroupTable = new();
    private readonly static StrokeNumberTable StrokeNumberTable = new();
    private readonly static StrokeTable StrokeTable = new();

    #region Lookup Tables
    private readonly static LookupTable<VariantTypeElement> VariantTypesTable = new();
    private readonly static LookupTable<CommentElement> CommentsTable = new();
    private readonly static LookupTable<ComponentGroupStyleElement> ComponentGroupStylesTable = new();
    private readonly static LookupTable<StrokeNumberGroupStyleElement> StrokeNumberGroupStylesTable = new();
    private readonly static LookupTable<ComponentCharacterElement> ComponentCharactersTable = new();
    private readonly static LookupTable<ComponentOriginalElement> ComponentOriginalsTable = new();
    private readonly static LookupTable<ComponentPositionElement> ComponentPositionsTable = new();
    private readonly static LookupTable<ComponentRadicalElement> ComponentRadicalsTable = new();
    private readonly static LookupTable<ComponentPhonElement> ComponentPhonsTable = new();
    private readonly static LookupTable<StrokeTypeElement> StrokeTypesTable = new();
    #endregion

    public void Initialize(Document document)
    {
        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        VariantTypesTable.InsertItems(context, document.GetVariantTypes());
        CommentsTable.InsertItems(context, document.GetComments());
        ComponentGroupStylesTable.InsertItems(context, document.GetComponentGroupStyles());
        StrokeNumberGroupStylesTable.InsertItems(context, document.GetStrokeNumberGroupStyles());
        ComponentCharactersTable.InsertItems(context, document.GetComponentCharacters());
        ComponentOriginalsTable.InsertItems(context, document.GetComponentOriginals());
        ComponentPositionsTable.InsertItems(context, document.GetComponentPositions());
        ComponentRadicalsTable.InsertItems(context, document.GetComponentRadicals());
        ComponentPhonsTable.InsertItems(context, document.GetComponentPhons());
        StrokeTypesTable.InsertItems(context, document.GetStrokeTypes());

        EntryTable.InsertItems(context, document.GetEntries());
        VariantTable.InsertItems(context, document.Variants.Values);
        VariantCommentTable.InsertItems(context, document.VariantComments.Values);

        ComponentGroupTable.InsertItems(context, document.ComponentGroups.Values);
        ComponentTable.InsertItems(context, document.Components.Values);
        StrokeTable.InsertItems(context, document.Strokes.Values);

        StrokeNumberGroupTable.InsertItems(context, document.StrokeNumberGroups.Values);
        StrokeNumberTable.InsertItems(context, document.StrokeNumbers.Values);

        transaction.Commit();
        context.ExecuteVacuum();
    }
}
