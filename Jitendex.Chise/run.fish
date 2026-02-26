#!/usr/bin/env fish
# Copyright (c) 2025-2026 Stephen Kraus
# SPDX-License-Identifier: AGPL-3.0-or-later

set project_dir (realpath (status dirname))

dotnet run \
    --project "$project_dir" \
    --configuration 'Release' \
    -- "$project_dir"/../Data/chise-ids
