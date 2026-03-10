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

using Microsoft.EntityFrameworkCore;
using Jitendex.Data.JMdict.Entities;
using Jitendex.Data.JMdict.Entities.Text;
using Jitendex.Data.JMdict.Entities.References;
using Jitendex.Data.JMdict.Entities.EntryItems;
using Jitendex.Data.JMdict.Entities.EntryItems.KanjiFormItems;
using Jitendex.Data.JMdict.Entities.EntryItems.Links;
using Jitendex.Data.JMdict.Entities.EntryItems.ReadingItems;
using Jitendex.Data.JMdict.Entities.EntryItems.SenseItems;
using Jitendex.Data.JMdict.Entities.Furigana;

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
    public DbSet<KanjiFormRestriction> KanjiFormRestrictions { get; set; } = null!;
    public DbSet<LanguageSource> LanguageSources { get; set; } = null!;
    public DbSet<Misc> Miscs { get; set; } = null!;
    public DbSet<PartOfSpeech> PartsOfSpeech { get; set; } = null!;
    public DbSet<ReadingRestriction> ReadingRestrictions { get; set; } = null!;
    #endregion

    #region Internal Entry Links
    public DbSet<RestrictionLink> RestrictionLinks { get; set; } = null!;
    public DbSet<ReadingRestrictionLink> ReadingRestrictionLinks { get; set; } = null!;
    public DbSet<KanjiFormRestrictionLink> KanjiFormRestrictionLinks { get; set; } = null!;
    #endregion

    #region External Entry references
    public DbSet<EntryReference> EntryReferences { get; set; } = null!;
    public DbSet<ReadingReference> ReadingReferences { get; set; } = null!;
    public DbSet<KanjiFormReference> KanjiFormReferences { get; set; } = null!;
    public DbSet<AmbiguousReference> AmbiguousReferences { get; set; } = null!;
    #endregion

    #region Furigana Items
    public DbSet<Compound> Compounds { get; set; } = null!;
    public DbSet<CompoundReading> CompoundReadings { get; set; } = null!;
    public DbSet<Character> Characters { get; set; } = null!;
    public DbSet<CharacterReading> CharacterReadings { get; set; } = null!;
    public DbSet<CharacterReadingType> CharacterReadingTypes { get; set; } = null!;
    public DbSet<DerivedCharacterReading> DerivedCharacterReadings { get; set; } = null!;
    public DbSet<DerivedCharacterReadingType> DerivedCharacterReadingTypes { get; set; } = null!;
    public DbSet<ReadingKanjiFormBridge> ReadingKanjiFormBridges { get; set; } = null!;
    #endregion

    #region Keywords
    public DbSet<PriorityTag> PriorityTags { get; set; } = null!;
    public DbSet<ReadingInfoTag> ReadingInfoTags { get; set; } = null!;
    public DbSet<KanjiFormInfoTag> KanjiFormInfoTags { get; set; } = null!;

    public DbSet<PartOfSpeechTag> PartOfSpeechTags { get; set; } = null!;
    public DbSet<FieldTag> FieldTags { get; set; } = null!;
    public DbSet<MiscTag> MiscTags { get; set; } = null!;
    public DbSet<DialectTag> DialectTags { get; set; } = null!;

    public DbSet<GlossType> GlossTypes { get; set; } = null!;
    public DbSet<CrossReferenceType> CrossReferenceTypes { get; set; } = null!;
    public DbSet<LanguageSourceType> LanguageSourceTypes { get; set; } = null!;
    public DbSet<Language> Languages { get; set; } = null!;
    #endregion
}
