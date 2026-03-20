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

using Microsoft.EntityFrameworkCore.Storage;
using Dependencies = Microsoft.EntityFrameworkCore.Storage.RelationalCommandBuilderDependencies;
using Builder = Microsoft.EntityFrameworkCore.Storage.RelationalCommandBuilder;

namespace Jitendex.Data.EntityFrameworkCore;

internal sealed class SqliteCommandBuilder(Dependencies dependencies) : Builder(dependencies)
{
    public override IRelationalCommand Build()
        => new RelationalCommand
        (
            dependencies: Dependencies,
            commandText: ToString().WithoutRowId(),
            logCommandText: string.Empty,
            parameters: Parameters
        );
}
