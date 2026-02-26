#!/usr/bin/env fish
# Copyright (c) 2025-2026 Stephen Kraus
# SPDX-License-Identifier: AGPL-3.0-or-later

set project_dir (realpath (status dirname))

set kanjivg_kanji_dir \
    "$project_dir"/../Data/kanjivg/kanji

# set cache_dir 'Jitendex/kanjivg'
# if set -q XDG_CACHE_HOME
#     set kanji_file_dir "$XDG_CACHE_HOME"/"$cache_dir"
# else
#     set kanji_file_dir "$HOME"/'.cache'/"$cache_dir"
# end

# # Use the current KanjiVG commit ID as the name of the cached archive.
# set kanji_file_name \
#     (git -C (dirname "$kanjivg_kanji_dir") rev-parse --verify --short HEAD).tar.br
# or return 1

# set kanji_file_path \
#     "$kanji_file_dir"/"$kanji_file_name"

# if not test -e "$kanji_file_path"
#     if test -d "$kanji_file_dir"
#         rm -r "$kanji_file_dir"
#     end
#     mkdir -p "$kanji_file_dir"

#     cd "$kanjivg_kanji_dir"
#     tar -c *.svg | brotli -4 > "$kanji_file_path"; or return 1
#     cd "$project_dir"
# end

time dotnet run \
    --project "$project_dir" \
    --configuration 'Release' \
    -- "$kanjivg_kanji_dir"
