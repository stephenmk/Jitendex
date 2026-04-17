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
using Jitendex.Import.Tatoeba.Parsing;
using Jitendex.Import.Tatoeba.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.Tatoeba;

internal sealed class DocumentReader(ILogger<DocumentReader> logger)
    : IDocumentReader<DateOnly, Document>
{
    public async Task<Document> ReadAsync(FileInfo file, DateOnly date)
    {
        await using var fileStream = file.OpenRead();
        await using var brotliStream = new BrotliStream(fileStream, CompressionMode.Decompress);
        using var reader = new StreamReader(brotliStream);

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

        var segmentation = new SegmentationRow
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
            var token = new TokenRow
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

    private ExampleRow GetExample(in ExampleText text, Document document)
    {
        var id = text.GetExampleId();
        var example = new ExampleRow(id, text.GetExampleText());
        CheckExample(example, document);
        return example;
    }

    private ExampleRow GetTranslation(in ExampleText text, Document document)
    {
        var id = text.GetTranslationId();
        var translation = new ExampleRow(id, text.GetTranslationText());
        CheckExample(translation, document);
        return translation;
    }

    private void CheckExample(ExampleRow example, Document document)
    {
        if (!document.Examples.TryGetValue(example.Id, out var oldSentence))
        {
            document.Examples.Add(example.Id, example);
        }
        else if (!string.Equals(example.Text, oldSentence.Text, StringComparison.Ordinal))
        {
            logger.LogWarning("Sentence #{ID} has more than one distinct text", example.Id);
        }
    }
}
