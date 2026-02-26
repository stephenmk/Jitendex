/*
Copyright (c) 2025 Stephen Kraus
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

using Jitendex.Chise.Entities;
using Jitendex.Chise.Import.Parsing;
using static Jitendex.Chise.Import.Parsing.UnicodeConverter;

namespace Jitendex.Chise.Import;

internal class ChiseIdsReader
{
    private readonly Logger _logger;

    public ChiseIdsReader()
    {
        _logger = new Logger();
    }

    public IdsCollector Read(DirectoryInfo chiseIdsDir)
    {
        var IdsCollector = new IdsCollector(_logger);
        foreach (var file in chiseIdsDir.EnumerateFiles("*.txt"))
        {
            foreach (var codepoint in ReadFile(file))
            {
                IdsCollector.AddCodepoint(codepoint);
            }
        }
        _logger.WriteLogs();
        return IdsCollector;
    }

    private IEnumerable<Codepoint> ReadFile(FileInfo file)
    {
        int lineNumber = 0;
        using StreamReader sr = file.OpenText();

        while (sr.ReadLine() is string line)
        {
            lineNumber++;

            if (line.StartsWith(';'))
            {
                continue;
            }

            var lineElements = new LineElements(file.Name, lineNumber, line);

            _logger.LogLineErrors(lineElements);

            if (lineElements.InsufficientElementsError)
            {
                continue;
            }

            if (MakeCodepoint(lineElements) is Codepoint codepoint)
            {
                yield return codepoint;
            }
        }
    }

    private Codepoint? MakeCodepoint(in LineElements lineElements)
    {
        var unicodeCharacter = MakeUnicodeCharacter(lineElements);

        var id = unicodeCharacter is null
                 ? new string(lineElements.Codepoint)
                 : unicodeCharacter.CodepointId;

        var sequence = MakeSequence(lineElements);
        var altSequence = MakeAltSequence(lineElements);

        if (sequence is null)
        {
            // There was an error making the sequence.
            return null;
        }

        return new Codepoint
        {
            Id = id,
            UnicodeScalarValue = unicodeCharacter?.ScalarValue,
            SequenceText = sequence.Text,
            AltSequenceText = altSequence?.Text,
            UnicodeCharacter = unicodeCharacter,
            Sequence = sequence,
            AltSequence = altSequence,
        };
    }

    private UnicodeCharacter? MakeUnicodeCharacter(in LineElements lineElements)
    {
        if (!lineElements.Codepoint.StartsWith("&U"))
        {
            return null;
        }

        if (ScalarValue(lineElements.Character) is not int scalarValue)
        {
            _logger.LogInvalidUnicodeCodepoint(lineElements);
            return null;
        }

        var longId = GetLongCodepointId(scalarValue);

        if (!longId.SequenceEqual(lineElements.Codepoint))
        {
            var shortId = GetShortCodepointId(scalarValue);
            if (!shortId.SequenceEqual(lineElements.Codepoint))
            {
                _logger.LogUnicodeCharacterInequality(lineElements);
            }
        }

        return new UnicodeCharacter
        {
            ScalarValue = scalarValue,
            CodepointId = new string(longId),
        };
    }

    private Sequence? MakeSequence(in LineElements lineElements)
    {
        Stack<Codepoint> sequenceArguments;

        try
        {
            sequenceArguments = SequenceTextParser.Parse(lineElements.Sequence);
        }
        catch (InvalidOperationException)
        {
            _logger.LogInsufficientIdsArgs(lineElements);
            return null;
        }

        if (sequenceArguments.Count != 1)
        {
            _logger.LogInsufficientIdsOps(lineElements);
            return null;
        }

        return sequenceArguments.Pop().Sequence;
    }

    private Sequence? MakeAltSequence(in LineElements lineElements)
    {
        if (lineElements.AltSequence.IsEmpty)
        {
            return null;
        }

        Stack<Codepoint> altSequenceArguments;

        try
        {
            altSequenceArguments = SequenceTextParser.Parse(lineElements.AltSequence);
        }
        catch (InvalidOperationException)
        {
            _logger.LogInsufficientAltIdsArgs(lineElements);
            return null;
        }

        if (altSequenceArguments.Count != 1)
        {
            _logger.LogInsufficientAltIdsOps(lineElements);
            return null;
        }

        return altSequenceArguments.Pop().Sequence;
    }
}
