// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, Lookups.cs, is part of Jitendex.
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

using System.ComponentModel.DataAnnotations.Schema;

namespace Jitendex.Data.KanjiVG.Entities;

public interface ILookup
{
    int Id { get; init; }
    string Text { get; set; }
}

[Table(nameof(VariantType))]
public sealed class VariantType : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<Variant> Variants { get; init; } = [];
    public string FileNameFormat()
        => Text.Length == 0 ? string.Empty : $"-{Text}";
}

[Table(nameof(Comment))]
public sealed class Comment : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<Variant> Variants { get; init; } = [];
}

[Table(nameof(ComponentGroupStyle))]
public sealed class ComponentGroupStyle : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<ComponentGroup> Groups { get; init; } = [];
}

[Table(nameof(StrokeNumberGroupStyle))]
public sealed class StrokeNumberGroupStyle : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<StrokeNumberGroup> Groups { get; init; } = [];
}

[Table(nameof(ComponentCharacter))]
public sealed class ComponentCharacter : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<Component> Components { get; init; } = [];
}

[Table(nameof(ComponentOriginal))]
public sealed class ComponentOriginal : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<Component> Components { get; init; } = [];
}

[Table(nameof(ComponentPosition))]
public sealed class ComponentPosition : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<Component> Components { get; init; } = [];
}

[Table(nameof(ComponentRadical))]
public sealed class ComponentRadical : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<Component> Components { get; init; } = [];
}

[Table(nameof(ComponentPhon))]
public sealed class ComponentPhon : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<Component> Components { get; init; } = [];
}

[Table(nameof(StrokeType))]
public sealed class StrokeType : ILookup
{
    public required int Id { get; init; }
    public required string Text { get; set; }
    public List<Stroke> Strokes { get; init; } = [];
}
