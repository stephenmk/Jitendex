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

namespace Jitendex.Dto.JMdict;

public static class DtoTextExtensions
{
    public static string ToText(this SequenceDto x)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Entry #{x.Id}");
        if (x.Entry is not null)
        {
            sb.AppendLine(x.Entry.ToText());
        }
        else
        {
            sb.AppendLine("< deleted >");
        }
        return sb.ToString();
    }

    private static string ToText(this EntryDto x)
    {
        var sb = new StringBuilder();
        if (x.KanjiForms.Length > 0)
        {
            sb.AppendLine("Kanji Forms");
            for (int i = 0; i < x.KanjiForms.Length; i++)
            {
                sb.AppendLine($"\t{i + 1}: {x.KanjiForms[i].ToText()}");
            }
        }
        if (x.Readings.Length > 0)
        {
            sb.AppendLine("Readings");
            for (int i = 0; i < x.Readings.Length; i++)
            {
                sb.AppendLine($"\t{i + 1}: {x.Readings[i].ToText()}");
            }
        }
        if (x.Senses.Length > 0)
        {
            sb.AppendLine("Senses");
            for (int i = 0; i < x.Senses.Length; i++)
            {
                sb.AppendLine($"\t{i + 1}. {x.Senses[i].ToText()}");
            }
        }
        return sb.ToString();
    }

    private static string ToText(this KanjiFormDto x)
    {
        var sb = new StringBuilder(x.Text);
        if (x.Infos.Length > 0)
        {
            sb.Append($" [{string.Join(", ", x.Infos)}]");
        }
        if (x.Priorities.Length > 0)
        {
            sb.Append($" [{string.Join(", ", x.Priorities)}]");
        }
        return sb.ToString();
    }

    private static string ToText(this ReadingDto x)
    {
        var sb = new StringBuilder(x.Text);
        if (x.Infos.Length > 0)
        {
            sb.Append($"[{string.Join(",", x.Infos)}]");
        }
        if (x.Priorities.Length > 0)
        {
            sb.Append($"[{string.Join(",", x.Priorities)}]");
        }
        if (x.Restrictions.Length > 0)
        {
            sb.Append($"[{string.Join("；", x.Restrictions)}]");
        }
        if (x.NoKanji)
        {
            sb.Append("[nokanji]");
        }
        return sb.ToString();
    }

    private static string ToText(this SenseDto x)
    {
        var sb = new StringBuilder();
        if (x.PartsOfSpeech.Length > 0)
        {
            sb.Append($"[{string.Join(", ", x.PartsOfSpeech)}]");
        }
        if (x.Miscs.Length > 0)
        {
            sb.Append($"[{string.Join(", ", x.Miscs)}]");
        }
        if (x.Fields.Length > 0)
        {
            sb.Append($"{{{string.Join(", ", x.Fields)}}}");
        }
        if (x.KanjiFormRestrictions.Length > 0)
        {
            sb.Append($"[{string.Join("；", x.KanjiFormRestrictions)}]");
        }
        if (x.ReadingRestrictions.Length > 0)
        {
            sb.Append($"[{string.Join("；", x.ReadingRestrictions)}]");
        }
        if (x.Dialects.Length > 0)
        {
            sb.AppendLine();
            sb.Append($"\t\tDialect: {string.Join(", ", x.Dialects)}");
        }
        if (x.LanguageSources.Length > 0)
        {
            sb.AppendLine();
            sb.Append("\t\tSource language:");
            foreach (var langSource in x.LanguageSources)
            {
                sb.AppendLine();
                sb.Append($"\t\t{langSource.ToText()}");
            }
        }
        if (x.Notes.Length > 0)
        {
            foreach (var note in x.Notes)
            {
                sb.AppendLine();
                sb.Append($"\t\t《{note}》");
            }
        }
        if (x.Glosses.Length > 0)
        {
            foreach (var gloss in x.Glosses)
            {
                sb.AppendLine();
                sb.Append($"\t\t▶ {gloss.ToText()}");
            }
        }
        if (x.CrossReferences.Length > 0)
        {
            sb.AppendLine();
            sb.Append("\t\tCross references:");
            foreach (var xref in x.CrossReferences)
            {
                sb.AppendLine();
                sb.Append($"\t\t{xref.ToText()}");
            }
        }
        return sb.ToString();
    }

    private static string ToText(this GlossDto x)
        => x.TypeName is null
            ? x.Text
            : $"[{x.TypeName}] {x.Text}";

    private static string ToText(this CrossReferenceDto x)
        => $"⇒{x.TypeName}: {x.Text}";

    private static string ToText(this LanguageSourceDto x)
    {
        var sb = new StringBuilder(x.LanguageCode);
        if (!string.Equals(x.TypeName, "full", StringComparison.Ordinal))
        {
            sb.Append($" ({x.TypeName})");
        }
        if (x.IsWasei)
        {
            sb.Append(" [wasei]");
        }
        if (x.Text is not null)
        {
            sb.Append($": {x.Text}");
        }
        return sb.ToString();
    }
}
