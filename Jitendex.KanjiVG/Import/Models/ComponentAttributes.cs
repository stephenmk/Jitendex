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

namespace Jitendex.KanjiVG.Import.Models;

internal sealed class ComponentAttributes
{
    public required string Id { get; set; }
    public string? Text { get; set; }
    public bool IsVariant { get; set; }
    public bool IsPartial { get; set; }
    public string? Original { get; set; }
    public int? Part { get; set; }
    public int? Number { get; set; }
    public bool IsTradForm { get; set; }
    public bool IsRadicalForm { get; set; }
    public string? Position { get; set; }
    public string? Radical { get; set; }
    public string? Phon { get; set; }
}
