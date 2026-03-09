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

using Jitendex.Import.Kanjidic2.Models;

namespace Jitendex.Import.Kanjidic2;

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

        FindNew<string>(diff, docA, docB, propertyName: nameof(Document.CodepointTypes));
        FindNew<string>(diff, docA, docB, propertyName: nameof(Document.DictionaryTypes));
        FindNew<string>(diff, docA, docB, propertyName: nameof(Document.QueryCodeTypes));
        FindNew<string>(diff, docA, docB, propertyName: nameof(Document.MisclassificationTypes));
        FindNew<string>(diff, docA, docB, propertyName: nameof(Document.RadicalTypes));
        FindNew<string>(diff, docA, docB, propertyName: nameof(Document.ReadingTypes));
        FindNew<string>(diff, docA, docB, propertyName: nameof(Document.VariantTypes));

        DiffDictionaryProperties<int, EntryElement>(diff, docA, docB, propertyName: nameof(Document.Entries));

        DiffDictionaryProperties<(int, int), CodepointGroupElement>(diff, docA, docB, propertyName: nameof(Document.CodepointGroups));
        DiffDictionaryProperties<(int, int), DictionaryGroupElement>(diff, docA, docB, propertyName: nameof(Document.DictionaryGroups));
        DiffDictionaryProperties<(int, int), MiscGroupElement>(diff, docA, docB, propertyName: nameof(Document.MiscGroups));
        DiffDictionaryProperties<(int, int), QueryCodeGroupElement>(diff, docA, docB, propertyName: nameof(Document.QueryCodeGroups));
        DiffDictionaryProperties<(int, int), RadicalGroupElement>(diff, docA, docB, propertyName: nameof(Document.RadicalGroups));
        DiffDictionaryProperties<(int, int), ReadingMeaningGroupElement>(diff, docA, docB, propertyName: nameof(Document.ReadingMeaningGroups));

        DiffDictionaryProperties<(int, int, int), CodepointElement>(diff, docA, docB, propertyName: nameof(Document.Codepoints));
        DiffDictionaryProperties<(int, int, int), DictionaryElement>(diff, docA, docB, propertyName: nameof(Document.Dictionaries));
        DiffDictionaryProperties<(int, int, int), NanoriElement>(diff, docA, docB, propertyName: nameof(Document.Nanoris));
        DiffDictionaryProperties<(int, int, int), QueryCodeElement>(diff, docA, docB, propertyName: nameof(Document.QueryCodes));
        DiffDictionaryProperties<(int, int, int), RadicalElement>(diff, docA, docB, propertyName: nameof(Document.Radicals));
        DiffDictionaryProperties<(int, int, int), RadicalNameElement>(diff, docA, docB, propertyName: nameof(Document.RadicalNames));
        DiffDictionaryProperties<(int, int, int), ReadingMeaningElement>(diff, docA, docB, propertyName: nameof(Document.ReadingMeanings));
        DiffDictionaryProperties<(int, int, int), StrokeCountElement>(diff, docA, docB, propertyName: nameof(Document.StrokeCounts));
        DiffDictionaryProperties<(int, int, int), VariantElement>(diff, docA, docB, propertyName: nameof(Document.Variants));

        DiffDictionaryProperties<(int, int, int, int), MeaningElement>(diff, docA, docB, propertyName: nameof(Document.Meanings));
        DiffDictionaryProperties<(int, int, int, int), ReadingElement>(diff, docA, docB, propertyName: nameof(Document.Readings));

        return diff;
    }
}
