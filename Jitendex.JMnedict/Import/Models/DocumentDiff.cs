/*
Copyright (c) 2026 Stephen Kraus
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

namespace Jitendex.JMnedict.Import.Models;

internal sealed class DocumentDiff
{
    public DocumentHeader FileHeader { get; init; }
    public Document InsertDocument { get; init; }
    public Document UpdateDocument { get; init; }
    public Document DeleteDocument { get; init; }
    public IReadOnlySet<int> SequenceIds { get; init; }

    public DocumentDiff(Document docA, Document docB)
    {
        FileHeader = docB.Header;
        InsertDocument = new Document(0) { Header = docB.Header };
        UpdateDocument = new Document(0) { Header = docB.Header };
        DeleteDocument = new Document(0) { Header = docB.Header };

        FindNew<string, PriorityTagElement>(docA, docB, nameof(Document.PriorityTags));
        FindNew<string, ReadingInfoTagElement>(docA, docB, nameof(Document.ReadingInfoTags));
        FindNew<string, KanjiFormInfoTagElement>(docA, docB, nameof(Document.KanjiFormInfoTags));
        FindNew<string, NameTypeTagElement>(docA, docB, nameof(Document.NameTypeTags));

        DiffDictionaryProperties<int, EntryElement>(docA, docB, nameof(Document.Entries));

        DiffDictionaryProperties<(int, int), KanjiFormElement>(docA, docB, nameof(Document.KanjiForms));
        DiffDictionaryProperties<(int, int), ReadingElement>(docA, docB, nameof(Document.Readings));
        DiffDictionaryProperties<(int, int), TranslationElement>(docA, docB, nameof(Document.Translations));

        DiffDictionaryProperties<(int, int, int), KanjiFormInfoElement>(docA, docB, nameof(Document.KanjiFormInfos));
        DiffDictionaryProperties<(int, int, int), KanjiFormPriorityElement>(docA, docB, nameof(Document.KanjiFormPriorities));

        DiffDictionaryProperties<(int, int, int), ReadingInfoElement>(docA, docB, nameof(Document.ReadingInfos));
        DiffDictionaryProperties<(int, int, int), ReadingPriorityElement>(docA, docB, nameof(Document.ReadingPriorities));
        DiffDictionaryProperties<(int, int, int), RestrictionElement>(docA, docB, nameof(Document.Restrictions));

        DiffDictionaryProperties<(int, int, int), CrossReferenceElement>(docA, docB, nameof(Document.CrossReferences));
        DiffDictionaryProperties<(int, int, int), DetailElement>(docA, docB, nameof(Document.Details));
        DiffDictionaryProperties<(int, int, int), NameTypeElement>(docA, docB, nameof(Document.NameTypes));

        SequenceIds = InsertDocument.ConcatAllEntryIds()
            .Concat(UpdateDocument.ConcatAllEntryIds())
            .Concat(DeleteDocument.ConcatAllEntryIds())
            .ToHashSet();
    }

    private void FindNew<TKey, TValue>(Document docA, Document docB, string propertyName) where TKey : notnull
    {
        var prop = typeof(Document).GetProperty(propertyName)!;
        var dictA = (Dictionary<TKey, TValue>)prop.GetValue(docA)!;
        var dictB = (Dictionary<TKey, TValue>)prop.GetValue(docB)!;
        var inserts = (Dictionary<TKey, TValue>)prop.GetValue(InsertDocument)!;

        foreach (var (key, value) in dictB)
        {
            if (!dictA.ContainsKey(key))
            {
                inserts.Add(key, value);
            }
        }
    }

    private void DiffDictionaryProperties<TKey, TValue>(Document docA, Document docB, string propertyName)
        where TKey : notnull
        where TValue : notnull
    {
        var prop = typeof(Document).GetProperty(propertyName)!;
        var dictA = (Dictionary<TKey, TValue>)prop.GetValue(docA)!;
        var dictB = (Dictionary<TKey, TValue>)prop.GetValue(docB)!;
        var inserts = (Dictionary<TKey, TValue>)prop.GetValue(InsertDocument)!;
        var updates = (Dictionary<TKey, TValue>)prop.GetValue(UpdateDocument)!;
        var deletes = (Dictionary<TKey, TValue>)prop.GetValue(DeleteDocument)!;
        var comparer = EqualityComparer<TValue>.Default;

        foreach (var (key, valueA) in dictA)
        {
            if (!dictB.TryGetValue(key, out var valueB))
            {
                deletes.Add(key, valueA);
            }
            else if (!comparer.Equals(valueA, valueB))  // Hot spot!!!
            {
                updates.Add(key, valueB);
            }
        }
        foreach (var (key, valueB) in dictB)
        {
            if (!dictA.ContainsKey(key))
            {
                inserts.Add(key, valueB);
            }
        }
    }
}
