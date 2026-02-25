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

using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Jitendex.Import;
using Jitendex.Tatoeba.Import.Models;
using Jitendex.Tatoeba.Import.Parsing;

namespace Jitendex.Tatoeba.Import;

internal sealed class DocumentReader(ILogger<DocumentReader> logger)
    : IDocumentReader<DateOnly, Document>
{
    public async Task<Document> ReadAsync(FileInfo file, DateOnly date)
    {
        await using FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using BrotliStream bs = new(fs, CompressionMode.Decompress);
        using StreamReader reader = new(bs);

        var document = new Document { ArchiveKey = date };

        while (await reader.ReadLineAsync() is string lineA)
        {
            if (!lineA.StartsWith("A: ", StringComparison.Ordinal))
            {
                logger.LogError("Expected `{LineA}` to start with \"A: \"", lineA);
                continue;
            }
            if (await reader.ReadLineAsync() is not string lineB)
            {
                logger.LogError("No B-line found for A-line `{LineA}`", lineA);
                continue;
            }
            if (!lineB.StartsWith("B: ", StringComparison.Ordinal))
            {
                logger.LogError("Expected `{LineB}` to start with \"B: \"", lineB);
                continue;
            }

            try
            {
                var text = new ExampleText(lineA.AsSpan(3), lineB.AsSpan(3));
                MakeIndex(text, document);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception caught while parsing text: `{Message}`", ex.Message);
            }
        }

        return document;
    }

    private void MakeIndex(in ExampleText text, Document document)
    {
        var example = GetExample(text, document);
        var translation = GetTranslation(text, document);
        var index = document.Segmentations.NextOrder(example.Id);

        var segmentation = new SegmentationElement
        {
            ExampleId = example.Id,
            Order = index,
            TranslationId = translation.Id,
        };

        document.Segmentations.Add(segmentation.GetKey(), segmentation);

        int tokenOrder = 0;
        foreach (var range in text.ElementTextRanges())
        {
            var elementText = text.GetElementText(range);
            var token = new TokenElement
            {
                ExampleId = segmentation.ExampleId,
                SegmentationOrder = segmentation.Order,
                Order = tokenOrder++,
                Headword = elementText.GetHeadword(),
                Reading = elementText.GetReading(),
                EntryId = elementText.GetEntryId(),
                SenseNumber = elementText.GetSenseNumber(),
                SentenceForm = elementText.GetSentenceForm(),
                IsPriority = elementText.GetIsPriority(),
            };
            document.Tokens.Add(token.GetKey(), token);
        }
    }

    private ExampleElement GetExample(in ExampleText text, Document document)
    {
        var id = text.GetExampleId();

        if (document.Translations.ContainsKey(id))
        {
            logger.LogWarning("Sequence ID {Id} is used for different language sentences", id);
        }

        var example = new ExampleElement(id, text.GetExampleText());

        if (!document.Examples.TryGetValue(id, out var oldSentence))
        {
            document.Examples.Add(id, example);
        }
        else if (!string.Equals(example.Text, oldSentence.Text, StringComparison.Ordinal))
        {
            logger.LogWarning("Japanese sentence #{ID} has more than one distinct text", id);
        }

        return example;
    }

    private TranslationElement GetTranslation(in ExampleText text, Document document)
    {
        var id = text.GetTranslationId();

        if (document.Examples.ContainsKey(id))
        {
            logger.LogWarning("Sequence ID {Id} is used for different language sentences", id);
        }

        var translation = new TranslationElement(id, text.GetTranslationText());

        if (!document.Translations.TryGetValue(id, out var oldSentence))
        {
            document.Translations.Add(id, translation);
        }
        else if (!string.Equals(translation.Text, oldSentence.Text, StringComparison.Ordinal))
        {
            logger.LogWarning("English sentence #{ID} has more than one distinct text", id);
        }

        return translation;
    }
}
