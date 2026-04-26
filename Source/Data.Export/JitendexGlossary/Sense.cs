// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Sense.cs, is part of Jitendex.
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

namespace Jitendex.Data.Export.JitendexGlossary;

public sealed record Sense
(
    int? DisplayNumber,
    ImmutableArray<GlossaryTag> Tags,
    ImmutableArray<string> Notes,
    ImmutableArray<string> Glosses,
    ImmutableArray<string> LiteralGlosses,
    ImmutableArray<string> FigurativeGlosses,
    ImmutableArray<CrossReference> CrossReferences,
    ImmutableArray<Example> Examples,
    ImmutableArray<Graphic> Graphics
);

public sealed record CrossReference
(
    string Class,
    string SurfaceForm,
    ImmutableArray<FuriganaSegment> FuriganaSegments,
    int HeadwordNumber,
    int SenseNumber,
    ImmutableArray<string> Glosses
);

public sealed record Example
(
    string Translation,
    ImmutableArray<ExampleSegment> Segments
);

public sealed record ExampleSegment
(
    string BaseText,
    string? RubyText,
    bool IsHighlighted
);

public sealed record Graphic
(
    string Path
);
