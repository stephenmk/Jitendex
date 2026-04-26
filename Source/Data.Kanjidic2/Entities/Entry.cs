// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Entry.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Jitendex.Data.Kanjidic2.Entities.Groups;

namespace Jitendex.Data.Kanjidic2.Entities;

[Table(nameof(Entry))]
public sealed class Entry
{
    [Key]
    public required int UnicodeScalarValue { get; init; }
    public List<CodepointGroup> CodepointGroups { get; init; } = [];
    public List<DictionaryGroup> DictionaryGroups { get; init; } = [];
    public List<MiscGroup> MiscGroups { get; init; } = [];
    public List<QueryCodeGroup> QueryCodeGroups { get; init; } = [];
    public List<RadicalGroup> RadicalGroups { get; init; } = [];
    public List<ReadingMeaningGroup> ReadingMeaningGroups { get; init; } = [];

    [ForeignKey(nameof(UnicodeScalarValue))]
    public required Sequence Sequence { get; init; }
}
