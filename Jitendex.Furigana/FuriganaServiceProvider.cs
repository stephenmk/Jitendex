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
using Jitendex.Furigana.Internal;
using Jitendex.Furigana.Internal.Models;
using Jitendex.Furigana.Internal.Algorithms;
using Jitendex.Furigana.Internal.Algorithms.APriori;

namespace Jitendex.Furigana;

public static class FuriganaServiceProvider
{
    public static IFuriganaService GetFuriganaService()
    {
        var resourceCache = new ResourceCache();

        ImmutableArray<IAlgorithm> smartGenerators =
        [
            new APosterioriAlgorithm(resourceCache),
            new APrioriAlgorithm
            (
                new SingleCharacterAlgorithm(),
                new RepeatedCharacterAlgorithm()
            ),
        ];

        ImmutableArray<IAlgorithm> dumbGenerators =
        [
            smartGenerators[1]
        ];

        var smartSolver = new IterationSolver(smartGenerators);
        var dumbSolver = new IterationSolver(dumbGenerators);

        var service = new Service([smartSolver, dumbSolver], resourceCache);
        return service;
    }
}
