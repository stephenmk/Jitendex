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

using System.Xml;
using Microsoft.Extensions.Logging;
using Jitendex.KanjiVG.Models;
using Jitendex.KanjiVG.Readers.Lookups;

namespace Jitendex.KanjiVG.Readers;

internal partial class ComponentReader
(
    ILogger<ComponentReader> logger,
    ComponentAttributesReader attributesReader,
    StrokeReader strokeReader,
    ComponentCharacterCache characterCache,
    ComponentOriginalCache originalCache,
    ComponentPositionCache positionCache,
    ComponentRadicalCache radicalCache,
    ComponentPhonCache phonCache
)
{
    public async Task ReadAsync(XmlReader xmlReader, ComponentGroup group)
    {
        var attributes = attributesReader.Read(xmlReader, group);
        var character = characterCache.Get(attributes.Text);
        var original = originalCache.Get(attributes.Original);
        var position = positionCache.Get(attributes.Position);
        var radical = radicalCache.Get(attributes.Radical);
        var phon = phonCache.Get(attributes.Phon);

        var component = new Component
        {
            UnicodeScalarValue = group.Entry.UnicodeScalarValue,
            VariantTypeId = group.Entry.VariantTypeId,
            GlobalOrder = group.ComponentCount() + 1,
            ParentGlobalOrder = null,
            LocalOrder = group.Components.Count + 1,
            CharacterId = character.Id,
            IsVariant = attributes.IsVariant,
            IsPartial = attributes.IsPartial,
            OriginalId = original.Id,
            Part = attributes.Part,
            Number = attributes.Number,
            IsTradForm = attributes.IsTradForm,
            IsRadicalForm = attributes.IsRadicalForm,
            PositionId = position.Id,
            RadicalId = radical.Id,
            PhonId = phon.Id,
            Group = group,
            Parent = null,
            Character = character,
            Original = original,
            Position = position,
            Radical = radical,
            Phon = phon,
        };

        group.Components.Add(component);
        character.Components.Add(component);
        original.Components.Add(component);
        position.Components.Add(component);
        radical.Components.Add(component);
        phon.Components.Add(component);

        if (!string.Equals(attributes.Id, component.XmlIdAttribute(), StringComparison.Ordinal))
        {
            LogWrongId(component.Group.Entry.FileName(), attributes.Id, component.XmlIdAttribute());
        }

        bool exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, component);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(group.Entry.FileName(), text);
                    break;
                case XmlNodeType.EndElement:
                    exit = xmlReader.Name == "g";
                    break;
            }
        }
    }

    private async Task ReadAsync(XmlReader xmlReader, Component parent)
    {
        var attributes = attributesReader.Read(xmlReader, parent.Group);
        var character = characterCache.Get(attributes.Text);
        var original = originalCache.Get(attributes.Original);
        var position = positionCache.Get(attributes.Position);
        var radical = radicalCache.Get(attributes.Radical);
        var phon = phonCache.Get(attributes.Phon);

        var component = new Component
        {
            UnicodeScalarValue = parent.UnicodeScalarValue,
            VariantTypeId = parent.VariantTypeId,
            GlobalOrder = parent.Group.ComponentCount() + 1,
            ParentGlobalOrder = parent.GlobalOrder,
            LocalOrder = parent.Children.Count + 1,
            CharacterId = character.Id,
            IsVariant = attributes.IsVariant,
            IsPartial = attributes.IsPartial,
            OriginalId = original.Id,
            Part = attributes.Part,
            Number = attributes.Number,
            IsTradForm = attributes.IsTradForm,
            IsRadicalForm = attributes.IsRadicalForm,
            PositionId = position.Id,
            RadicalId = radical.Id,
            PhonId = phon.Id,
            Group = parent.Group,
            Parent = parent,
            Character = character,
            Original = original,
            Position = position,
            Radical = radical,
            Phon = phon,
        };

        parent.Children.Add(component);
        character.Components.Add(component);
        original.Components.Add(component);
        position.Components.Add(component);
        radical.Components.Add(component);
        phon.Components.Add(component);

        if (!string.Equals(attributes.Id, component.XmlIdAttribute(), StringComparison.Ordinal))
        {
            LogWrongId(component.Group.Entry.FileName(), attributes.Id, component.XmlIdAttribute());
        }

        bool exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, component);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(parent.Group.Entry.FileName(), text);
                    break;
                case XmlNodeType.EndElement:
                    exit = xmlReader.Name == "g";
                    break;
            }
        }
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Component component)
    {
        switch (xmlReader.Name)
        {
            case "g":
                await ReadAsync(xmlReader, component);
                break;
            case "path":
                strokeReader.Read(xmlReader, component);
                break;
            default:
                LogUnexpectedComponentName(xmlReader.Name, component.Group.Entry.FileName(), component.XmlIdAttribute());
                break;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "{File}: Unexpected XML text node `{Text}`")]
    partial void LogUnexpectedTextNode(string file, string text);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected component child name `{Name}` in file `{FileName}`, parent ID `{ParentId}`")]
    partial void LogUnexpectedComponentName(string name, string fileName, string parentId);

    [LoggerMessage(LogLevel.Warning,
    "{File}: Component ID `{Actual}` not equal to expected value `{Expected}`")]
    partial void LogWrongId(string file, string actual, string expected);
}
