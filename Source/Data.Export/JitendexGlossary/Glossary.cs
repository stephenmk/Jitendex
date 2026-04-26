// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Glossary.cs, is part of Jitendex.
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

public sealed record Glossary
(
    ImmutableArray<FuriganaSegment> FuriganaSegments,
    ImmutableArray<GlossaryTag> Tags,
    LanguageSource? LanguageSource,
    ImmutableArray<Pronunciation> Pronunciations,
    ImmutableArray<SenseGroup> SenseGroups,
    ImmutableArray<AlternativeHeadword> OtherReadings,
    ImmutableArray<AlternativeHeadword> OtherSurfaces,
    ImmutableArray<AlternativeHeadword> RelatedForms,
    ImmutableArray<DataSource> DataSources
);
