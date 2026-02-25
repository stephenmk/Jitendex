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
using Jitendex.Import;
using Jitendex.JMdict.Import.Models;
using Jitendex.JMdict.Import.Parsing.EntryElementReaders.KanjiFormElementReaders;

namespace Jitendex.JMdict.Import.Parsing.EntryElementReaders;

internal partial class KanjiFormReader
(
    ILogger<KanjiFormReader> logger,
    KInfoReader infoReader,
    KPriorityReader priorityReader
) : XmlParentElementReader<Document, KanjiFormElement>(logger)
{
    public async Task ReadAsync(XmlReader xmlReader, Document document, EntryElement entry)
    {
        var kanjiForm = new KanjiFormElement
        {
            EntryId = entry.Id,
            Order = document.KanjiForms.NextOrder(entry.Id),
            Text = null!,
        };

        await ReadToEndAsync(xmlReader, document, kanjiForm, XmlTagName.KanjiForm);

        if (kanjiForm.Text is not null)
        {
            document.KanjiForms.Add(kanjiForm.Key(), kanjiForm);
        }
        else
        {
            LogMissingElement(kanjiForm.EntryId, kanjiForm.Order, XmlTagName.KanjiFormText);
        }
    }

    protected override async Task ReadChildElementAsync(XmlReader xmlReader, Document document, KanjiFormElement kanjiForm)
    {
        switch (xmlReader.Name)
        {
            case XmlTagName.KanjiFormText:
                await ReadKanjiFormText(xmlReader, kanjiForm);
                break;
            case XmlTagName.KanjiFormInfo:
                await infoReader.ReadAsync(xmlReader, document, kanjiForm);
                break;
            case XmlTagName.KanjiFormPriority:
                await priorityReader.ReadAsync(xmlReader, document, kanjiForm);
                break;
            default:
                LogUnexpectedChildElement(xmlReader, XmlTagName.KanjiForm);
                break;
        }
    }

    private async Task ReadKanjiFormText(XmlReader xmlReader, KanjiFormElement kanjiForm)
    {
        if (kanjiForm.Text is not null)
        {
            LogMultipleElements(kanjiForm.EntryId, kanjiForm.Order, XmlTagName.KanjiFormText);
        }

        kanjiForm.Text = await xmlReader.ReadElementContentAsStringAsync();

        if (string.IsNullOrWhiteSpace(kanjiForm.Text))
        {
            LogEmptyTextForm(kanjiForm.EntryId, kanjiForm.Order);
        }
    }

    [LoggerMessage(LogLevel.Error,
    "Entry `{EntryId}` kanji form #{Order} does not contain a <{XmlTagName}> element")]
    partial void LogMissingElement(int entryId, int order, string xmlTagName);

    [LoggerMessage(LogLevel.Warning,
    "Entry `{EntryId}` kanji form #{Order} contains no text")]
    partial void LogEmptyTextForm(int entryId, int order);

    [LoggerMessage(LogLevel.Warning,
    "Entry `{EntryId}` kanji form #{Order} contains multiple <{XmlTagName}> elements")]
    partial void LogMultipleElements(int entryId, int order, string xmlTagName);
}
