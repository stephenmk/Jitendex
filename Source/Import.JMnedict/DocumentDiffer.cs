/*
Copyright (c) 2026 Stephen Kraus
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

using Jitendex.Import.JMnedict.TableRows;

namespace Jitendex.Import.JMnedict;

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

        FindNew<string>(diff, docA, docB, nameof(Document.PriorityTags));
        FindNew<string>(diff, docA, docB, nameof(Document.ReadingInfoTags));
        FindNew<string>(diff, docA, docB, nameof(Document.KanjiFormInfoTags));
        FindNew<string>(diff, docA, docB, nameof(Document.NameTypeTags));
        FindNew<string>(diff, docA, docB, nameof(Document.DetailLanguages));

        DiffDictionaryProperties<int, EntryElement>(diff, docA, docB, nameof(Document.Entries));

        DiffDictionaryProperties<(int, int), KanjiFormRow>(diff, docA, docB, nameof(Document.KanjiForms));
        DiffDictionaryProperties<(int, int), ReadingRow>(diff, docA, docB, nameof(Document.Readings));
        DiffDictionaryProperties<(int, int), TranslationRow>(diff, docA, docB, nameof(Document.Translations));

        DiffDictionaryProperties<(int, int, int), KanjiFormInfoRow>(diff, docA, docB, nameof(Document.KanjiFormInfos));
        DiffDictionaryProperties<(int, int, int), KanjiFormPriorityRow>(diff, docA, docB, nameof(Document.KanjiFormPriorities));

        DiffDictionaryProperties<(int, int, int), ReadingInfoRow>(diff, docA, docB, nameof(Document.ReadingInfos));
        DiffDictionaryProperties<(int, int, int), ReadingPriorityRow>(diff, docA, docB, nameof(Document.ReadingPriorities));
        DiffDictionaryProperties<(int, int, int), RestrictionRow>(diff, docA, docB, nameof(Document.Restrictions));

        DiffDictionaryProperties<(int, int, int), CrossReferenceRow>(diff, docA, docB, nameof(Document.CrossReferences));
        DiffDictionaryProperties<(int, int, int), DetailRow>(diff, docA, docB, nameof(Document.Details));
        DiffDictionaryProperties<(int, int, int), NameTypeRow>(diff, docA, docB, nameof(Document.NameTypes));

        return diff;
    }
}
