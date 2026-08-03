// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, JMdictForkContext.cs, is part of Jitendex.
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

using Jitendex.Data.JMdict.Entities;
using Jitendex.Data.JMdict.Entities.EntryChildren;
using Jitendex.Data.JMdict.Entities.EntryChildren.KanjiFormChildren;
using Jitendex.Data.JMdict.Entities.EntryChildren.ReadingChildren;
using Jitendex.Data.JMdict.Entities.EntryChildren.SenseChildren;
using Jitendex.Data.JMdict.ForkEntities;
using Jitendex.Data.JMdict.ForkEntities.Furigana;
using Jitendex.Data.JMdict.ForkEntities.Headwords;
using Jitendex.Data.JMdict.ForkEntities.Kanwa;
using Jitendex.Data.JMdict.ForkEntities.Links;
using Jitendex.Data.JMdict.ForkEntities.Media;
using Jitendex.Data.JMdict.ForkEntities.References;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Data.JMdict;

public class JMdictForkContext() : SqliteContext(DatabaseFile.JMdictFork)
{
    public DbSet<FileHeader> FileHeaders { get; set; } = null!;
    public DbSet<Sequence> Sequences { get; set; } = null!;
    public DbSet<Revision> Revisions { get; set; } = null!;
    public DbSet<Entry> Entries { get; set; } = null!;

    #region Entry Items
    public DbSet<KanjiForm> KanjiForms { get; set; } = null!;
    public DbSet<Reading> Readings { get; set; } = null!;
    public DbSet<Sense> Senses { get; set; } = null!;
    #endregion

    #region Kanji Form Items
    public DbSet<KanjiFormInfo> KanjiFormInfos { get; set; } = null!;
    public DbSet<KanjiFormPriority> KanjiFormPriorities { get; set; } = null!;
    #endregion

    #region Reading Items
    public DbSet<ReadingInfo> ReadingInfos { get; set; } = null!;
    public DbSet<ReadingPriority> ReadingPriorities { get; set; } = null!;
    public DbSet<Restriction> Restrictions { get; set; } = null!;
    #endregion

    #region Sense Items
    public DbSet<CrossReference> CrossReferences { get; set; } = null!;
    public DbSet<Dialect> Dialects { get; set; } = null!;
    public DbSet<Field> Fields { get; set; } = null!;
    public DbSet<Gloss> Glosses { get; set; } = null!;
    public DbSet<GlossType> GlossTypes { get; set; } = null!;
    public DbSet<KanjiFormRestriction> KanjiFormRestrictions { get; set; } = null!;
    public DbSet<LanguageSource> LanguageSources { get; set; } = null!;
    public DbSet<Misc> Miscs { get; set; } = null!;
    public DbSet<SenseNote> Notes { get; set; } = null!;
    public DbSet<PartOfSpeech> PartsOfSpeech { get; set; } = null!;
    public DbSet<ReadingRestriction> ReadingRestrictions { get; set; } = null!;
    #endregion

    #region Internal Entry Restriction Links
    public DbSet<ReadingRestrictionLink> ReadingRestrictionLinks { get; set; } = null!;
    public DbSet<KanjiFormRestrictionLink> KanjiFormRestrictionLinks { get; set; } = null!;
    public DbSet<RestrictionLink> RestrictionLinks { get; set; } = null!;
    public DbSet<ReadingKanjiFormBridge> ReadingKanjiFormBridges { get; set; } = null!;
    #endregion

    #region External Entry References
    public DbSet<EntryReference> EntryReferences { get; set; } = null!;
    public DbSet<ReadingReference> ReadingReferences { get; set; } = null!;
    public DbSet<KanjiFormReference> KanjiFormReferences { get; set; } = null!;
    #endregion

    #region Furigana Items
    public DbSet<FuriganaSegment> FuriganaSegments { get; set; } = null!;
    public DbSet<CharacterReadingLink> CharacterReadingLinks { get; set; } = null!;
    public DbSet<CompoundReadingLink> CompoundReadingLinks { get; set; } = null!;
    #endregion

    #region Kanwa Items
    public DbSet<Compound> Compounds { get; set; } = null!;
    public DbSet<CompoundReading> CompoundReadings { get; set; } = null!;
    public DbSet<CompoundReadingType> CompoundReadingTypes { get; set; } = null!;
    public DbSet<Character> Characters { get; set; } = null!;
    public DbSet<CharacterReading> CharacterReadings { get; set; } = null!;
    public DbSet<CharacterReadingType> CharacterReadingTypes { get; set; } = null!;
    public DbSet<DerivedCharacterReading> DerivedCharacterReadings { get; set; } = null!;
    public DbSet<DerivedCharacterReadingType> DerivedCharacterReadingTypes { get; set; } = null!;
    public DbSet<CharacterVariant> CharacterVariants { get; set; } = null!;
    public DbSet<VariantType> VariantTypes { get; set; } = null!;
    #endregion

    #region Media
    public DbSet<Graphic> Graphics { get; set; } = null!;
    public DbSet<License> GraphicLicenses { get; set; } = null!;
    public DbSet<SenseGraphic> SenseGraphics { get; set; } = null!;
    #endregion

    #region Headwords
    public DbSet<Headword> Headwords { get; set; } = null!;
    public DbSet<HeadwordRedirect> HeadwordRedirects { get; set; } = null!;
    public DbSet<HeadwordReference> HeadwordReferences { get; set; } = null!;
    public DbSet<HeadwordRule> HeadwordRules { get; set; } = null!;
    public DbSet<HeadwordSense> HeadwordSenses { get; set; } = null!;
    public DbSet<HeadwordTag> HeadwordTags { get; set; } = null!;
    #endregion

    #region Keywords
    public DbSet<PriorityTag> PriorityTags { get; set; } = null!;
    public DbSet<ReadingInfoTag> ReadingInfoTags { get; set; } = null!;
    public DbSet<KanjiFormInfoTag> KanjiFormInfoTags { get; set; } = null!;

    public DbSet<PartOfSpeechTag> PartOfSpeechTags { get; set; } = null!;
    public DbSet<FieldTag> FieldTags { get; set; } = null!;
    public DbSet<MiscTag> MiscTags { get; set; } = null!;
    public DbSet<DialectTag> DialectTags { get; set; } = null!;

    public DbSet<GlossTypeTag> GlossTypeTags { get; set; } = null!;
    public DbSet<CrossReferenceType> CrossReferenceTypes { get; set; } = null!;
    public DbSet<LanguageSourceType> LanguageSourceTypes { get; set; } = null!;
    public DbSet<Language> Languages { get; set; } = null!;
    #endregion

    public DbSet<Patch> Patches { get; set; } = null!;
}
