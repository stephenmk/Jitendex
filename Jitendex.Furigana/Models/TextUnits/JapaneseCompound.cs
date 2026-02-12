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
using Jitendex.Furigana.Models.TextUnits.Readings;

namespace Jitendex.Furigana.Models.TextUnits;

/// <summary>
/// Represents a compound expression and its potential readings.
/// </summary>
/// <remarks>
/// <para>
/// For example, おとな【大人】 can't be cut as [大|お][人|とな] or [大|おと][人|な].
/// It is treated as a single unit: [大人|おとな].
/// </para>
/// <para>
/// A given expression may have multiple potential readings. For example,
/// 発条 could either be [発条|ばね] or [発条|ぜんまい].
/// </para>
/// </remarks>
public sealed class JapaneseCompound : IJapaneseTextUnit<CompoundReading>
{
    public string Text { get; }
    public ImmutableArray<CompoundReading> Readings { get; }

    public JapaneseCompound(string text, IEnumerable<string> readings)
    {
        Text = text;
        Readings = readings
            .Select(KanaTransform.KatakanaToHiragana)
            .Distinct()
            .Select(r => new CompoundReading(this, r))
            .ToImmutableArray();
    }
}
