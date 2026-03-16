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

using Jitendex.Import.JMdict.Models;

namespace Jitendex.Import.JMdict;

internal sealed class DocumentDiffer : DocumentDiffer<DateOnly, Document, DocumentDiff>
{
    public override DocumentDiff Diff(Document docA, Document docB)
    {
        var diff = new DocumentDiff
        {
            ArchiveKey = docB.ArchiveKey,
            Inserts = new Document(0) { ArchiveKey = docB.ArchiveKey },
            Updates = new Document(0) { ArchiveKey = docB.ArchiveKey },
            Deletes = new Document(0) { ArchiveKey = docB.ArchiveKey },
        };

        FindNew<string>(diff, docA, docB, nameof(Document.ReadingInfoTags));
        FindNew<string>(diff, docA, docB, nameof(Document.KanjiFormInfoTags));
        FindNew<string>(diff, docA, docB, nameof(Document.PartOfSpeechTags));
        FindNew<string>(diff, docA, docB, nameof(Document.FieldTags));
        FindNew<string>(diff, docA, docB, nameof(Document.MiscTags));
        FindNew<string>(diff, docA, docB, nameof(Document.DialectTags));
        FindNew<string>(diff, docA, docB, nameof(Document.GlossTypes));
        FindNew<string>(diff, docA, docB, nameof(Document.CrossReferenceTypes));
        FindNew<string>(diff, docA, docB, nameof(Document.LanguageSourceTypes));
        FindNew<string>(diff, docA, docB, nameof(Document.PriorityTags));
        FindNew<string>(diff, docA, docB, nameof(Document.Languages));

        DiffDictionaryProperties<int, EntryElement>(diff, docA, docB, nameof(Document.Entries));

        DiffDictionaryProperties<(int, int), KanjiFormElement>(diff, docA, docB, nameof(Document.KanjiForms));
        DiffDictionaryProperties<(int, int), ReadingElement>(diff, docA, docB, nameof(Document.Readings));
        DiffDictionaryProperties<(int, int), SenseElement>(diff, docA, docB, nameof(Document.Senses));

        DiffDictionaryProperties<(int, int, int), KanjiFormInfoElement>(diff, docA, docB, nameof(Document.KanjiFormInfos));
        DiffDictionaryProperties<(int, int, int), KanjiFormPriorityElement>(diff, docA, docB, nameof(Document.KanjiFormPriorities));

        DiffDictionaryProperties<(int, int, int), ReadingInfoElement>(diff, docA, docB, nameof(Document.ReadingInfos));
        DiffDictionaryProperties<(int, int, int), ReadingPriorityElement>(diff, docA, docB, nameof(Document.ReadingPriorities));
        DiffDictionaryProperties<(int, int, int), RestrictionElement>(diff, docA, docB, nameof(Document.Restrictions));

        DiffDictionaryProperties<(int, int, int), CrossReferenceElement>(diff, docA, docB, nameof(Document.CrossReferences));
        DiffDictionaryProperties<(int, int, int), DialectElement>(diff, docA, docB, nameof(Document.Dialects));
        DiffDictionaryProperties<(int, int, int), FieldElement>(diff, docA, docB, nameof(Document.Fields));
        DiffDictionaryProperties<(int, int, int), GlossElement>(diff, docA, docB, nameof(Document.Glosses));
        DiffDictionaryProperties<(int, int, int), GlossTypeNameElement>(diff, docA, docB, nameof(Document.GlossTypeNames));
        DiffDictionaryProperties<(int, int, int), KanjiFormRestrictionElement>(diff, docA, docB, nameof(Document.KanjiFormRestrictions));
        DiffDictionaryProperties<(int, int, int), LanguageSourceElement>(diff, docA, docB, nameof(Document.LanguageSources));
        DiffDictionaryProperties<(int, int, int), MiscElement>(diff, docA, docB, nameof(Document.Miscs));
        DiffDictionaryProperties<(int, int, int), NoteElement>(diff, docA, docB, nameof(Document.Notes));
        DiffDictionaryProperties<(int, int, int), PartOfSpeechElement>(diff, docA, docB, nameof(Document.PartsOfSpeech));
        DiffDictionaryProperties<(int, int, int), ReadingRestrictionElement>(diff, docA, docB, nameof(Document.ReadingRestrictions));

        return diff;
    }
}
