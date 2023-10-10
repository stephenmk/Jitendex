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
Compare Jitendex (**left**) with the reigning champ of JMdict-based
dictionaries, [Jisho](https://jisho.org/) (**right**).

![bane_jisho](https://github.com/stephenmk/Jitendex/assets/8003332/7d7f06d5-021f-43e7-a3b0-1d168e23100d)


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
Compare Jitendex (**left**) with the original [JMdict file for Yomichan](https://foosoft.net/projects/yomichan/index.html#dictionaries)
(**right**) developed by FooSoft.

![itadakimasu_firefox_light_noncompact](https://github.com/stephenmk/Jitendex/assets/8003332/adc87ae2-91a9-4bc2-acf6-9de8bdfc1fc8)

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
with the original version displayed in Example 2 above. Jitendex
is a continuation of that project with support for more app formats,
more ambitious goals, and more design improvements.

Compare the search results for けいそつ from Jitendex (**left**) and
my previous version of JMdict for Yomichan (**right**).

![keisotu_jitendex](https://github.com/stephenmk/Jitendex/assets/8003332/91c2d82c-7b94-4a1a-9a73-949ae324b817)

In Jitendex (**left**), you can clearly tell that the 軽卒 form (the bottom
search result) may be found in two different entries. In the first
entry it refers to a type of soldier, and in the second entry it is an
irregular spelling of 軽率 (the top search result).

## Other miscellaneous examples
<details>
  <summary>Entry for 猫の手も借りたい in GoldenDict-ng</summary>

![neko_no_te](https://github.com/stephenmk/Jitendex/assets/8003332/fb6dcfe7-3df5-4a5e-a38a-8ad163a59e10)
</details>


<details>
  <summary>Entry for 現在 in Yomibaba (compact glossary mode disabled; dark mode; Firefox)</summary>

![genzai_firefox_dark_uncompact](https://github.com/stephenmk/Jitendex/assets/8003332/46185a5a-5db3-4a39-be78-e409fa1ab878)
</details>

<details>
  <summary>Entry for 素姓 in Yomibaba (compact glossary mode enabled; dark mode; Firefox)</summary>

![sujou_firefox_dark_compact](https://github.com/stephenmk/Jitendex/assets/8003332/0662a974-45fe-477c-9d5b-280a59203d71)
</details>

<details>
  <summary>Entry for ばね in Yomibaba (compact glossary mode disabled; dark mode; Chromium)</summary>

![bane_chromium](https://github.com/stephenmk/Jitendex/assets/8003332/84e59ede-561a-4b13-b802-500e43cc4273)
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
Version | First Publication | Details
-- | -- | --
1.0 | 2023-10-09 | Japanese-English versions for Yomichan and MDict formats


# Donations

In addition to developing Jitendex, I am also an active contributor to the
upstream data source. I have edited thousands of JMdict entries since 2022
and am currently driving an effort to add
[hundreds of new entries](https://github.com/JMdictProject/JMdictIssues/issues/101)
for words recorded in recently published Japanese dictionaries.

If you would like to support my efforts, you can leave a tip via PayPal
on either [Ko-fi](https://ko-fi.com/jitendex) or
[Liberapay](https://liberapay.com/Jitendex/).
