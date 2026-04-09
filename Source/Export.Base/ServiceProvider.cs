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
using Microsoft.Extensions.DependencyInjection;
using Jitendex.Data.Export;
using Jitendex.Data.Home;
using Jitendex.Data.JMdict;
using Jitendex.Export.Base.Services;
using Jitendex.Export.Base.Tables;
using Jitendex.Export.Base.Tables.TermChildren;

namespace Jitendex.Export.Base;

internal static class ServiceProvider
{
    public static Service GetService() => new ServiceCollection()
        .AddTransient<Service>()

        #region Databases
        .AddDbContext<ExportContext>()
        .AddDbContext<JMdictForkContext>()
        .AddDbContext<HomeContext>()
        #endregion

        #region Services
        .AddTransient<HeadwordService>()
        .AddTransient<TermService>()
        .AddTransient<JMdictTermService>()
        #endregion

        #region Tables
        .AddTransient<HeadwordTable>()
        .AddTransient<HeadwordFuriganaTable>()
        .AddTransient<TermTable>()
        .AddTransient<TermGroupTable>()
        .AddTransient<JMdictEntryTable>()

        .AddTransient<TermRedirectTable>()
        .AddTransient<TermRuleTable>()
        .AddTransient<TermNumberTable>()
        #endregion

        // Logging
        .AddLogging(static builder =>
            builder.AddSimpleConsole(static options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            }))

        // Build and return the Service service.
        .BuildServiceProvider()
        .GetRequiredService<Service>();
}
