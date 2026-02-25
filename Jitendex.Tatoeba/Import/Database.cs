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

using Microsoft.Extensions.Logging;
using Jitendex.MinimalJsonDiff;
using Jitendex.Tatoeba.Import.Models;
using Jitendex.Tatoeba.Import.Tables;

namespace Jitendex.Tatoeba.Import;

internal sealed class Database(ILogger<Database> logger, TatoebaContext context)
{
    private static readonly FileHeaderTable FileHeaderTable = new();
    private static readonly RevisionTable RevisionTable = new();
    private static readonly SequenceTable SequenceTable = new();
    private static readonly ExampleTable ExampleTable = new();
    private static readonly TranslationTable TranslationTable = new();
    private static readonly SegmentationTable SegmentationTable = new();
    private static readonly TokenTable TokenTable = new();

    public void Initialize(Document document)
    {
        logger.LogInformation("Initializing database with data from {Date:yyyy-MM-dd}", document.Header.Date);

        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        FileHeaderTable.InsertItem(context, document.Header);
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertItems(context, document.GetSequences(fileHeaderId));

        ExampleTable.InsertItems(context, document.Examples.Values);
        TranslationTable.InsertItems(context, document.Translations.Values);
        SegmentationTable.InsertItems(context, document.Segmentations.Values);
        TokenTable.InsertItems(context, document.Tokens.Values);

        transaction.Commit();
    }

    public void Update(DocumentDiff diff)
    {
        logger.LogInformation("Updating {Count} sequences with data from {Date:yyyy-MM-dd}", diff.SequenceIds.Count, diff.Date);

        using var transaction = context.Database.BeginTransaction();

        var aSequences = DtoMapper.LoadSequencesWithoutRevisions(context, diff.SequenceIds);

        context.ExecuteDeferForeignKeysPragma();

        FileHeaderTable.InsertItem(context, diff.InsertDocument.Header);
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertOrIgnoreItems(context, diff.InsertDocument.GetSequences(fileHeaderId));

        ExampleTable.InsertItems(context, diff.InsertDocument.Examples.Values);
        TranslationTable.InsertItems(context, diff.InsertDocument.Translations.Values);
        SegmentationTable.InsertItems(context, diff.InsertDocument.Segmentations.Values);
        TokenTable.InsertItems(context, diff.InsertDocument.Tokens.Values);

        ExampleTable.UpdateItems(context, diff.UpdateDocument.Examples.Values);
        TranslationTable.UpdateItems(context, diff.UpdateDocument.Translations.Values);
        SegmentationTable.UpdateItems(context, diff.UpdateDocument.Segmentations.Values);
        TokenTable.UpdateItems(context, diff.UpdateDocument.Tokens.Values);

        TokenTable.DeleteItems(context, diff.DeleteDocument.Tokens.Values);
        SegmentationTable.DeleteItems(context, diff.DeleteDocument.Segmentations.Values);
        TranslationTable.DeleteItems(context, diff.DeleteDocument.Translations.Values);
        ExampleTable.DeleteItems(context, diff.DeleteDocument.Examples.Values);

        var bSequences = DtoMapper.LoadSequencesWithoutRevisions(context, diff.SequenceIds);

        var sequences = context.Sequences
            .Where(seq => diff.SequenceIds.Contains(seq.Id))
            .Select(seq => new
            {
                seq.Id,
                RevisionCount = seq.Revisions.Count,
            });

        var revisions = new List<DocumentRevision>(aSequences.Count);

        foreach (var sequence in sequences)
        {
            if (aSequences.TryGetValue(sequence.Id, out var aSequence))
            {
                var bSequence = bSequences[sequence.Id];
                var baDiff = JsonDiffer.Diff(a: bSequence, b: aSequence);
                revisions.Add(new
                (
                    SequenceId: sequence.Id,
                    Number: sequence.RevisionCount,
                    FileHeaderId: fileHeaderId,
                    IsPriority: diff.PrioritySequenceIds.Contains(sequence.Id),
                    DiffJson: baDiff
                ));
            }
        }

        RevisionTable.InsertItems(context, revisions);
        transaction.Commit();
    }
}
