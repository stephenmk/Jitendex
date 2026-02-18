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

using System.Collections.Immutable;
using System.Text;
using Jitendex.JapaneseTextUtils;
using Jitendex.Furigana.Internal;
using Jitendex.Furigana.Internal.Models;
using Jitendex.Furigana.Internal.SolutionGenerators;

namespace Jitendex.Furigana;

public static class FuriganaSolverProvider
{
    public static IFuriganaSolver GetFuriganaSolver()
    {
        var resourceCache = new ResourceCache();

        ImmutableArray<ISolutionPartsGenerator> solutionPartsGenerators =
        [
            new CachedSolutionPartsGenerator(resourceCache),
            new DefaultSolutionPartsGenerator
            (
                new DefaultSingleCharacterParts(),
                new DefaultRepeatedCharacterParts()
            )
        ];

        var iterationSolver = new IterationSolver(solutionPartsGenerators);
        var solver = new Solver(iterationSolver, resourceCache);
        return solver;
    }
}

public interface IFuriganaSolver
{
    public Solution? SolveVocab(string kanjiForm, string reading);
    public Solution? SolveName(string kanjiForm, string reading);
    public void AddCompounds(IEnumerable<JapaneseCompound> compounds);
    public void AddCharacters(IEnumerable<JapaneseCharacter> characters);
}

public sealed record JapaneseCompound
{
    public string Text { get; }
    public ImmutableArray<string> Readings { get; }
    public JapaneseCompound(string text, IEnumerable<string> readings)
    {
        Text = text;
        var normalizedReadings = new List<string>();
        foreach (var reading in readings)
        {
            normalizedReadings.Add(reading.KatakanaToHiragana());
        }
        Readings = [.. normalizedReadings];
    }
};

public sealed record JapaneseCharacter
{
    public Rune Rune { get; }
    public ImmutableArray<CharacterReading> VocabReadings { get; }
    public ImmutableArray<CharacterReading> NameReadings { get; }
    public JapaneseCharacter(Rune rune, IEnumerable<CharacterReading> vocabReadings, IEnumerable<CharacterReading> nameReadings)
    {
        Rune = rune;
        VocabReadings = [.. vocabReadings];
        NameReadings = [.. nameReadings];
    }
};

public sealed record CharacterReading
{
    public string Text { get; }
    public bool IsPrefix { get; }
    public bool IsSuffix { get; }
    public CharacterReading(string text, bool isPrefix, bool isSuffix)
    {
        Text = text.KatakanaToHiragana();
        IsPrefix = isPrefix;
        IsSuffix = isSuffix;
    }
}

public sealed record SolutionPart
(
    string BaseText,
    string? Furigana
);

public sealed class Solution
{
    public required string KanjiFormText { get; init; }
    public required string ReadingText { get; init; }
    public required ImmutableArray<SolutionPart> Parts { get; init; }

    public override bool Equals(object? obj)
        => obj is Solution sln
        && string.Equals(KanjiFormText, sln.KanjiFormText, StringComparison.Ordinal)
        && string.Equals(ReadingText, sln.ReadingText, StringComparison.Ordinal)
        && Parts.SequenceEqual(sln.Parts);

    public override int GetHashCode() => Parts.Aggregate
    (
        seed: HashCode.Combine(KanjiFormText, ReadingText),
        func: static (hashcode, part) => HashCode.Combine(hashcode, part.GetHashCode())
    );
}
