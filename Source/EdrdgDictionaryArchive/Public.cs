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

using Microsoft.Extensions.DependencyInjection;
using Jitendex.Import;
using Jitendex.EdrdgDictionaryArchive.Internal;

namespace Jitendex.EdrdgDictionaryArchive;

public sealed class EdrdgArchiveServiceOptions
{
    public DictionaryFile File { get; set; }
    public DirectoryInfo? ArchiveDirectory { get; set; }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEdrdgArchiveService(this IServiceCollection services, Action<EdrdgArchiveServiceOptions> configure)
    {
        var options = new EdrdgArchiveServiceOptions();
        configure(options);

        services.AddTransient<FileBuilder>();
        services.AddTransient<FileArchive>();
        services.AddTransient<FileCache>();

        services.AddTransient<IFileArchive<DateOnly>>(provider =>
        {
            var builder = provider.GetRequiredService<FileBuilder>();
            return new EdrdgArchiveService
            (
                file: options.File,
                archiveDirectory: options.ArchiveDirectory,
                builder: builder
            );
        });

        return services;
    }
}

public enum DictionaryFile : byte
{
    JMdict,
    JMdict_e,
    JMdict_e_examp,
    JMnedict,
    kanjidic2,
    examples,
}
