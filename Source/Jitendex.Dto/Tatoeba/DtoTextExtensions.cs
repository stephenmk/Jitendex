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

namespace Jitendex.Dto.Tatoeba;

public static class DtoTextExtensions
{
    public static string ToText(this SequenceDto sequence)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Tatoeba Sentence #{sequence.Id}");
        if (sequence.Example is not null)
        {
            sb.AppendLine(sequence.Example.ToText());
        }
        else
        {
            sb.AppendLine("< deleted >");
        }
        return sb.ToString();
    }

    private static string ToText(this ExampleDto example)
    {
        var sb = new StringBuilder();
        sb.AppendLine(example.Text);
        foreach (var segmentation in example.Segmentations)
        {
            sb.AppendLine();
            sb.AppendLine(segmentation.ToText());
        }
        return sb.ToString();
    }

    private static string ToText(this SegmentationDto segmentation) =>
        $"""
        --Translation--
        {segmentation.Translation.ToText()}
        --Tokens--
        {string.Join(' ', segmentation.Tokens.Select(static t => t.ToText()))}
        """;

    private static string ToText(this TranslationDto translation)
        => $"#{translation.Id}: {translation.Text}";

    private static string ToText(this TokenDto token)
    {
        var sb = new StringBuilder(token.Headword);
        if (token.Reading is not null)
        {
            sb.Append($"({token.Reading})");
        }
        if (token.EntryId is not null)
        {
            sb.Append($"(#{token.EntryId})");
        }
        if (token.SenseNumber is not null)
        {
            sb.Append($"[{token.SenseNumber}]");
        }
        if (token.SentenceForm is not null)
        {
            sb.Append($"{{{token.SentenceForm}}}");
        }
        if (token.IsPriority)
        {
            sb.Append('~');
        }
        return sb.ToString();
    }
}
