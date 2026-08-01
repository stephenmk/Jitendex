// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, KanjiAliveAudioTable.cs, is part of Jitendex.
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

using Jitendex.Data;
using Jitendex.Data.Home.Entities.Sound;
using Jitendex.Import.Home.TableRows;

namespace Jitendex.Import.Home.Tables.Media;

internal sealed class AudioTable : Table<KanjiAliveAudioRow>
{
    protected override string Name { get; } = nameof(KanjiAliveAudio);

    protected override ImmutableArray<string> ColumnNames { get; } =
    [
        nameof(KanjiAliveAudio.Filename),
        nameof(KanjiAliveAudio.EntryId),
        nameof(KanjiAliveAudio.ReadingText),
        nameof(KanjiAliveAudio.KanjiFormText),
        nameof(KanjiAliveAudio.Suffix),
        nameof(KanjiAliveAudio.PitchAccent),
        nameof(KanjiAliveAudio.FileData),
    ];

    protected override ImmutableArray<string> KeyColNames { get; } =
    [
        nameof(KanjiAliveAudio.Filename)
    ];

    protected override object?[] ParameterValues(KanjiAliveAudioRow row) =>
    [
        row.Filename,
        row.EntryId,
        row.ReadingText,
        row.KanjiFormText,
        row.Suffix,
        row.PitchAccent,
        row.FileData,
    ];
}
