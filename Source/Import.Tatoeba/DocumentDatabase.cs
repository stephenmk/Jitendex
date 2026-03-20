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
using Jitendex.Data.Tatoeba;
using Jitendex.Import.Tatoeba.Models;
using Jitendex.Import.Tatoeba.Tables;

namespace Jitendex.Import.Tatoeba;

internal sealed class DocumentDatabase(ILogger<DocumentDatabase> logger, TatoebaContext context)
    : IDocumentDatabase<DateOnly, Document, DocumentDiff>
{
    private static readonly FileHeaderTable FileHeaderTable = new();
    private static readonly RevisionTable RevisionTable = new();
    private static readonly SequenceTable SequenceTable = new();
    private static readonly ExampleTable ExampleTable = new();
    private static readonly SegmentationTable SegmentationTable = new();
    private static readonly TokenTable TokenTable = new();

    public void EnsureCreated()
        => context.Database.EnsureCreated();

    public DateOnly? GetLastKey()
        => context.FileHeaders
            .OrderByDescending(static x => x.Id)
            .Take(1)
            .Select(static x => (DateOnly?)x.Date)
            .FirstOrDefault();

    public void Initialize(Document document)
    {
        logger.LogInformation("Initializing database with data from {Date:yyyy-MM-dd}", document.ArchiveKey);

        context.RecreateDatabase();

        using var transaction = context.Database.BeginTransaction();

        FileHeaderTable.InsertItem(context, new(document.ArchiveKey));
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertItems(context, document.GetSequences(fileHeaderId));

        ExampleTable.InsertItems(context, document.Examples.Values);
        SegmentationTable.InsertItems(context, document.Segmentations.Values);
        TokenTable.InsertItems(context, document.Tokens.Values);

        transaction.Commit();
        context.ExecuteVacuum();
    }

    public void Update(DocumentDiff diff)
    {
        var sequenceIds = diff.SequenceIds();
        var prioritySequenceIds = diff.PrioritySequenceIds();

        logger.LogInformation("Updating {Count} sequences with data from {Date:yyyy-MM-dd}", sequenceIds.Count, diff.ArchiveKey);

        using var transaction = context.Database.BeginTransaction();

        var aSequences = DtoMapper.LoadSequencesWithoutRevisions(context, sequenceIds);

        context.ExecuteDeferForeignKeysPragma();

        FileHeaderTable.InsertItem(context, new(diff.ArchiveKey));
        var fileHeaderId = (int)context.GetLastInsertRowId();
        SequenceTable.InsertOrIgnoreItems(context, diff.Upserts.GetSequences(fileHeaderId));

        ExampleTable.UpsertItems(context, diff.Upserts.Examples.Values);
        SegmentationTable.UpsertItems(context, diff.Upserts.Segmentations.Values);
        TokenTable.UpsertItems(context, diff.Upserts.Tokens.Values);

        TokenTable.DeleteItems(context, diff.Deletes.Tokens.Values);
        SegmentationTable.DeleteItems(context, diff.Deletes.Segmentations.Values);
        ExampleTable.DeleteItems(context, diff.Deletes.Examples.Values);

        var bSequences = DtoMapper.LoadSequencesWithoutRevisions(context, sequenceIds);

        var sequences = context.Sequences
            .Where(seq => sequenceIds.Contains(seq.Id))
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
                    IsPriority: prioritySequenceIds.Contains(sequence.Id),
                    DiffJson: baDiff
                ));
            }
        }

        RevisionTable.InsertItems(context, revisions);
        transaction.Commit();
    }
}
