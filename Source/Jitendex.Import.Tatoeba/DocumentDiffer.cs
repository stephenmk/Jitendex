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

using Jitendex.Import.Tatoeba.Models;

namespace Jitendex.Import.Tatoeba;

internal sealed class DocumentDiffer : DocumentDiffer<DateOnly, Document, DocumentDiff>
{
    public override DocumentDiff Diff(Document docA, Document docB)
    {
        var diff = new DocumentDiff
        {
            ArchiveKey = docB.ArchiveKey,
            Inserts = new Document(0) { ArchiveKey = docB.ArchiveKey },
            Updates = new Document(0) { ArchiveKey = docB.ArchiveKey },
            Deletes = new Document(0) { ArchiveKey = docB.ArchiveKey },
        };

        DiffDictionaryProperties<int, ExampleElement>(diff, docA, docB, propertyName: nameof(Document.Examples));
        DiffDictionaryProperties<(int, int), SegmentationElement>(diff, docA, docB, propertyName: nameof(Document.Segmentations));
        DiffDictionaryProperties<(int, int, int), TokenElement>(diff, docA, docB, propertyName: nameof(Document.Tokens));

        return diff;
    }
}
