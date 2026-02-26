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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jitendex.Chise.Entities;

[Table(nameof(Codepoint))]
public class Codepoint
{
    [Key]
    public required string Id { get; init; }
    public required int? UnicodeScalarValue { get; init; }
    public required string? SequenceText { get; init; }
    public required string? AltSequenceText { get; init; }

    [ForeignKey(nameof(UnicodeScalarValue))]
    public required UnicodeCharacter? UnicodeCharacter { get; init; }

    [ForeignKey(nameof(SequenceText))]
    public required Sequence? Sequence { get; init; }

    [ForeignKey(nameof(AltSequenceText))]
    public required Sequence? AltSequence { get; init; }

    [InverseProperty(nameof(Component.Codepoint))]
    public List<Component> Components { get; } = [];

    public string ToCharacter() => UnicodeCharacter?.Character().ToString() ?? Id;
}
