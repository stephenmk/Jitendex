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

using Microsoft.EntityFrameworkCore;
using Jitendex.SQLite;
using Jitendex.JMdictAnalysis.Entities;
using Jitendex.JMdictAnalysis.Entities.EntryItems;
using Jitendex.JMdictAnalysis.Entities.EntryItems.KanjiFormItems;
using Jitendex.JMdictAnalysis.Entities.EntryItems.ReadingItems;
using Jitendex.JMdictAnalysis.Entities.EntryItems.SenseItems;

namespace Jitendex.JMdictAnalysis.Analysis;

internal sealed class Database(JmdictAnalysisContext context)
{
    public void TransferDataFromJmdict()
    {
        context.AttachDatabase(DatabaseFile.JMdict);
        context.Database.ExecuteSqlRaw(TransferCommandText);
    }

    /// <remarks>
    /// This assumes that the table names and column names
    /// in the JMdict SQLite database are exactly the same.
    /// </remarks>
    private const string TransferCommandText =
        $"""
        INSERT INTO "{nameof(ReadingInfoTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(ReadingInfoTag)}";

        INSERT INTO "{nameof(KanjiFormInfoTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(KanjiFormInfoTag)}";

        INSERT INTO "{nameof(PartOfSpeechTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(PartOfSpeechTag)}";

        INSERT INTO "{nameof(FieldTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(FieldTag)}";

        INSERT INTO "{nameof(MiscTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(MiscTag)}";

        INSERT INTO "{nameof(DialectTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(DialectTag)}";

        INSERT INTO "{nameof(GlossType)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(GlossType)}";

        INSERT INTO "{nameof(CrossReferenceType)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(CrossReferenceType)}";

        INSERT INTO "{nameof(LanguageSourceType)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(LanguageSourceType)}";

        INSERT INTO "{nameof(PriorityTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(PriorityTag)}";

        INSERT INTO "{nameof(Language)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.CreatedDate)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Language)}";

        INSERT INTO "{nameof(Sequence)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Sequence)}";

        INSERT INTO "{nameof(Entry)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Entry)}";

        INSERT INTO "{nameof(KanjiForm)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(KanjiForm)}";

        INSERT INTO "{nameof(KanjiFormInfo)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(KanjiFormInfo)}";

        INSERT INTO "{nameof(KanjiFormPriority)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(KanjiFormPriority)}";

        INSERT INTO "{nameof(Reading)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Reading)}";

        INSERT INTO "{nameof(ReadingInfo)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(ReadingInfo)}";

        INSERT INTO "{nameof(ReadingPriority)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(ReadingPriority)}";

        INSERT INTO "{nameof(Restriction)}"
             ( "{nameof(Restriction.EntryId)}"
             , "{nameof(Restriction.ReadingOrder)}"
             , "{nameof(Restriction.Order)}"
             , "{nameof(Restriction.KanjiFormText)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Restriction)}";

        INSERT INTO "{nameof(Sense)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Sense)}";

        INSERT INTO "{nameof(CrossReference)}"
             ( "{nameof(CrossReference.EntryId)}"
             , "{nameof(CrossReference.SenseOrder)}"
             , "{nameof(CrossReference.Order)}"
             , "{nameof(CrossReference.TypeName)}"
             , "{nameof(CrossReference.Text)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(CrossReference)}";

        INSERT INTO "{nameof(Dialect)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Dialect)}";

        INSERT INTO "{nameof(Field)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Field)}";

        INSERT INTO "{nameof(Gloss)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Gloss)}";

        INSERT INTO "{nameof(KanjiFormRestriction)}"
             ( "{nameof(KanjiFormRestriction.EntryId)}"
             , "{nameof(KanjiFormRestriction.SenseOrder)}"
             , "{nameof(KanjiFormRestriction.Order)}"
             , "{nameof(KanjiFormRestriction.KanjiFormText)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(KanjiFormRestriction)}";

        INSERT INTO "{nameof(LanguageSource)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(LanguageSource)}";

        INSERT INTO "{nameof(Misc)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Misc)}";

        INSERT INTO "{nameof(Note)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(Note)}";

        INSERT INTO "{nameof(PartOfSpeech)}"
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(PartOfSpeech)}";

        INSERT INTO "{nameof(ReadingRestriction)}"
             ( "{nameof(ReadingRestriction.EntryId)}"
             , "{nameof(ReadingRestriction.SenseOrder)}"
             , "{nameof(ReadingRestriction.Order)}"
             , "{nameof(ReadingRestriction.ReadingText)}"
             )
        SELECT * FROM "{nameof(DatabaseFile.JMdict)}"."{nameof(ReadingRestriction)}";
        """;
}
