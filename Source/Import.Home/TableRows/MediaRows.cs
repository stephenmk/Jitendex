// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, MediaRows.cs, is part of Jitendex.
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

namespace Jitendex.Import.Home.TableRows;

internal sealed record GraphicRow
(
    int Id,
    int LicenseId,
    bool Cropped,
    string PageUrl,
    string FileUrl,
    string Author,
    string? AuthorUrl,
    string? Title,
    byte[] FileData
);

internal sealed record KanjiAliveAudioRow
(
    string Filename,
    int EntryId,
    string ReadingText,
    string KanjiFormText,
    string? Suffix,
    int? PitchAccent,
    byte[] FileData
);
