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

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.JMnedict.Entities.EntryChildren.TranslationChildren;

[Table(nameof(Detail))]
[PrimaryKey(nameof(EntryId), nameof(TranslationOrder), nameof(Order))]
public sealed class Detail
{
    public required int EntryId { get; init; }
    public required int TranslationOrder { get; init; }
    public required int Order { get; init; }
    public required string Text { get; set; }
    public required string? LanguageName { get; set; }

    [ForeignKey($"{nameof(EntryId)}, {nameof(TranslationOrder)}")]
    public Translation Translation { get; init; } = null!;

    [ForeignKey(nameof(LanguageName))]
    public DetailLanguage? Language { get; init; } = null!;
}
