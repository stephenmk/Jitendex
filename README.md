# Jitendex

Jitendex is a free and openly licensed Japanese-to-English
dictionary. The dictionary is provided in multiple file formats,
allowing you to use the data in a variety of dictionary apps.

Currently Jitendex is built solely upon data provided by the
[JMdict project](https://www.edrdg.org/wiki/index.php/JMdict-EDICT_Dictionary_Project),
but additional free and open data sources will be incorporated
in the future. See the [planned features section](#planned-features) for details.

# Installation
See the instructions provided with
[the latest release of Jitendex](https://github.com/stephenmk/Jitendex/releases/latest).

# Features
Jitendex is designed to display information in an organized and easily
readable manner. It uses both color and space to separate chunks of
information, minimizes repetition by merging related categories into
groups, and hides long and often-repeated explanations in the
background. This setup allows readers to retrieve important
information from entries at a glance.

## Example 1: ばね【発条】
Compare Jitendex with the reigning champ of JMdict-based
dictionaries, [Jisho](https://jisho.org/).

Jitendex (viewed in GoldenDict-ng) | Jisho.org
:---: | :--:
![bane_goldendict](https://github.com/stephenmk/Jitendex/assets/8003332/cfa3e210-98d0-4322-9d6d-3d39508093ed) | ![bane_jisho](https://github.com/stephenmk/Jitendex/assets/8003332/95e0d708-7861-4360-b99e-04e399efd317)

 - Jisho writes long explanations of commonly-used categories such
   as "Usually written using kana alone." This same information is
   available in Jitendex by hovering over colorful tags with
   abbreviated names such as "kana."
 - The second and third senses of the word are both nouns, both are
   usually written in kana, and both apply only to ばね and バネ.
   Instead of repeating all of this information, Jitendex places
   both senses at the same indentation level underneath a single
   metadata tag group.
 - jisho provides information regarding variant spellings in
   two separate lists: "Other forms" and "Notes." Readers must strain
   their eyes and play a game of "spot the difference" in order to make
   sense of it. Jitendex displays all of the same information in a tidy
   table.

That said, Jisho is a great resource with many useful features that
Jitendex doesn't even attempt to implement. I'm only picking on it
because it's popular.

## Example 2: いただきます
Compare Jitendex with the original [JMdict file for Yomichan](https://foosoft.net/projects/yomichan/index.html#dictionaries) developed by FooSoft.

Jitendex (viewed in Yomichan) | FooSoft's "JMdict for Yomichan"
:--: | :--:
![itadakimasu_yomi](https://github.com/stephenmk/Jitendex/assets/8003332/1121c2e0-4189-4e3c-a0d0-9f739fdfbead) | ![itadakimasu_yomi_old](https://github.com/stephenmk/Jitendex/assets/8003332/bdceca7c-6aa3-4941-b610-6de79300ab63)

Jitendex includes example sentences, usage notes, cross references, antonym information, language-of-origin details, and
extra metadata (such as the "*literally*" note) that are all missing from FooSoft's version.

## Example 3: けいそつ
I previously
[designed](https://github.com/FooSoft/yomichan/issues/2183)
[and](https://github.com/FooSoft/yomichan-import/pull/40)
[developed](https://github.com/FooSoft/yomichan-import/pull/41)
new versions of JMdict/JMnedict dictionaries for Yomichan which addressed the
[many](https://github.com/FooSoft/yomichan/issues/1165)
[long-standing](https://github.com/FooSoft/yomichan/issues/2057)
[issues](https://github.com/FooSoft/yomichan/issues/1716#issuecomment-1214436766)
with the original version displayed in Example 2 above. This new version was
sometimes referred to as "JMdict Extra" in order to distinguish it from
FooSoft's version. Jitendex is a continuation of that project with support
for more app formats, more ambitious goals, and more design improvements.

Compare the search results for けいそつ from Jitendex and
my previous version of JMdict for Yomichan.

Jitendex (viewed in Yomichan) | stephenmk's "JMdict Extra"
:--: | :--:
![keisotu_jitendex](https://github.com/stephenmk/Jitendex/assets/8003332/ff10c25f-7537-4ee2-98cc-797992c895b8) | ![keisotu_jmdict](https://github.com/stephenmk/Jitendex/assets/8003332/cb25f7c1-db04-43d2-95c9-7a2bdded9b2d)

In Jitendex, you can clearly tell that the 軽卒 form (the bottom
search result) may be found in two different entries. In the first
entry it refers to a type of soldier, and in the second entry it is an
irregular spelling of 軽率 (the top search result).

## Other miscellaneous examples
<details>
  <summary>Entry for 猫の手も借りたい in GoldenDict-ng</summary>

![neko](https://github.com/stephenmk/Jitendex/assets/8003332/afd8826c-1751-49b4-a079-fe7df22596ba)
</details>

<details>
  <summary>Entry for ばね in Yomibaba (compact glossary mode disabled; dark mode; Chromium)</summary>

![bane_chromium](https://github.com/stephenmk/Jitendex/assets/8003332/afb3e1ac-b3f7-441f-b73d-c611b3894e41)
</details>

<details>
  <summary>Entry for 頂く in Yomibaba (compact glossary mode enabled; light mode; Chromium)</summary>

![itadaku](https://github.com/stephenmk/Jitendex/assets/8003332/56530a1b-41de-496d-92ec-286b407895e4)
</details>

<details>
  <summary>Entry for 現在 in Yomibaba (compact glossary mode disabled; dark mode; Firefox)</summary>

![genzai](https://github.com/stephenmk/Jitendex/assets/8003332/f0785bef-d974-4f9e-9c8b-ec504abfe875)
</details>

<details>
  <summary>Entry for 素姓 in Yomibaba (compact glossary mode enabled; dark mode; Firefox)</summary>

![sujou_firefox](https://github.com/stephenmk/Jitendex/assets/8003332/b8059942-ee29-4e90-8035-9001935c5e2e)
</details>

# Planned Features
A few of the following ideas are much more ambitious than the others,
and I can't make any promises on when they all might be implemented.
This is a personal project, so improvements will likely appear
slowly and incrementally over time.

### The Jiten Index
Japanese dictionaries ("jiten") are notoriously inconsistent about
orthography. You may search your dictionary collection for the word
'へそ繰り' and find it in one dictionary, but you will have no way of
knowing that most dictionaries have the word recorded as '臍繰り'. The
conventional solution to this dilemma is to perform a second search
for the reading of the word that you found from your first search
('へそくり' in this case). In this second search, you may have to sift
through unrelated homophones as well as repeats of the entries that
you already viewed during your first search. This process becomes
quite a hassle to perform for every new word that you encounter.

The goal of the Jiten Index is to provide a more convenient solution to
this problem. I envision it as being a separate page linked to every
applicable Jitendex entry. The page will contain a list of other
popular dictionaries and display the forms of a word recorded in
each one. If an entry for a word is known to exist on a particular
website (kotobank, goo.ne.jp, jitenon, etc.), it will contain a
hyperlink to that resource as well.

### Furigana for example sentences
Most of the words that appear in example sentences are indexed to
particular JMdict entries, so it wouldn't be too hard to gather accurate
furigana information for them. The words that aren't indexed (proper nouns,
numbers and counters) would need to be reviewed individually to ensure
correctness.

### Multimedia Support
- Images for select entries sourced from Wikimedia Commons (or other
  free and open image sources).
- Word pronunciation audio provided by the
  [KanjiAlive project](https://github.com/kanjialive/kanji-data-media)
  (or other sources).

### Dictionary data from other EDRDG projects
- Target languages other than English that are included in the JMdict file (Spanish, German, etc.)
- Kanji entries using data provided by [KANJIDIC2](https://www.edrdg.org/wiki/index.php/KANJIDIC_Project).
- Names and proper nouns from [JMnedict](https://www.edrdg.org/enamdict/enamdict_doc.html).

### Dictionary data that is not provided by EDRDG projects
- Free monolingual (Japanese-to-Japanese) definitions are available
  from Wiktionary. It may be possible to produce a version of
  Jitendex with Japanese as a target language using this data.
- There exist free sources of pitch accent data, e.g. from Wiktionary
  and [kanjium](https://github.com/mifunetoshiro/kanjium). Pitch accents
  can vary depending on the context in which a word is used, so they
  would technically need to be associated with each JMdict entry on
  a sense-by-sense basis. Furthermore, the data would need to be
  cross-referenced with modern sources to ensure its accuracy.


# Feedback
Jitendex is targeted at a general audience of users. I may be able to
alter the design of the dictionaries based upon user feedback, but
changes to accommodate extremely specific use-cases (such as custom
frequency lists or flashcard types) will likely not be implemented.


# Legal
© CC BY-SA 4.0 Stephen Kraus 2023

You are free to use, modify, and redistribute Jitendex files under
the terms of the
[Creative Commons Attribution-ShareAlike License (V4.0)](https://creativecommons.org/licenses/by-sa/4.0/).

Jitendex includes material from the JMdict (EDICT, etc.) dictionary
files in accordance with the license provisions of the Electronic
Dictionaries Research Group. Visit
[edrdg.org](https://www.edrdg.org/) for more information.

# Version History
See [VERSION_HISTORY.md](VERSION_HISTORY.md).
