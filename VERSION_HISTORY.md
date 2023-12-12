# Version History

Version | First Publication | Details
-- | -- | --
3.1 | 2023-12-12 | <ul><li>Yomitan (formerly "Yomichan")<ul><li>Add a small amount of vertical padding between extra information items (example sentences, cross references, etc.)</li><li>Fix a visual glitch in which the corners of the rectangular images were not being rounded correctly</li></ul></li></ul>
3.0 | 2023-11-16 | <ul><li>MDict Format<ul><li>Add ~9,160 pronunciation audio files distributed across ~8,230 separate entries</li><li>Include pitch accent information for ~7,720 of the pronunciations</li><li>Align furigana in headwords with their respective kanji</li></ul></li><li>All formats<ul><li>Rewrite and expand the text displayed in all part-of-speech tags, miscellaneous tags, etc. (resolves #11)</li><li>Align furigana in cross-referenced expressions with their respective kanji</li></ul></li></ul>
2.0 | 2023-11-09 | <ul><li>All formats<ul><li>Add furigana to approximately 22,000 of the 26,000 unique example sentences.</li></ul></li><li>Yomichan<ul><li>Increase font size of Japanese text in example sentences and cross references.</li></ul></li><li>MDict<ul><li>Change color of the keyword underline in example sentences from black to red.</li></ul></li></ul>
1.6 | 2023-11-04 | Add color to the square braces surrounding sense restrictions. See the entry for ばね【発条】, for example.
1.5 | 2023-10-29 | <ul><li>All Formats<ul><li>Change the symbol for valid kanji/reading forms to use the kanji 可 rather than 有.</li><li>Change the symbol for old kanji/reading forms to use the kanji 旧 rather than 古.</li></ul></li></ul>
1.4 | 2023-10-21 | <ul><li>Yomichan<ul><li>Fix a bug in which the text within monochrome SVG images was not converted into path data</li></ul></li></ul>
1.3 | 2023-10-16 | <ul><li>Yomichan<ul><li>Slightly increase the size of fonts in rectangular SVG images (again)</li><li>Add `data-sc-content` attributes to some glossary elements to allow for style customizations. Note that these attributes are volatile and may change in future versions.</li></ul></li></ul>
1.2 | 2023-10-14 | <ul><li>Yomichan<ul><li>Decrease size and increase weight of fonts in rectangular SVG images</li><li>Convert SVG text elements into paths to ensure text is always displayed the same regardless of browser or OS</li><li>Decrease font weight of text in table header cells</li></ul></li><li>All Formats<ul><li>For entries that contain only one sense, display the glossary as a block (i.e. underneath the part-of-speech tags) instead of inline (on the same line as the part-of-speech tags)</li></ul></li></ul>
1.1 | 2023-10-12 | Adjust shape, size, and font of rectangular images in the Yomichan version
1.0 | 2023-10-09 | Japanese-English versions for Yomichan and MDict formats
