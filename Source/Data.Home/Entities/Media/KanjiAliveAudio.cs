// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, KanjiAliveAudio.cs, is part of Jitendex.
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

namespace Jitendex.Data.Home.Entities.Media;

[Table(nameof(KanjiAliveAudio))]
public sealed class KanjiAliveAudio
{
    [Key]
    public required string Filename { get; init; }
    public required int EntryId { get; set; }
    public required string ReadingText { get; set; }
    public required string KanjiFormText { get; set; }
    public required string? Suffix { get; set; }
    public required int? PitchAccent { get; set; }
    public required byte[] FileData { get; set; }
}
