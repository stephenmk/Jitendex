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
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Jitendex.AppDirectory;
using Jitendex.Furigana;
using Jitendex.Kanjidic2;

namespace Jitendex.JMdictAnalysis.Analysis.Services;

internal sealed class FuriganaSolverService(ILogger<FuriganaSolverService> logger, Kanjidic2Context kanjiContext)
{
    public async Task<IFuriganaService> LoadAsync(DirectoryInfo? dataDir)
    {
        var service = FuriganaServiceProvider.GetFuriganaService();

        // Characters
        AddKanjiCharactersToSolver(service);
        await AddAlphanumericCharactersToSolver(service, dataDir);
        await AddPunctuationCharactersToSolver(service, dataDir);
        await AddKanaCharactersToSolver(service, dataDir);

        // Compounds
        await AddKanjiCompoundsToSolver(service, dataDir);
        await AddAlphanumericCompoundsToSolver(service, dataDir);

        return service;
    }

    private void AddKanjiCharactersToSolver(IFuriganaService service)
    {
        var characters = kanjiContext.DerivedReadings
            .GroupBy(static r => r.UnicodeScalarValue)
            .Select(static g => new
            {
                Rune = new Rune(g.Key),
                Readings = g.Select(static r => new { r.Text, r.IsPrefix, r.IsSuffix }).ToImmutableArray()
            });
        foreach (var character in characters)
        {
            foreach (var reading in character.Readings)
            {
                service.AddCharacterReading(character.Rune, reading.Text, reading.IsPrefix, reading.IsSuffix);
            }
        }
    }

    private async Task AddAlphanumericCharactersToSolver(IFuriganaService solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "characters", "alphanumeric.json");
        await AddDefaultCharactersToSolver(solver, filePath);
    }

    private async Task AddPunctuationCharactersToSolver(IFuriganaService solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "characters", "symbols_and_punctuation.json");
        await AddDefaultCharactersToSolver(solver, filePath);
    }

    private async Task AddKanaCharactersToSolver(IFuriganaService solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "characters", "kana.json");
        await AddDefaultCharactersToSolver(solver, filePath);
    }

    private async Task AddDefaultCharactersToSolver(IFuriganaService service, string filePath)
    {
        await using var stream = File.OpenRead(filePath);
        var dictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, string[]>>(stream);
        if (dictionary is null || dictionary.Count == 0)
        {
            logger.LogError("Failed to load dictionary from path {Path}", filePath);
            return;
        }
        foreach (var (character, readings) in dictionary)
        {
            var rune = character.EnumerateRunes().First();
            foreach (var reading in readings)
            {
                service.AddCharacterReading(rune, reading);
            }
        }
    }

    private async Task AddAlphanumericCompoundsToSolver(IFuriganaService solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "compounds", "alphanumeric.json");
        await AddCompoundsToSolver(solver, filePath);
    }

    private async Task AddKanjiCompoundsToSolver(IFuriganaService solver, DirectoryInfo? dataDir)
    {
        var filePath = GetJsonFilePath(dataDir, "compounds", "kanji.json");
        await AddCompoundsToSolver(solver, filePath);
    }

    private async Task AddCompoundsToSolver(IFuriganaService service, string filePath)
    {
        await using var stream = File.OpenRead(filePath);
        var dictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, string[]>>(stream);
        if (dictionary is null || dictionary.Count == 0)
        {
            logger.LogError("Failed to load dictionary from path {Path}", filePath);
            return;
        }
        foreach (var (compound, readings) in dictionary)
        {
            foreach (var reading in readings)
            {
                service.AddCompoundReading(compound, reading);
            }
        }
    }

    private string GetJsonFilePath(DirectoryInfo? dataDir, string subdirectory, string filename)
    {
        dataDir ??= DataHome.Get(DataSubdirectory.JitendexDataDirectory);
        return Path.Join(dataDir.FullName, "furigana", subdirectory, filename);
    }
}
