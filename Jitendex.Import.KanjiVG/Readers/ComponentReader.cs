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
using Jitendex.Import.KanjiVG.Models;

namespace Jitendex.Import.KanjiVG.Readers;

internal partial class ComponentReader
(
    ILogger<ComponentReader> logger,
    ComponentAttributesReader attributesReader,
    StrokeReader strokeReader
)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, ComponentGroupElement group)
    {
        var attributes = attributesReader.Read(xmlReader, group);

        var characterId = document.ComponentCharacters.GetNullableLookupId(attributes.Text);
        var originalId = document.ComponentOriginals.GetNullableLookupId(attributes.Original);
        var positionId = document.ComponentPositions.GetNullableLookupId(attributes.Position);
        var radicalId = document.ComponentRadicals.GetNullableLookupId(attributes.Radical);
        var phonId = document.ComponentPhons.GetNullableLookupId(attributes.Phon);

        var component = new ComponentElement
        {
            UnicodeScalarValue = group.UnicodeScalarValue,
            VariantTypeId = group.VariantTypeId,
            Order = document.Components.NextOrder(group.Key()),
            ParentOrder = null,
            IdAttribute = attributes.Id,
            CharacterId = characterId,
            IsVariant = attributes.IsVariant,
            IsPartial = attributes.IsPartial,
            OriginalId = originalId,
            Part = attributes.Part,
            Number = attributes.Number,
            IsTradForm = attributes.IsTradForm,
            IsRadicalForm = attributes.IsRadicalForm,
            PositionId = positionId,
            RadicalId = radicalId,
            PhonId = phonId,
        };

        document.Components.Add(component.Key(), component);

        bool exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, group, component);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(group, text);
                    break;
                case XmlNodeType.EndElement:
                    exit = string.Equals(xmlReader.Name, XmlTagName.Group, StringComparison.Ordinal);
                    break;
            }
        }
    }

    private async Task ReadAsync(XmlReader xmlReader, Document document, ComponentGroupElement group, ComponentElement parent)
    {
        var attributes = attributesReader.Read(xmlReader, group);

        var characterId = document.ComponentCharacters.GetNullableLookupId(attributes.Text);
        var originalId = document.ComponentOriginals.GetNullableLookupId(attributes.Original);
        var positionId = document.ComponentPositions.GetNullableLookupId(attributes.Position);
        var radicalId = document.ComponentRadicals.GetNullableLookupId(attributes.Radical);
        var phonId = document.ComponentPhons.GetNullableLookupId(attributes.Phon);

        var component = new ComponentElement
        {
            UnicodeScalarValue = parent.UnicodeScalarValue,
            VariantTypeId = parent.VariantTypeId,
            Order = document.Components.NextOrder(group.Key()),
            ParentOrder = parent.Order,
            IdAttribute = attributes.Id,
            CharacterId = characterId,
            IsVariant = attributes.IsVariant,
            IsPartial = attributes.IsPartial,
            OriginalId = originalId,
            Part = attributes.Part,
            Number = attributes.Number,
            IsTradForm = attributes.IsTradForm,
            IsRadicalForm = attributes.IsRadicalForm,
            PositionId = positionId,
            RadicalId = radicalId,
            PhonId = phonId,
        };

        document.Components.Add(component.Key(), component);

        bool exit = false;
        while (!exit && await xmlReader.ReadAsync())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    await ReadChildElementAsync(xmlReader, document, group, component);
                    break;
                case XmlNodeType.Text:
                    var text = await xmlReader.GetValueAsync();
                    LogUnexpectedTextNode(group, text);
                    break;
                case XmlNodeType.EndElement:
                    exit = string.Equals(xmlReader.Name, XmlTagName.Group, StringComparison.Ordinal);
                    break;
            }
        }
    }

    private async Task ReadChildElementAsync(XmlReader xmlReader, Document document, ComponentGroupElement group, ComponentElement component)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.Group:
                await ReadAsync(xmlReader, document, group, component);
                break;
            case XmlTagName.Path:
                strokeReader.Read(xmlReader, document, group, component);
                break;
            default:
                LogUnexpectedComponentName(xmlReader.Name, component.IdAttribute);
                break;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Unexpected XML text node `{Text}` in group `{Group}`")]
    partial void LogUnexpectedTextNode(ComponentGroupElement group, string text);

    [LoggerMessage(LogLevel.Warning,
    "Unexpected component child name `{Name}` under parent ID `{ParentId}`")]
    partial void LogUnexpectedComponentName(string name, string parentId);

    [LoggerMessage(LogLevel.Warning,
    "{File}: Component ID `{Actual}` not equal to expected value `{Expected}`")]
    partial void LogWrongId(string file, string actual, string expected);
}
