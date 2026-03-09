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

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.Tatoeba.Entities;

[Table(nameof(Segmentation))]
[PrimaryKey(nameof(ExampleId), nameof(Order))]
public sealed class Segmentation
{
    public required int ExampleId { get; init; }
    public required int Order { get; init; }
    public required int TranslationId { get; set; }

    [ForeignKey(nameof(ExampleId))]
    public required Example Example { get; init; }

    [ForeignKey(nameof(TranslationId))]
    public required Translation Translation { get; set; }

    [InverseProperty(nameof(Token.Segmentation))]
    public List<Token> Tokens { get; init; } = [];
}
