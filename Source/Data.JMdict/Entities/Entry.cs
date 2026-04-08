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
using Jitendex.Data.JMdict.Entities.EntryChildren;
using Jitendex.Data.JMdict.ForkEntities.Headwords;

namespace Jitendex.Data.JMdict.Entities;

[Table(nameof(Entry))]
public sealed class Entry
{
    public required int Id { get; init; }
    public List<Reading> Readings { get; init; } = [];
    public List<KanjiForm> KanjiForms { get; init; } = [];
    public List<Sense> Senses { get; init; } = [];

    [ForeignKey(nameof(Id))]
    public Sequence Sequence { get; init; } = null!;

    #region Fork Properties

    [InverseProperty(nameof(Headword.Entry))]
    public List<Headword> Headwords { get; init; } = [];

    #endregion
}
