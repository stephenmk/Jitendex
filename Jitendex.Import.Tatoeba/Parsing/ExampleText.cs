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

namespace Jitendex.Tatoeba.Import.Parsing;

internal readonly ref struct ExampleText
{
    private readonly ReadOnlySpan<char> _lineA;
    private readonly ReadOnlySpan<char> _lineB;

    private readonly int _tabIdx;
    private readonly int _idIdx;
    private readonly int _underscoreIdx;

    private const string _idPrefix = "#ID=";
    private static readonly int _idPrefixLength = _idPrefix.Length;

    public ExampleText(ReadOnlySpan<char> lineA, ReadOnlySpan<char> lineB)
    {
        _lineA = lineA;
        _lineB = lineB;

        _tabIdx = _lineA.IndexOf('\t');
        _idIdx = _lineA.IndexOf(_idPrefix, StringComparison.Ordinal);
        _underscoreIdx = _idIdx > -1
            ? _idIdx + _lineA[_idIdx..].IndexOf("_")
            : -1;
    }

    public string GetExampleText()
    {
        if (_tabIdx == -1)
        {
            throw new FormatException($"Missing tab character in `{_lineA}`");
        }
        return _lineA[.._tabIdx].ToString();
    }

    public string GetTranslationText()
    {
        if (_tabIdx == -1)
        {
            throw new FormatException($"Missing tab character in `{_lineA}`");
        }
        if (_idIdx == -1)
        {
            throw new FormatException($"Missing ID string in `{_lineA}`");
        }
        return _lineA[(_tabIdx + 1).._idIdx].ToString();
    }

    public int GetTranslationId()
    {
        if (_idIdx == -1)
        {
            throw new FormatException($"Missing ID string in `{_lineA}`");
        }

        var text = _lineA[(_idIdx + _idPrefixLength).._underscoreIdx];

        if (int.TryParse(text, out int id))
        {
            return id;
        }
        else
        {
            throw new FormatException($"Non-integer English sentence ID in `{_lineA}`");
        }
    }

    public int GetExampleId()
    {
        if (_underscoreIdx == -1)
        {
            throw new FormatException($"Missing underscore in `{_lineA}`");
        }

        var text = _lineA[(_underscoreIdx + 1)..];

        if (int.TryParse(text, out int id))
        {
            return id;
        }
        else
        {
            throw new FormatException($"Non-integer Japanese sentence ID in `{_lineA}`");
        }
    }

    public MemoryExtensions.SpanSplitEnumerator<char> ElementTextRanges()
        => _lineB.Split(' ');

    public IndexElementText GetElementText(Range range)
        => new(_lineB[range]);
}
