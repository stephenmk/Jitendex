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

using System.Collections.Immutable;
using Jitendex.KanjiVG.Entities;
using Microsoft.Extensions.Logging;

namespace Jitendex.KanjiVG.Import.Readers.Lookups;

internal abstract class LookupCache<T> where T : ILookup
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, int> _textToId;
    private readonly Dictionary<string, T> _cache;
    private readonly bool _doLogUnknown;
    public IEnumerable<T> Values { get => _cache.Values; }

    public LookupCache(ILogger logger)
    {
        _logger = logger;

        var knownLookups = KnownLookups();

        _textToId = knownLookups
            .Select(static (text, index) =>
                new KeyValuePair<string, int>(text, index + 1))
            .ToDictionary();

        _cache = new(_textToId.Count);

        _doLogUnknown = knownLookups.Length > 0;
    }

    public T Get(string text)
    {
        if (_cache.TryGetValue(text, out T? lookup))
        {
            return lookup;
        }

        if (!_textToId.TryGetValue(text, out int id))
        {
            if (_doLogUnknown) LogUnknownLookup(text);
            id = _textToId.Count + 1;
            _textToId[text] = id;
        }

        lookup = NewLookup(id, text);

        _cache.Add(text, lookup);
        return lookup;
    }

    private void LogUnknownLookup(string text)
    {
        _logger.LogWarning
        (
            "Type `{TypeName}` with value `{Text}` has never been seen before",
            typeof(T).Name,
            text
        );
    }

    protected abstract T NewLookup(int id, string text);
    protected abstract ImmutableArray<string> KnownLookups();
}
