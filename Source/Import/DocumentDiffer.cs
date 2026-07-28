// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DocumentDiffer.cs, is part of Jitendex.
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

namespace Jitendex.Import;

public abstract class DocumentDiffer<TKey, TDocument, TDiff> : IDocumentDiffer<TKey, TDocument, TDiff>
    where TDocument : IDocument<TKey>
    where TDiff : IDocumentDiff<TKey, TDocument>
{
    public abstract TDiff Diff(TDocument docA, TDocument docB);

    protected void FindNew<T>(TDiff diff, TDocument docA, TDocument docB, string propertyName)
        where T : notnull
    {
        var prop = typeof(TDocument).GetProperty(propertyName)!;
        var setA = (HashSet<T>)prop.GetValue(docA)!;
        var setB = (HashSet<T>)prop.GetValue(docB)!;
        var inserts = (HashSet<T>)prop.GetValue(diff.Upserts)!;

        foreach (var value in setB)
        {
            if (!setA.Contains(value))
            {
                inserts.Add(value);
            }
        }
    }

    protected void Diff<T1, T2>(TDiff diff, TDocument docA, TDocument docB, string propertyName)
        where T1 : struct
        where T2 : notnull
    {
        var prop = typeof(TDocument).GetProperty(propertyName)!;
        var dictA = (Dictionary<T1, T2>)prop.GetValue(docA)!;
        var dictB = (Dictionary<T1, T2>)prop.GetValue(docB)!;
        var upserts = (Dictionary<T1, T2>)prop.GetValue(diff.Upserts)!;
        var deletes = (Dictionary<T1, T2>)prop.GetValue(diff.Deletes)!;
        var comparer = EqualityComparer<T2>.Default;

        foreach (var (key, valueA) in dictA)
        {
            if (!dictB.TryGetValue(key, out var valueB))
            {
                deletes.Add(key, valueA);
            }
            else if (!comparer.Equals(valueA, valueB))  // Hot spot!!!
            {
                upserts.Add(key, valueB);
            }
        }
        foreach (var (key, valueB) in dictB)
        {
            if (!dictA.ContainsKey(key))
            {
                upserts.Add(key, valueB);
            }
        }
    }
}
