# Jitendex

Jitendex is a free, offline, and openly licensed Japanese-to-English dictionary. Updated versions with new content are published every week.

# Installation
See the instructions provided with [the latest release](https://github.com/stephenmk/Jitendex/releases/latest).

# Design and Features
The following examples provide a quick overview of the design and features of Jitendex.

## Example 1: Entry Layout
The design goal of Jitendex is to allow readers to retrieve
all of the important information from an entry at a glance.

Compare Jitendex with the reigning champ of free Japanese dictionaries, [Jisho.org](https://jisho.org/).

<table>
  <tr>
    <th>Jitendex (viewed in GoldenDict-ng)</th>
    <th>Jisho.org</th>
  </tr>
  <tr>
     <td><img alt='Screenshot of the Jitendex entry for ばね in GodlenDict-ng' src='https://github.com/stephenmk/Jitendex/assets/8003332/c4f7cc69-e2b1-4885-867d-bcd2a6ed680b'/></td>
     <td><img alt='Screenshot of the Jisho.org entry for ばね'src='https://github.com/stephenmk/Jitendex/assets/8003332/95e0d708-7861-4360-b99e-04e399efd317'/></td>
  </tr>
  <tr>
     <td colspan="2"><br>Jitendex replaces lengthy explanations with short tags. The full explanation text is accessible by hovering over these tags.</td>
  </tr>
  <tr>
     <td align='center'><img alt='Screenshot of the first sense of the Jitendex entry for ばね in GodlenDict-ng' src='https://github.com/stephenmk/Jitendex/assets/8003332/ab22fcf1-7824-4b80-9fa7-f01e632a4c9e'/></td>
     <td><img alt='Screenshot of the first sense of the Jisho.org entry for ばね' src='https://github.com/stephenmk/Jitendex/assets/8003332/fe7ef964-19c0-4826-b71a-627eb0781417'/></td>
  </tr>
  <tr>
     <td colspan="2"><br>Repetition is reduced by combining sections with identical metadata tags into a single group.</td>
  </tr>
  <tr>
     <td align='center'><img alt='Screenshot of the second and third senses of the Jitendex entry for ばね in GodlenDict-ng' src='https://github.com/stephenmk/Jitendex/assets/8003332/6b94643b-5b9c-4e28-aeb8-9f7a01ec1184'/></td>
     <td><img alt='Screenshot of the second and third senses of the Jisho.org entry for ばね' src='https://github.com/stephenmk/Jitendex/assets/8003332/2086acce-7348-40c9-8b3d-576c10809c37'/></td>
  </tr>
     <td colspan="2"><br>Variant kanji forms and readings in Jitendex are displayed in a tidy table.</td>
  </tr>
  <tr>
     <td align='center'><img alt='Screenshot of the variant forms table in the Jitendex entry for ばね in GodlenDict-ng' src='https://github.com/stephenmk/Jitendex/assets/8003332/e008a321-9d7b-4948-a099-a79195a8993c'/></td>
     <td><img alt='Screenshot of the "Other Forms" and "Notes" sections of the Jisho.org entry for ばね' src='https://github.com/stephenmk/Jitendex/assets/8003332/58a134ef-712b-4bae-9598-d32b47f038a9'/></td>
  </tr>
</table>

## Example 2: Pronunciation audio
The MDict version of Jitendex contains high-quality audio clips produced by native Japanese speakers in roughly 8,230 entries.
About 930 of those entries contain multiple clips from different speakers (male and female).
Most of these audio clips are also accompanied by [pitch accent](https://en.wikipedia.org/wiki/Japanese_pitch_accent) information.

<table>
  <tr><th colspan="2">Jitendex (viewed in GoldenDict-ng)</th></tr>
  <tr>
   <td><img alt="Screenshot of the entry for 弁護士 in Jitendex featuring example audio and pitch accent information" src="https://github.com/stephenmk/Jitendex/assets/8003332/d90b8cf2-ab16-4437-accd-6d68a4b4be8c"/></td>
   <td><img alt="Screenshot of the entry for 美容院 in Jitendex featuring two example audio clips and pitch accent information" src="https://github.com/stephenmk/Jitendex/assets/8003332/fb210253-5c23-4c92-aa7d-21432b2f1f70"/></td>
  </tr>
</table>

## Example 3: Missing Information
Compare Jitendex with the original version of [JMdict for Yomichan](https://foosoft.net/projects/yomichan/index.html#dictionaries).

Jitendex (viewed in Yomitan) | JMdict for Yomichan
:--: | :--:
![Screenshot of the Jitendex entry for ケーワイ【ＫＹ】 in Yomitan](https://github.com/stephenmk/Jitendex/assets/8003332/8facedad-2c8f-4d75-823d-5303c2eee51f) | ![Screenshot of FooSoft's 'JMdict' entry for ケーワイ【ＫＹ】 in Yomitan](https://github.com/stephenmk/Jitendex/assets/8003332/b18a8437-7de1-409e-84d1-bd44516eda35)

The original version is missing a variety of supplemental information from the EDRDG's [JMdict database](https://www.edrdg.org/wiki/index.php/JMdict-EDICT_Dictionary_Project).
This information has all been implemented in Jitendex.

* Example sentences
* Usage notes
* Etymology notes
* Cross references
* Antonyms
* Language of origin information
* Definition notes (such as "*Figuratively*", *"Literally"*, etc.)

## Example 4: Multiple Search Results
I previously
[designed](https://github.com/FooSoft/yomichan/issues/2111)
[and](https://github.com/FooSoft/yomichan/issues/2183)
[also](https://github.com/FooSoft/yomichan-import/pull/40)
[developed](https://github.com/FooSoft/yomichan-import/pull/41)
new versions of JMdict/JMnedict dictionaries for Yomichan which addressed
[the](https://github.com/FooSoft/yomichan/issues/1165)
[many](https://github.com/FooSoft/yomichan/issues/1716#issuecomment-1214436766)
[long](https://github.com/FooSoft/yomichan/issues/2057)-[standing](https://github.com/FooSoft/yomichan/issues/2058)
[issues](https://github.com/FooSoft/yomichan/issues/2210)
with the original version displayed in Example 3 above. This new version was
sometimes referred to as "JMdict Extra" in order to distinguish it from
the original version. Jitendex is a continuation of that project with support
for more app formats, more ambitious goals, and more design improvements.

Compare the search results for けいそつ from Jitendex and
my previous version of JMdict for Yomichan.

Jitendex (viewed in Yomitan) | stephenmk's "JMdict Extra"
:--: | :--:
![Screenshot of the Jitendex entries for けいそつ in Yomitan](https://github.com/stephenmk/Jitendex/assets/8003332/4a397349-7c17-49e0-b1a3-9d565f728d65) | ![Screenshot of the 'JMDict Extra' entries for けいそつ in Yomitan](https://github.com/stephenmk/Jitendex/assets/8003332/4a666880-6b5a-48b2-b73d-eced944216ce)

In Jitendex, you can clearly tell that the 軽卒 form (the bottom
search result) is found in two different entries. In the first
entry it refers to a type of soldier, and in the second entry it is
a rare spelling of 軽率 (the top search result).

## Other miscellaneous examples
<details>
  <summary>Entry for 猫の手も借りたい in GoldenDict-ng</summary>

![Screenshot of the Jitendex entry for 猫の手も借りたい in GoldenDict-ng](https://github.com/stephenmk/Jitendex/assets/8003332/0c7c44bc-0d80-4b99-8aae-c6b00e9b4330)
</details>

<details>
  <summary>Entry for ばね in Yomitan (compact glossary mode disabled; dark mode)</summary>

![Screenshot of the Jitendex entry for ばね in Yomitan](https://github.com/stephenmk/Jitendex/assets/8003332/4aa0bd35-58ff-4988-bd09-e373995593d9)
</details>

<details>
  <summary>Entry for 頂く in Yomitan (compact glossary mode enabled; light mode)</summary>

![Screenshot of the Jitendex entry for 頂く in Yomitan](https://github.com/stephenmk/Jitendex/assets/8003332/070b60ae-0d03-4083-90f1-a37f5e0bd687)
</details>

<details>
  <summary>Entry for 現在 in Yomitan (compact glossary mode disabled; dark mode)</summary>

![Screenshot of the Jitendex entry for 現在 in Yomitan](https://github.com/stephenmk/Jitendex/assets/8003332/b795605c-85ef-4f76-b3c9-c4bc116f9758)
</details>

<details>
  <summary>Entry for 素姓 in Yomitan (compact glossary mode enabled; dark mode)</summary>

![Screenshot of the Jitendex entry for 素姓 in Yomitan](https://github.com/stephenmk/Jitendex/assets/8003332/4d94b214-e3ba-4ef7-b5ee-1bbd017d5419)
</details>

# Feedback
Jitendex is targeted at a general audience of users. I may be able to
alter the design of the dictionaries based upon user feedback, but
changes to accommodate extremely specific use-cases (such as custom
frequency lists or flashcard types) will likely not be implemented.

If you see any errors or incorrect information in the dictionary
data, please feel free to report them in the
[GitHub discussions forum](https://github.com/stephenmk/Jitendex/discussions).

# Legal
© CC BY-SA 4.0 Stephen Kraus 2023

You are free to use, modify, and redistribute Jitendex files under the terms of the [Creative Commons Attribution-ShareAlike License (V4.0)](https://creativecommons.org/licenses/by-sa/4.0/).

Jitendex includes material from several copyrighted sources in compliance with the terms and conditions of those projects.
* JMdict (EDICT, etc.) dictionary data is provided by the Electronic Dictionaries Research Group. Visit [edrdg.org](https://www.edrdg.org/) for more information.
* Example sentences (Japanese and English) are provided by [Tatoeba](https://tatoeba.org/en/downloads). This data is licensed [CC BY 2.0 FR](https://creativecommons.org/licenses/by/2.0/fr/).
* Example pronunciation audio is provided by the [Kanji alive](https://github.com/kanjialive/kanji-data-media) project under a [Creative Commons Attribution 4.0 International License](http://creativecommons.org/licenses/by/4.0/).
* Positional information for the furigana displayed in headwords is provided by the [JmdictFurigana project](https://github.com/Doublevil/JmdictFurigana). This data is distributed under a Creative Commons Attribution-ShareAlike License.


# Version History
See [VERSION_HISTORY.md](VERSION_HISTORY.md).
