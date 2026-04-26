// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, DtoTextExtensions.cs, is part of Jitendex.
//
// Jitendex is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License or (at your option) any later version.
//
// Jitendex is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with Jitendex.
// If not, see <https://www.gnu.org/licenses/>.

namespace Jitendex.Dto.JMdict;

public static class DtoTextExtensions
{
    public static string ToText(this SequenceDto x)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"JMdict Entry #{x.Id}");
        if (x.Entry is not null)
        {
            sb.Append(x.Entry.ToText());
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
        if (x.KanjiForms.Count > 0)
        {
            sb.AppendLine("Kanji Forms");
            for (int i = 0; i < x.KanjiForms.Count; i++)
            {
                sb.AppendLine($"\t{i + 1}: {x.KanjiForms[i].ToText()}");
            }
        }
        if (x.Readings.Count > 0)
        {
            sb.AppendLine("Readings");
            for (int i = 0; i < x.Readings.Count; i++)
            {
                sb.AppendLine($"\t{i + 1}: {x.Readings[i].ToText()}");
            }
        }
        if (x.Senses.Count > 0)
        {
            sb.AppendLine("Senses");
            for (int i = 0; i < x.Senses.Count; i++)
            {
                sb.AppendLine($"\t{i + 1}. {x.Senses[i].ToText()}");
            }
        }
        return sb.ToString();
    }

    private static string ToText(this KanjiFormDto x)
    {
        var sb = new StringBuilder(x.Text);
        if (x.Infos.Count > 0)
        {
            sb.Append($"[{string.Join(",", x.Infos)}]");
        }
        if (x.Priorities.Count > 0)
        {
            sb.Append($"[{string.Join(",", x.Priorities)}]");
        }
        return sb.ToString();
    }

    private static string ToText(this ReadingDto x)
    {
        var sb = new StringBuilder(x.Text);
        if (x.Infos.Count > 0)
        {
            sb.Append($"[{string.Join(",", x.Infos)}]");
        }
        if (x.Priorities.Count > 0)
        {
            sb.Append($"[{string.Join(",", x.Priorities)}]");
        }
        if (x.Restrictions.Count > 0)
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
        if (x.PartsOfSpeech.Count > 0)
        {
            sb.Append($"[{string.Join(", ", x.PartsOfSpeech)}]");
        }
        if (x.Miscs.Count > 0)
        {
            sb.Append($"[{string.Join(", ", x.Miscs)}]");
        }
        if (x.Fields.Count > 0)
        {
            sb.Append($"{{{string.Join(", ", x.Fields)}}}");
        }
        if (x.KanjiFormRestrictions.Count > 0)
        {
            sb.Append($"[{string.Join("；", x.KanjiFormRestrictions)}]");
        }
        if (x.ReadingRestrictions.Count > 0)
        {
            sb.Append($"[{string.Join("；", x.ReadingRestrictions)}]");
        }
        if (x.Dialects.Count > 0)
        {
            foreach (var dialect in x.Dialects)
            {
                sb.AppendLine();
                sb.Append($"\t[dialect = {dialect}]");
            }
        }
        if (x.LanguageSources.Count > 0)
        {
            foreach (var langSource in x.LanguageSources)
            {
                sb.AppendLine();
                sb.Append($"\t[langsrc = {langSource.ToText()}]");
            }
        }
        if (x.Notes.Count > 0)
        {
            foreach (var note in x.Notes)
            {
                sb.AppendLine();
                sb.Append($"\t《{note}》");
            }
        }
        if (x.Glosses.Count > 0)
        {
            foreach (var gloss in x.Glosses)
            {
                sb.AppendLine();
                sb.Append($"\t▶ {gloss.ToText()}");
            }
        }
        if (x.CrossReferences.Count > 0)
        {
            foreach (var xref in x.CrossReferences)
            {
                sb.AppendLine();
                sb.Append($"\t{xref.ToText()}");
            }
        }
        return sb.ToString();
    }

    private static string ToText(this GlossDto x)
        => x.TypeName is null
            ? x.Text
            : $"[{x.TypeName}] {x.Text}";

    private static string ToText(this CrossReferenceDto x)
        => $"⇒ {x.TypeName}: {x.Text}";

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
