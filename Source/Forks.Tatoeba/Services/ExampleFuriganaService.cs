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

using Microsoft.Extensions.Logging;
using Jitendex.Data.Home;
using Jitendex.Data.Tatoeba;
using Jitendex.Forks.Tatoeba.Models;
using Jitendex.Forks.Tatoeba.Tables;
using Microsoft.EntityFrameworkCore;

namespace Jitendex.Forks.Tatoeba.Services;

internal partial class ExampleFuriganaService
(
    ILogger<ExampleFuriganaService> logger,
    TatoebaForkContext tatoebaContext,
    HomeContext homeContext,
    ExampleFuriganaTable table
)
{
    public void Write()
    {
        CheckIDs();
        InsertRows();
        CheckSentences();
    }

    private void CheckIDs()
    {
        var validExampleIds = tatoebaContext.Examples
            .Select(static e => e.Id)
            .ToHashSet();

        var exampleIds = homeContext.Examples
            .Select(static e => e.Id);

        foreach (var id in exampleIds)
        {
            if (!validExampleIds.Contains(id))
            {
                LogMissingExample(id);
                homeContext.Examples
                    .Where(e => e.Id == id)
                    .ExecuteDelete();
            }
        }
    }

    private void InsertRows()
    {
        var rows = homeContext.ExampleFurigana
            .Select(static x => new ExampleFuriganaRow
            (
                x.ExampleId,
                x.Order,
                x.BaseText,
                x.RubyText
            ));

        table.InsertItems(tatoebaContext, rows);
    }

    private void CheckSentences()
    {
        var texts = tatoebaContext.Examples
            .Where(e => e.Furigana.Count > 0)
            .Select(static e => new
            {
                e.Id,
                e.Text,
                BaseTexts = e.Furigana
                    .Select(static f => f.BaseText)
            });

        foreach (var text in texts)
        {
            var furiganaSentence = string.Join(string.Empty, text.BaseTexts);
            if (!string.Equals(text.Text, furiganaSentence, StringComparison.Ordinal))
            {
                LogDifferentText(text.Id, text.Text, furiganaSentence);
            }
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Example ID {ExampleId} does not exist in Examples table")]
    partial void LogMissingExample(int exampleId);

    [LoggerMessage(LogLevel.Warning,
    "Sentence text in example ID {ExampleId} differs from the furigana sentence text\n{OldText}\n{NewText}")]
    partial void LogDifferentText(int exampleId, string newText, string oldText);
}
