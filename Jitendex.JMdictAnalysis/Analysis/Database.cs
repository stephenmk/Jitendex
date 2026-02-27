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
using Jitendex.JMdict.Entities;
using Jitendex.JMdict.Entities.EntryItems;
using Jitendex.JMdict.Entities.EntryItems.KanjiFormItems;
using Jitendex.JMdict.Entities.EntryItems.ReadingItems;
using Jitendex.JMdict.Entities.EntryItems.SenseItems;

namespace Jitendex.JMdictAnalysis.Analysis;

internal sealed class Database(JmdictAnalysisContext context)
{
    public void TransferDataFromJmdict()
    {
        context.AttachDatabase(DatabaseFile.JMdict);
        context.Database.ExecuteSqlRaw(TransferCommandText);
    }

    private const string Schema = nameof(DatabaseFile.JMdict);

    /// <remarks>
    /// This assumes that the table names and column names
    /// in the JMdict SQLite database are exactly the same.
    /// </remarks>
    private const string TransferCommandText =
        $"""
        INSERT INTO "{nameof(FileHeader)}"
        SELECT * FROM "{Schema}"."{nameof(FileHeader)}";

        INSERT INTO "{nameof(Sequence)}"
        SELECT * FROM "{Schema}"."{nameof(Sequence)}";

        INSERT INTO "{nameof(ReadingInfoTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(ReadingInfoTag)}";

        INSERT INTO "{nameof(KanjiFormInfoTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(KanjiFormInfoTag)}";

        INSERT INTO "{nameof(PartOfSpeechTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(PartOfSpeechTag)}";

        INSERT INTO "{nameof(FieldTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(FieldTag)}";

        INSERT INTO "{nameof(MiscTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(MiscTag)}";

        INSERT INTO "{nameof(DialectTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(DialectTag)}";

        INSERT INTO "{nameof(GlossType)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(GlossType)}";

        INSERT INTO "{nameof(CrossReferenceType)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(CrossReferenceType)}";

        INSERT INTO "{nameof(LanguageSourceType)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(LanguageSourceType)}";

        INSERT INTO "{nameof(PriorityTag)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(PriorityTag)}";

        INSERT INTO "{nameof(Language)}"
             ( "{nameof(IKeyword.Name)}"
             , "{nameof(IKeyword.OriginFileId)}"
             )
        SELECT * FROM "{Schema}"."{nameof(Language)}";

        INSERT INTO "{nameof(Revision)}"
        SELECT * FROM "{Schema}"."{nameof(Revision)}";

        INSERT INTO "{nameof(Entry)}"
        SELECT * FROM "{Schema}"."{nameof(Entry)}";

        INSERT INTO "{nameof(KanjiForm)}"
        SELECT * FROM "{Schema}"."{nameof(KanjiForm)}";

        INSERT INTO "{nameof(KanjiFormInfo)}"
        SELECT * FROM "{Schema}"."{nameof(KanjiFormInfo)}";

        INSERT INTO "{nameof(KanjiFormPriority)}"
        SELECT * FROM "{Schema}"."{nameof(KanjiFormPriority)}";

        INSERT INTO "{nameof(Reading)}"
        SELECT * FROM "{Schema}"."{nameof(Reading)}";

        INSERT INTO "{nameof(ReadingInfo)}"
        SELECT * FROM "{Schema}"."{nameof(ReadingInfo)}";

        INSERT INTO "{nameof(ReadingPriority)}"
        SELECT * FROM "{Schema}"."{nameof(ReadingPriority)}";

        INSERT INTO "{nameof(Restriction)}"
             ( "{nameof(Restriction.EntryId)}"
             , "{nameof(Restriction.ReadingOrder)}"
             , "{nameof(Restriction.Order)}"
             , "{nameof(Restriction.KanjiFormText)}"
             )
        SELECT * FROM "{Schema}"."{nameof(Restriction)}";

        INSERT INTO "{nameof(Sense)}"
        SELECT * FROM "{Schema}"."{nameof(Sense)}";

        INSERT INTO "{nameof(CrossReference)}"
             ( "{nameof(CrossReference.EntryId)}"
             , "{nameof(CrossReference.SenseOrder)}"
             , "{nameof(CrossReference.Order)}"
             , "{nameof(CrossReference.TypeName)}"
             , "{nameof(CrossReference.Text)}"
             )
        SELECT * FROM "{Schema}"."{nameof(CrossReference)}";

        INSERT INTO "{nameof(Dialect)}"
        SELECT * FROM "{Schema}"."{nameof(Dialect)}";

        INSERT INTO "{nameof(Field)}"
        SELECT * FROM "{Schema}"."{nameof(Field)}";

        INSERT INTO "{nameof(Gloss)}"
        SELECT * FROM "{Schema}"."{nameof(Gloss)}";

        INSERT INTO "{nameof(KanjiFormRestriction)}"
             ( "{nameof(KanjiFormRestriction.EntryId)}"
             , "{nameof(KanjiFormRestriction.SenseOrder)}"
             , "{nameof(KanjiFormRestriction.Order)}"
             , "{nameof(KanjiFormRestriction.KanjiFormText)}"
             )
        SELECT * FROM "{Schema}"."{nameof(KanjiFormRestriction)}";

        INSERT INTO "{nameof(LanguageSource)}"
        SELECT * FROM "{Schema}"."{nameof(LanguageSource)}";

        INSERT INTO "{nameof(Misc)}"
        SELECT * FROM "{Schema}"."{nameof(Misc)}";

        INSERT INTO "{nameof(Note)}"
        SELECT * FROM "{Schema}"."{nameof(Note)}";

        INSERT INTO "{nameof(PartOfSpeech)}"
        SELECT * FROM "{Schema}"."{nameof(PartOfSpeech)}";

        INSERT INTO "{nameof(ReadingRestriction)}"
             ( "{nameof(ReadingRestriction.EntryId)}"
             , "{nameof(ReadingRestriction.SenseOrder)}"
             , "{nameof(ReadingRestriction.Order)}"
             , "{nameof(ReadingRestriction.ReadingText)}"
             )
        SELECT * FROM "{Schema}"."{nameof(ReadingRestriction)}";
        """;
}
