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

public static class FuriganaServiceProvider
{
    public static IFuriganaService GetFuriganaService()
    {
        var resourceCache = new ResourceCache();

        ImmutableArray<ISolutionPartsGenerator> smartGenerators =
        [
            new CachedSolutionPartsGenerator(resourceCache),
            new DefaultSolutionPartsGenerator
            (
                new DefaultSingleCharacterParts(),
                new DefaultRepeatedCharacterParts()
            ),
        ];

        ImmutableArray<ISolutionPartsGenerator> dumbGenerators =
        [
            smartGenerators[1]
        ];

        var smartSolver = new IterationSolver(smartGenerators);
        var dumbSolver = new IterationSolver(dumbGenerators);

        var service = new Service([smartSolver, dumbSolver], resourceCache);
        return service;
    }
}

public interface IFuriganaService
{
    public Solution? Solve(string text, string reading);
    public Solution? SolveName(string text, string reading);
    public Solution? SolveChineseLoanword(string text, string reading);
    public Solution? SolveKoreanLoanword(string text, string reading);

    public void AddCharacterReading(Rune character, string reading, bool isPrefix = false, bool isSuffix = false);
    public void AddNameReading(Rune kanji, string reading);
    public void AddHanziReading(Rune hanzi, string reading);
    public void AddHanjaReading(Rune hanja, string reading);
    public void AddCompoundReading(string compound, string reading);
}

public sealed class Solution
{
    public required string Text { get; init; }
    public required string Reading { get; init; }
    public required ImmutableArray<SolutionPart> Parts { get; init; }

    public override bool Equals(object? obj)
        => obj is Solution sln
        && string.Equals(Text, sln.Text, StringComparison.Ordinal)
        && string.Equals(Reading, sln.Reading, StringComparison.Ordinal)
        && Parts.SequenceEqual(sln.Parts);

    public override int GetHashCode() => Parts.Aggregate
    (
        seed: HashCode.Combine(Text, Reading),
        func: static (hashcode, part) => HashCode.Combine(hashcode, part)
    );
}

public sealed record SolutionPart
(
    string BaseText,
    string? RubyText
);
