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

namespace Jitendex.Data.Export.JitendexGlossary;

public sealed record SenseGroup
(
    int? DisplayNumber,
    ImmutableArray<GlossaryTag> SenseTags,
    ImmutableArray<Sense> Senses
);

public sealed record FuriganaSegment
(
    string BaseText,
    string? RubyText
);

public sealed record AlternativeHeadword
(
    ImmutableArray<FuriganaSegment> FuriganaSegments,
    int Number,
    int Total,
    ImmutableArray<int> RestrictedSenseNumbers,
    ImmutableArray<GlossaryTag> Tags
);

public sealed record LanguageSource
(
    string LanguageName
);

public sealed record Pronunciation
(
    string Text
);

public sealed record GlossaryTag
(
    string Class,
    string Code,
    string DisplayText,
    string Description
);

public sealed record DataSource
(
    string Prefix,
    string AnchorText,
    string Href
);
