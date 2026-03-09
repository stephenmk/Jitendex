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

using Jitendex.Import.ChiseIds.Models;
using Jitendex.Import.ChiseIds.Parsing;
using static Jitendex.Import.ChiseIds.Parsing.UnicodeConverter;

namespace Jitendex.Import.ChiseIds;

internal class DocumentReader(Logger logger)
{
    public Document Read(DirectoryInfo chiseIdsDir)
    {
        var document = new Document();

        foreach (var file in chiseIdsDir.EnumerateFiles("*.txt"))
        {
            ReadFile(file, document);
        }

        logger.WriteLogs();

        return document;
    }

    private void ReadFile(FileInfo file, Document document)
    {
        int lineNumber = 0;
        using var reader = file.OpenText();

        while (reader.ReadLine() is string line)
        {
            lineNumber++;

            if (line.StartsWith(';'))
            {
                continue;
            }

            var lineElements = new LineElements(file.Name, lineNumber, line);

            logger.LogLineErrors(lineElements);

            if (lineElements.InsufficientElementsError)
            {
                continue;
            }

            MakeCodepoint(lineElements, document);
        }
    }

    private void MakeCodepoint(in LineElements lineElements, Document document)
    {
        var unicodeCharacter = MakeUnicodeCharacter(lineElements);

        if (unicodeCharacter.HasValue)
        {
            document.UnicodeCharacters.Add(unicodeCharacter.Value);
        }

        var id = unicodeCharacter.HasValue
            ? GetLongCodepointId(unicodeCharacter.Value)
            : new string(lineElements.Codepoint);

        var parsedSequence = MakeSequence(lineElements);
        var parsedAltSequence = MakeAltSequence(lineElements);

        if (parsedSequence is null)
        {
            // There was an error making the sequence.
            return;
        }

        document.AddParsedSequence(parsedSequence);
        document.AddParsedSequence(parsedAltSequence);

        var codepoint = new CodepointElement
        {
            Id = id,
            UnicodeScalarValue = unicodeCharacter,
            SequenceText = parsedSequence.Stack.Pop().SequenceText,
            AltSequenceText = parsedAltSequence?.Stack.Pop().SequenceText,
        };

        document.Codepoints.Add(codepoint.Id, codepoint);
    }

    private int? MakeUnicodeCharacter(in LineElements lineElements)
    {
        if (!lineElements.Codepoint.StartsWith("&U", StringComparison.Ordinal))
        {
            return null;
        }

        if (ScalarValue(lineElements.Character) is not int scalarValue)
        {
            logger.LogInvalidUnicodeCodepoint(lineElements);
            return null;
        }

        var longId = GetLongCodepointId(scalarValue);

        if (!longId.SequenceEqual(lineElements.Codepoint))
        {
            var shortId = GetShortCodepointId(scalarValue);
            if (!shortId.SequenceEqual(lineElements.Codepoint))
            {
                logger.LogUnicodeCharacterInequality(lineElements);
            }
        }

        return scalarValue;
    }

    private ParserState? MakeSequence(in LineElements lineElements)
    {
        ParserState state;

        try
        {
            state = SequenceTextParser.Parse(lineElements.Sequence);
        }
        catch (InvalidOperationException)
        {
            logger.LogInsufficientIdsArgs(lineElements);
            return null;
        }

        if (state.Stack.Count != 1)
        {
            logger.LogInsufficientIdsOps(lineElements);
            return null;
        }

        return state;
    }

    private ParserState? MakeAltSequence(in LineElements lineElements)
    {
        if (lineElements.AltSequence.IsEmpty)
        {
            return null;
        }

        ParserState state;

        try
        {
             state = SequenceTextParser.Parse(lineElements.AltSequence);
        }
        catch (InvalidOperationException)
        {
            logger.LogInsufficientAltIdsArgs(lineElements);
            return null;
        }

        if (state.Stack.Count != 1)
        {
            logger.LogInsufficientAltIdsOps(lineElements);
            return null;
        }

        return state;
    }
}
