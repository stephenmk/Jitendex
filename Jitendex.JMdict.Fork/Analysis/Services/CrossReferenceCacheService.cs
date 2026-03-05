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

using System.Collections.Frozen;
using Jitendex.MiscData;

namespace Jitendex.JMdict.Fork.Analysis.Services;

internal sealed class CrossReferenceCacheService
(
    MiscDataContext miscContext,
    JMdictForkContext forkContext
)
{
    public FrozenDictionary<string, int?> Load()
        => miscContext.CrossReferenceSequences
            .Select(static x => new
            {
                Key = $"{x.EntryId}・{x.SenseNumber}・{x.Text}",
                Value = x.RefEntryId,
            })
            .ToFrozenDictionary(static x => x.Key, static x => x.Value);

    private sealed record Key(int EntryId, int SenseNumber, string Text);

    public void Export()
    {
        var dictionary = forkContext.CrossReferences
            .Where(static x => x.IsAmbiguous == true)
            .Select(static x => new
            {
                Key = new Key(x.EntryId, x.SenseOrder + 1, x.Text),
                Value = x.RefEntryId
            })
            .ToDictionary(static x => x.Key, static x => x.Value);

        var hashset = new HashSet<Key>(dictionary.Count);

        foreach (var xref in miscContext.CrossReferenceSequences)
        {
            var key = new Key(xref.EntryId, xref.SenseNumber, xref.Text);
            if (dictionary.TryGetValue(key, out var value))
            {
                hashset.Add(key);
                if (xref.RefEntryId != value)
                {
                    xref.RefEntryId = value;
                }
            }
            else
            {
                miscContext.Remove(xref);
            }
        }

        foreach (var (key, value) in dictionary)
        {
            if (hashset.Contains(key))
            {
                continue;
            }
            miscContext.CrossReferenceSequences.Add(new()
            {
                EntryId = key.EntryId,
                SenseNumber = key.SenseNumber,
                Text = key.Text,
                RefEntryId = value,
            });
        }

        miscContext.SaveChanges();
    }
}
