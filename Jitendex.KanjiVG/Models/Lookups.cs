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

using System.ComponentModel.DataAnnotations.Schema;

namespace Jitendex.KanjiVG.Models;

public interface ILookup
{
    public int Id { get; set; }
    public string Text { get; set; }
}

[Table(nameof(VariantType))]
public class VariantType : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<Entry> Entries { get; } = [];
    public string FileNameFormat()
        => Text == string.Empty ? string.Empty : $"-{Text}";
}

[Table(nameof(Comment))]
public class Comment : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<Entry> Entries { get; } = [];
}

[Table(nameof(ComponentGroupStyle))]
public class ComponentGroupStyle : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<ComponentGroup> Groups { get; } = [];
}

[Table(nameof(StrokeNumberGroupStyle))]
public class StrokeNumberGroupStyle : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<StrokeNumberGroup> Groups { get; } = [];
}

[Table(nameof(ComponentCharacter))]
public class ComponentCharacter : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<Component> Components { get; } = [];
}

[Table(nameof(ComponentOriginal))]
public class ComponentOriginal : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<Component> Components { get; } = [];
}

[Table(nameof(ComponentPosition))]
public class ComponentPosition : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<Component> Components { get; } = [];
}

[Table(nameof(ComponentRadical))]
public class ComponentRadical : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<Component> Components { get; } = [];
}

[Table(nameof(ComponentPhon))]
public class ComponentPhon : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<Component> Components { get; } = [];
}

[Table(nameof(StrokeType))]
public class StrokeType : ILookup
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public List<Stroke> Strokes { get; } = [];
}
