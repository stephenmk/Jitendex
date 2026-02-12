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
using Jitendex.JapaneseTextUtils;

namespace Jitendex.Furigana.Models.TextUnits.Readings;

public sealed class OnReading : CharacterReading
{
    public override string Text { get; }
    public string? SokuonForm { get; }
    public ImmutableArray<string> RendakuReadings { get; }

    /// <remarks>
    /// Not sure if this combination ever actually occurs in practice
    /// </remarks>
    public ImmutableArray<string> RendakuSokuonReadings { get; }

    public OnReading(Kanji character, string text) : base(character, text)
    {
        if (text.Contains('.'))
        {
            throw new ArgumentException("Onyomi must not contain dot splits", nameof(text));
        }

        Text = text.Replace("-", string.Empty).KatakanaToHiragana();
        SokuonForm = Text.ToSokuonForm();
        RendakuReadings = Text.ToRendakuForms();
        RendakuSokuonReadings = SokuonForm?.ToRendakuForms() ?? [];
    }

    public override bool Equals(object? obj)
        => obj is OnReading && base.Equals(obj);

    public override int GetHashCode()
        => HashCode.Combine(typeof(OnReading), base.GetHashCode());
}
