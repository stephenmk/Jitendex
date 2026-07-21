// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, FuriganaServiceProvider.cs, is part of Jitendex.
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

using Jitendex.Furigana.Internal;
using Jitendex.Furigana.Internal.Algorithms;
using Jitendex.Furigana.Internal.Algorithms.Enlightenment;
using Jitendex.Furigana.Internal.Algorithms.Ignorance;
using Jitendex.Furigana.Internal.Models;

namespace Jitendex.Furigana;

public static class FuriganaServiceProvider
{
    public static IFuriganaService GetFuriganaService()
    {
        var knowledge = new Knowledge();

        var informedAlgo = new InformedAlgorithm
        (
            new KnownCharacterAlgorithm(knowledge),
            new KnownCompoundAlgorithm(knowledge)
        );
        var ignorantAlgo = new IgnorantAlgorithm
        (
            new SingleCharacterAlgorithm(),
            new RepeatedKanjiAlgorithm(),
            new IdentityAlgorithm(),
            new InitialismAlgorithm()
        );
        var lazyIgnorantAlgo = new IgnorantAlgorithm
        (
            new SingleCharacterAlgorithm(),
            new RepeatedKanjiAlgorithm(),
            new IdentityAlgorithm(),
            new InitialismAlgorithm(),
            new ConsecutiveKanjiAlgorithm()
        );

        var informedSolver = new IterationSolver([informedAlgo, ignorantAlgo]);
        var ignorantSolver = new IterationSolver([ignorantAlgo]);
        var lazyIgnorantSolver = new IterationSolver([lazyIgnorantAlgo]);

        return new Service
        (
            knowledge,
            solvers: [
                informedSolver,
                ignorantSolver,
                lazyIgnorantSolver
            ]
        );
    }
}
