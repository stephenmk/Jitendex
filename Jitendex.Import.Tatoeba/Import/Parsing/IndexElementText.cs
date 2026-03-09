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

internal readonly ref struct IndexElementText
{
    private readonly ReadOnlySpan<char> _text;
    private readonly int _parenIdx;
    private readonly int _squareIdx;
    private readonly int _curlyIdx;
    private readonly bool _isPriority;

    public IndexElementText(ReadOnlySpan<char> text)
    {
        _text = text;
        _parenIdx = text.IndexOf('(');
        _squareIdx = text.IndexOf('[');
        _curlyIdx = text.IndexOf('{');
        _isPriority = text.EndsWith('~');
    }

    public string GetHeadword()
    {
        ReadOnlySpan<char> headword;
        if (_parenIdx > -1)
        {
            headword = _text[.._parenIdx];
        }
        else if (_squareIdx > -1)
        {
            headword = _text[.._squareIdx];
        }
        else if (_curlyIdx > -1)
        {
            headword = _text[.._curlyIdx];
        }
        else if (_isPriority)
        {
            headword = _text[..^1];
        }
        else
        {
            headword = _text;
        }
        return headword.ToString();
    }

    public string? GetReading()
    {
        if (_parenIdx == -1)
        {
            return null;
        }
        else if (_text[_parenIdx..].IndexOf('#') == 1)
        {
            return null;
        }

        int closeIdx = _parenIdx + _text[_parenIdx..].IndexOf(')');
        if (closeIdx < _parenIdx)
        {
            throw new FormatException($"Missing closing parenthesis in text `{_text}`");
        }

        var reading = _text[(_parenIdx + 1)..closeIdx];
        return reading.ToString();
    }

    public int? GetEntryId()
    {
        if (_parenIdx == -1)
        {
            return null;
        }
        else if (_text[_parenIdx..].IndexOf('#') != 1)
        {
            return null;
        }

        int closeIdx = _parenIdx + _text[_parenIdx..].IndexOf(')');
        if (closeIdx < _parenIdx)
        {
            throw new FormatException($"Missing closing parenthesis in text `{_text}`");
        }

        var text = _text[(_parenIdx + 2)..closeIdx];

        if (int.TryParse(text, out int num))
        {
            return num;
        }
        else
        {
            throw new FormatException($"Non-integer entry ID in element `{_text}`");
        }
    }

    public int? GetSenseNumber()
    {
        if (_squareIdx == -1)
        {
            return null;
        }

        int closeIdx = _squareIdx + _text[_squareIdx..].IndexOf(']');
        if (closeIdx < _squareIdx)
        {
            throw new FormatException($"Missing closing square bracket in text `{_text}`");
        }

        var text = _text[(_squareIdx + 1)..closeIdx];

        if (int.TryParse(text, out int num))
        {
            return num;
        }
        else
        {
            throw new FormatException($"Non-integer sense number in element `{_text}`");
        }
    }

    public string? GetSentenceForm()
    {
        if (_curlyIdx == -1)
        {
            return null;
        }

        int closeIdx = _curlyIdx + _text[_curlyIdx..].IndexOf('}');
        if (closeIdx < _curlyIdx)
        {
            throw new FormatException($"Missing closing curly bracket in text `{_text}`");
        }
        var sentenceForm = _text[(_curlyIdx + 1)..closeIdx];
        return sentenceForm.ToString();
    }

    public bool GetIsPriority() => _isPriority;
}
