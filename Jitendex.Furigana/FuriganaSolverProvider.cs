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
using Jitendex.Furigana.Internal;
using Jitendex.Furigana.Internal.Models;
using Jitendex.Furigana.Internal.SolutionGenerators;

namespace Jitendex.Furigana;

public static class FuriganaSolverProvider
{
    public static IFuriganaSolver GetFuriganaSolver()
    {
        var resourceCache = new ResourceCache();
        var solutionPartsGenerators = new List<ISolutionPartsGenerator>()
        {
            new CachedSolutionPartsGenerator(resourceCache),
            new DefaultSolutionPartsGenerator
            (
                new DefaultSingleCharacterParts(),
                new DefaultRepeatedCharacterParts()
            )
        };
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
(
    string Text,
    ImmutableArray<string> Readings
);

public sealed record JapaneseCharacter
(
    Rune Rune,
    ImmutableArray<CharacterReading> VocabReadings,
    ImmutableArray<CharacterReading> NameReadings
);

public sealed record CharacterReading
(
    string Text,
    bool IsPrefix,
    bool IsSuffix
);

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
