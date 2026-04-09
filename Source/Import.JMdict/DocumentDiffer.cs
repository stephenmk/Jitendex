/*
Copyright (c) 2025-2026 Stephen Kraus
SPDX-License-Identifier: AGPL-3.0-or-later

This file is part of Jitendex.

Jitendex is free software: you can redistribute it and/or modify it under the terms
of the GNU Affero General Public License as published by the Free Software Foundation,
either version 3 of the License or (at your option) any later version.

Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with Jitendex.
If not, see <https://www.gnu.org/licenses/>.
*/

using Jitendex.Import.JMdict.TableRows;

namespace Jitendex.Import.JMdict;

internal sealed class DocumentDiffer : DocumentDiffer<DateOnly, Document, DocumentDiff>
{
    public override DocumentDiff Diff(Document docA, Document docB)
    {
        var diff = new DocumentDiff
        {
            ArchiveKey = docB.ArchiveKey,
            Upserts = new Document(0) { ArchiveKey = docB.ArchiveKey },
            Deletes = new Document(0) { ArchiveKey = docB.ArchiveKey },
        };

        FindNew<string>(diff, docA, docB, nameof(Document.ReadingInfoTags));
        FindNew<string>(diff, docA, docB, nameof(Document.KanjiFormInfoTags));
        FindNew<string>(diff, docA, docB, nameof(Document.PartOfSpeechTags));
        FindNew<string>(diff, docA, docB, nameof(Document.FieldTags));
        FindNew<string>(diff, docA, docB, nameof(Document.MiscTags));
        FindNew<string>(diff, docA, docB, nameof(Document.DialectTags));
        FindNew<string>(diff, docA, docB, nameof(Document.GlossTypeTags));
        FindNew<string>(diff, docA, docB, nameof(Document.CrossReferenceTypes));
        FindNew<string>(diff, docA, docB, nameof(Document.LanguageSourceTypes));
        FindNew<string>(diff, docA, docB, nameof(Document.PriorityTags));
        FindNew<string>(diff, docA, docB, nameof(Document.Languages));

        Diff<int, EntryRow>(diff, docA, docB, nameof(Document.Entries));

        #pragma warning disable format

        Diff<(int, int), KanjiFormRow>                (diff, docA, docB, nameof(Document.KanjiForms));
        Diff<(int, int), ReadingRow>                  (diff, docA, docB, nameof(Document.Readings));
        Diff<(int, int), SenseRow>                    (diff, docA, docB, nameof(Document.Senses));

        Diff<(int, int, int), KanjiFormInfoRow>       (diff, docA, docB, nameof(Document.KanjiFormInfos));
        Diff<(int, int, int), KanjiFormPriorityRow>   (diff, docA, docB, nameof(Document.KanjiFormPriorities));

        Diff<(int, int, int), ReadingInfoRow>         (diff, docA, docB, nameof(Document.ReadingInfos));
        Diff<(int, int, int), ReadingPriorityRow>     (diff, docA, docB, nameof(Document.ReadingPriorities));
        Diff<(int, int, int), RestrictionRow>         (diff, docA, docB, nameof(Document.Restrictions));

        Diff<(int, int, int), CrossReferenceRow>      (diff, docA, docB, nameof(Document.CrossReferences));
        Diff<(int, int, int), DialectRow>             (diff, docA, docB, nameof(Document.Dialects));
        Diff<(int, int, int), FieldRow>               (diff, docA, docB, nameof(Document.Fields));
        Diff<(int, int, int), GlossRow>               (diff, docA, docB, nameof(Document.Glosses));
        Diff<(int, int, int), GlossTypeRow>           (diff, docA, docB, nameof(Document.GlossTypes));
        Diff<(int, int, int), KanjiFormRestrictionRow>(diff, docA, docB, nameof(Document.KanjiFormRestrictions));
        Diff<(int, int, int), LanguageSourceRow>      (diff, docA, docB, nameof(Document.LanguageSources));
        Diff<(int, int, int), MiscRow>                (diff, docA, docB, nameof(Document.Miscs));
        Diff<(int, int, int), NoteRow>                (diff, docA, docB, nameof(Document.Notes));
        Diff<(int, int, int), PartOfSpeechRow>        (diff, docA, docB, nameof(Document.PartsOfSpeech));
        Diff<(int, int, int), ReadingRestrictionRow>  (diff, docA, docB, nameof(Document.ReadingRestrictions));

        #pragma warning restore format

        return diff;
    }
}
