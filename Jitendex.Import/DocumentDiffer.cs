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

namespace Jitendex.Import;

public abstract class DocumentDiffer<T> : IDocumentDiffer<T>
{
    public abstract IDocumentDiff<T> Diff(IDocument<T> docA, IDocument<T> docB);

    protected void FindNew<TKey, TValue>(IDocumentDiff<T> diff, IDocument<T> docA, IDocument<T> docB, string propertyName) where TKey : notnull
    {
        var prop = docA.GetType().GetProperty(propertyName)!;
        var dictA = (Dictionary<TKey, TValue>)prop.GetValue(docA)!;
        var dictB = (Dictionary<TKey, TValue>)prop.GetValue(docB)!;
        var inserts = (Dictionary<TKey, TValue>)prop.GetValue(diff.Inserts)!;

        foreach (var (key, value) in dictB)
        {
            if (!dictA.ContainsKey(key))
            {
                inserts.Add(key, value);
            }
        }
    }

    protected void DiffDictionaryProperties<TKey, TValue>(IDocumentDiff<T> diff, IDocument<T> docA, IDocument<T> docB, string propertyName)
        where TKey : notnull
        where TValue : notnull
    {
        var prop = docA.GetType().GetProperty(propertyName)!;
        var dictA = (Dictionary<TKey, TValue>)prop.GetValue(docA)!;
        var dictB = (Dictionary<TKey, TValue>)prop.GetValue(docB)!;
        var inserts = (Dictionary<TKey, TValue>)prop.GetValue(diff.Inserts)!;
        var updates = (Dictionary<TKey, TValue>)prop.GetValue(diff.Updates)!;
        var deletes = (Dictionary<TKey, TValue>)prop.GetValue(diff.Deletes)!;
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
