# Jitendex.Furigana

This project takes two strings of Japanese text, a "reading" string and a "kanji form" string,
and distributes the reading characters over their respective kanji.

<table>
  <tr>
    <th>Example</th>
    <th colspan=2>Inputs</th>
    <th>Output</th>
  </tr>
  <tr>
    <td rowspan=2 align="center">#1</td>
    <td>Reading</td>
    <td>にほんご</td>
    <td rowspan=2 align="center"><ruby>日<rt>に</rt>　</ruby><ruby>本<rt>ほん</rt></ruby>　<ruby>語<rt>ご</rt></ruby></td>
  </tr>
  <tr>
    <td>Kanji Form</td>
    <td>日本語</td>
  </tr>
  <tr>
    <td rowspan=2 align="center">#2</td>
    <td>Reading</td>
    <td>ふかこうりょく</td>
    <td rowspan=2 align="center"><ruby>不<rt>ふ</rt></ruby>　<ruby>可<rt>か</rt></ruby>　<ruby>抗<rt>こう</rt></ruby>　<ruby>力<rt>りょく</rt></ruby></td>
  </tr>
  <tr>
    <td>Kanji Form</td>
    <td>不可抗力</td>
  </tr>
  <tr>
    <td rowspan=2 align="center">#3</td>
    <td>Reading</td>
    <td>きがききすぎてまがぬける</td>
    <td rowspan=2 align="center"><ruby>気<rt>き</rt></ruby>が<ruby>利<rt>き</rt></ruby>き<ruby>過<rt>す</rt></ruby>ぎて<ruby>間<rt>ま</rt></ruby>が<ruby>抜<rt>ぬ</rt></ruby>ける</td>
  </tr>
  <tr>
    <td>Kanji Form</td>
    <td>気が利き過ぎて間が抜ける</td>
  </tr>
</table>

The small reading characters displayed in the "Output" column are called [furigana](https://en.wikipedia.org/wiki/Furigana), hence the name of the project.

# Comparison with JmdictFurigana

This program began as a fork of the [JmdictFurigana](https://github.com/Doublevil/JmdictFurigana) project by [Doublevil](https://github.com/Doublevil).
That project served as an invaluable source of ideas, but all of the code borrowed from it has since been replaced.
**Jitendex.Furigana** now uses an entirely different algorithm and data structures.

<table>
  <tr>
    <th>Comparison</th>
    <th>JmdictFurigana</th>
    <th>Jitendex.Furigana</th>
  </tr>
  <tr>
    <th>Algorithms</th>
    <td>
      Uses
      <a href="https://github.com/Doublevil/JmdictFurigana/tree/71acd897c6606e9dfa3c4947dbf8fcfafb567c77/JmdictFurigana/Business/Solvers">
        several small and simple algorithms</a>
      to solve furigana problems. It applies each method one-by-one to a given problem and returns a solution if any of them succeed.
    </td>
    <td>
      Uses a single, iterative parsing algorithm which incorporates all of the ideas behind JmdictFurigana's methods.
      The result is more powerful than the sum of its parts: it can solve problems that none of simpler algorithms could solve individually.
    </td>
  </tr>
  <tr>
    <th>Data Structures</th>
    <td>
      Algorithms all work upon furigana segments modelled as index-based objects.
      <br/><br/>
      Example:
      <br/>
      <code>阿呆陀羅|0:あ;1:ほん;2:だ;3:ら</code>
    </td>
    <td>
      Furigana segments are modelled as objects containing a pair of texts: the base text and the furigana text.
      This is conceptually much simpler and easier to work with.
      <br/><br/>
      Example:
      <br/>
      <code>[阿|あ][呆|ほん][陀|だ][羅|ら]</code>.
      </div>
    </td>
  </tr>
  <tr>
    <th>Analysis Features</th>
    <td>
      Outputs only the furigana segmentation data for a given problem.
    </td>
    <td>
      Provided that しょう is a reading for the character 象 in the word <ruby>具<rt>ぐ</rt></ruby><ruby>象<rt>しょう</rt></ruby>,
      we might be interested to know which other vocabulary words contain this reading of this character.
      <b>Jitendex.Furigana</b> keeps track of this information.
    </td>
  </tr>
  <tr>
    <th>Unicode Support</th>
    <td>
      Some Japanese characters are represented in Unicode using two
      <a href="https://en.wikipedia.org/wiki/UTF-16">UTF-16</a> code units.
      For example, the character 𠮟 (U+20B9F) is a UTF-16 "surrogate pair" composed of two code units:
      <code>D842</code> and <code>DF9F</code>.
      <br/><br/>
      The C# standard library was not well-equiped for handling these characters at the time <b>JmdictFurigana</b>
      was written. The program
      <a href="https://github.com/Doublevil/JmdictFurigana/blob/71acd897c6606e9dfa3c4947dbf8fcfafb567c77/JmdictFurigana.Tests/FuriganaTest.cs#L43-L52">
        cannot solve</a>
      furigana problems containing these characters.
    </td>
    <td>
      Microsoft has since
      <a href="https://learn.microsoft.com/en-us/dotnet/api/system.text.rune">
        made it easier to work with</a>
      surrogate pairs, and <b>Jitendex.Furigana</b>
      <a href="https://github.com/stephenmk/Jitendex/blob/dotnet/Jitendex.Furigana.Test/ServiceTests/SurrogatePairs.cs">
        can handle</a>
      these problems.
    </td>
  </tr>
  <tr>
    <th>Accuracy</th>
    <td>
      Produces solutions for some problems which are incorrect in a strict sense.
      For example, "とうきょう" with "ＴＯＫＹＯ" is parsed as
      <br/>
      <ruby>Ｔ<rt>と</rt></ruby>　<ruby>Ｏ<rt>う</rt></ruby>　<ruby>Ｋ<rt>き</rt></ruby>　<ruby>Ｙ<rt>ょ</rt></ruby>　<ruby>Ｏ<rt>う</rt></ruby>
      <br/>
      because both strings are of equal length. The equal-length method can safely be used for over 99% of words written with kanji,
      but the method fails on most words written with an alphabet.
    </td>
    <td>
      Contains an
      <a href="https://github.com/stephenmk/Jitendex/tree/dotnet/Jitendex.Furigana.Test/ServiceTests">
        extensive set of tests</a>
      to ensure the algorithm is working as intended.
    </td>
  </tr>
</table>
