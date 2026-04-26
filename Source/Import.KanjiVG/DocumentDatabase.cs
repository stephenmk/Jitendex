// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DocumentDatabase.cs, is part of Jitendex.
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

using Jitendex.Data.KanjiVG;
using Jitendex.Import.KanjiVG.Models;
using Jitendex.Import.KanjiVG.Tables;

namespace Jitendex.Import.KanjiVG;

internal sealed class DocumentDatabase(KanjiVGContext context)
{
    private static readonly KanjiTable EntryTable = new();
    private static readonly VariantTable VariantTable = new();
    private static readonly ComponentGroupTable ComponentGroupTable = new();
    private static readonly ComponentTable ComponentTable = new();
    private static readonly StrokeNumberGroupTable StrokeNumberGroupTable = new();
    private static readonly StrokeNumberTable StrokeNumberTable = new();
    private static readonly StrokeTable StrokeTable = new();

    #region Lookup Tables
    private static readonly LookupTable<VariantTypeElement> VariantTypesTable = new();
    private static readonly LookupTable<CommentElement> CommentsTable = new();
    private static readonly LookupTable<ComponentGroupStyleElement> ComponentGroupStylesTable = new();
    private static readonly LookupTable<StrokeNumberGroupStyleElement> StrokeNumberGroupStylesTable = new();
    private static readonly LookupTable<ComponentCharacterElement> ComponentCharactersTable = new();
    private static readonly LookupTable<ComponentOriginalElement> ComponentOriginalsTable = new();
    private static readonly LookupTable<ComponentPositionElement> ComponentPositionsTable = new();
    private static readonly LookupTable<ComponentRadicalElement> ComponentRadicalsTable = new();
    private static readonly LookupTable<ComponentPhonElement> ComponentPhonsTable = new();
    private static readonly LookupTable<StrokeTypeElement> StrokeTypesTable = new();
    #endregion

    public void Initialize(Document document)
    {
        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        #pragma warning disable format

        VariantTypesTable            .InsertItems(context, document.GetVariantTypes());
        CommentsTable                .InsertItems(context, document.GetComments());
        ComponentGroupStylesTable    .InsertItems(context, document.GetComponentGroupStyles());
        StrokeNumberGroupStylesTable .InsertItems(context, document.GetStrokeNumberGroupStyles());
        ComponentCharactersTable     .InsertItems(context, document.GetComponentCharacters());
        ComponentOriginalsTable      .InsertItems(context, document.GetComponentOriginals());
        ComponentPositionsTable      .InsertItems(context, document.GetComponentPositions());
        ComponentRadicalsTable       .InsertItems(context, document.GetComponentRadicals());
        ComponentPhonsTable          .InsertItems(context, document.GetComponentPhons());
        StrokeTypesTable             .InsertItems(context, document.GetStrokeTypes());

        EntryTable                   .InsertItems(context, document.GetKanjis());
        VariantTable                 .InsertItems(context, document.Variants.Values);

        ComponentGroupTable          .InsertItems(context, document.ComponentGroups.Values);
        ComponentTable               .InsertItems(context, document.Components.Values);
        StrokeTable                  .InsertItems(context, document.Strokes.Values);

        StrokeNumberGroupTable       .InsertItems(context, document.StrokeNumberGroups.Values);
        StrokeNumberTable            .InsertItems(context, document.StrokeNumbers.Values);

        #pragma warning restore format

        transaction.Commit();
        context.ExecuteVacuum();
    }
}
