// Copyright (c) Stephen Kraus
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// This file, CrossReferenceAttributesReader.cs, is part of Jitendex.
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

using System.Xml;
using Jitendex.Import.JMdict.TableRows;
using Microsoft.Extensions.Logging;

namespace Jitendex.Import.JMdict.NGReaders.EntryChildReaders;

internal partial class CrossReferenceAttributesReader(ILogger<CrossReferenceAttributesReader> logger)
{
    public sealed class CrossReferenceAttributes
    {
        public required string TypeName { get; set; }
        public int? Sequence { get; set; }
        public string? Corpus { get; set; }
        public int? SenseNumber { get; set; }
        public string? KanjiForm { get; set; }
        public string? Reading { get; set; }
    }

    public CrossReferenceAttributes Read(XmlReader xmlReader, SenseRow sense)
    {
        var attributes = new CrossReferenceAttributes()
        {
            TypeName = null!
        };

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case XmlNGAttributeName.CrossReferenceType:
                    attributes.TypeName = xmlReader.Value;
                    break;
                case XmlNGAttributeName.CrossReferenceSequence:
                    attributes.Sequence = GetInt(xmlReader, sense);
                    break;
                case XmlNGAttributeName.CrossReferenceCorpus:
                    attributes.Corpus = xmlReader.Value;
                    break;
                case XmlNGAttributeName.CrossReferenceSenseNumber:
                    attributes.SenseNumber = GetInt(xmlReader, sense);
                    break;
                case XmlNGAttributeName.CrossReferenceKanjiForm:
                    attributes.KanjiForm = xmlReader.Value;
                    break;
                case XmlNGAttributeName.CrossReferenceReading:
                    attributes.Reading = xmlReader.Value;
                    break;
                default:
                    LogUnknownAttributeName(xmlReader.Name, xmlReader.Value, sense.EntryId, sense.Number);
                    break;
            }
        }

        xmlReader.MoveToElement();

        if (attributes.TypeName is null)
        {
            LogMissingType(sense.EntryId, sense.Number);
            attributes.TypeName = "see";
        }

        if (attributes.Sequence is null)
            LogMissingSeq(sense.EntryId, sense.Number);

        if (attributes.KanjiForm is null && attributes.Reading is null)
            LogMissingKanjiReading(sense.EntryId, sense.Number);

        return attributes;
    }

    private int? GetInt(XmlReader xmlReader, SenseRow sense)
    {
        var value = xmlReader.Value;
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        else
        {
            LogUnparsableText(xmlReader.Name, value, sense.EntryId, sense.Number);
            return null;
        }
    }

    [LoggerMessage(LogLevel.Warning,
    "Unknown component attribute name `{Name}` with value `{Value}` for xref in entry {EntryId} sense #{SenseNo}")]
    partial void LogUnknownAttributeName(string name, string value, int entryId, int senseNo);

    [LoggerMessage(LogLevel.Warning,
    "Cannot parse value `{Value}` for attribute name `{Name}` for xref in entry {EntryId} sense #{SenseNo}")]
    partial void LogUnparsableText(string name, string value, int entryId, int senseNo);

    [LoggerMessage(LogLevel.Warning,
    "Missing type attribute for xref in entry {EntryId} sense #{SenseNo}")]
    partial void LogMissingType(int entryId, int senseNo);

    [LoggerMessage(LogLevel.Warning,
    "Missing sequence attribute for xref in entry {EntryId} sense #{SenseNo}")]
    partial void LogMissingSeq(int entryId, int senseNo);

    [LoggerMessage(LogLevel.Warning,
    "Missing kanji/reading info for xref in entry {EntryId} sense #{SenseNo}")]
    partial void LogMissingKanjiReading(int entryId, int senseNo);
}
