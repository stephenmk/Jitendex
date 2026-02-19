/*
Copyright (c) 2026 Stephen Kraus
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
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Jitendex.AppDirectory;
using Jitendex.Furigana;
using Jitendex.Kanjidic2;

namespace Jitendex.JMdictAnalysis.Import.Services;

internal sealed class FuriganaSolverService(ILogger<FuriganaSolverService> logger, Kanjidic2Context kanjiContext)
{
    public async Task<IFuriganaSolver> LoadAsync(DirectoryInfo? dataDir)
    {
        var solver = FuriganaSolverProvider.GetFuriganaSolver();

        // Characters
        AddKanjiCharactersToSolver(solver);
        await AddAlphanumericCharactersToSolver(solver, dataDir);
        await AddPunctuationCharactersToSolver(solver, dataDir);
        await AddKanaCharactersToSolver(solver, dataDir);

        // Compounds
        await AddKanjiCompoundsToSolver(solver, dataDir);
        await AddAlphanumericCompoundsToSolver(solver, dataDir);

        return solver;
    }

    private void AddKanjiCharactersToSolver(IFuriganaSolver solver)
    {
        var characters = kanjiContext.DerivedReadings
            .GroupBy(static r => r.UnicodeScalarValue)
            .Select(static g => new JapaneseCharacter
            (
                rune: new(g.Key),
                vocabReadings: g.Select(static r => new CharacterReading(r.Text, r.IsPrefix, r.IsSuffix)).ToImmutableArray(),
                nameReadings: Enumerable.Empty<CharacterReading>()
            ));
        solver.AddCharacters(characters);
    }

    private async Task AddAlphanumericCharactersToSolver(IFuriganaSolver solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "characters", "alphanumeric.json");
        await AddDefaultCharactersToSolver(solver, filePath);
    }

    private async Task AddPunctuationCharactersToSolver(IFuriganaSolver solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "characters", "symbols_and_punctuation.json");
        await AddDefaultCharactersToSolver(solver, filePath);
    }

    private async Task AddKanaCharactersToSolver(IFuriganaSolver solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "characters", "kana.json");
        await AddDefaultCharactersToSolver(solver, filePath);
    }

    private async Task AddDefaultCharactersToSolver(IFuriganaSolver solver, string filePath)
    {
        await using var stream = File.OpenRead(filePath);
        var dictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, ImmutableArray<string>>>(stream);
        if (dictionary is null || dictionary.Count == 0)
        {
            logger.LogError("Failed to load dictionary from path {Path}", filePath);
            return;
        }
        var characters = dictionary
            .Select(static x => new JapaneseCharacter
            (
                rune: x.Key.EnumerateRunes().First(),
                vocabReadings: x.Value.Select(static x => new CharacterReading(x, false, false)),
                nameReadings: []
            ));
        solver.AddCharacters(characters);
    }

    private async Task AddAlphanumericCompoundsToSolver(IFuriganaSolver solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "compounds", "alphanumeric.json");
        await AddCompoundsToSolver(solver, filePath);
    }

    private async Task AddKanjiCompoundsToSolver(IFuriganaSolver solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "compounds", "kanji.json");
        await AddCompoundsToSolver(solver, filePath);
    }

    private async Task AddCompoundsToSolver(IFuriganaSolver solver, string filePath)
    {
        await using var stream = File.OpenRead(filePath);
        var dictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, ImmutableArray<string>>>(stream);

        if (dictionary is null || dictionary.Count == 0)
        {
            logger.LogError("Failed to load dictionary from path {Path}", filePath);
            return;
        }

        var compounds = dictionary.Select(static x => new JapaneseCompound(x.Key, x.Value));
        solver.AddCompounds(compounds);
    }

    private string GetJsonFilePath(DirectoryInfo? dataDir, string subdirectory, string filename)
    {
        dataDir ??= DataHome.Get(DataSubdirectory.JitendexDataDirectory);
        return Path.Join(dataDir.FullName, "furigana", subdirectory, filename);
    }
}
