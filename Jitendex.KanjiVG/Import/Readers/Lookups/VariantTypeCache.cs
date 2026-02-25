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

using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Jitendex.KanjiVG.Entities;

namespace Jitendex.KanjiVG.Readers.Lookups;

internal partial class VariantTypeCache(ILogger<VariantTypeCache> logger) : LookupCache<VariantType>(logger)
{
    protected override VariantType NewLookup(int id, string text) => new()
    {
        Id = id,
        Text = text,
    };

    protected override ImmutableArray<string> KnownLookups() =>
    [
        "",
        "Kaisho",
        "KaishoVtLst",
        "Hyougai",
        "HzFst",
        "VtLst",
        "HyougaiKaisho",
        "KaishoHzFst",
        "KaishoVt3",
        "Vt6",
        "HzLst",
        "Jinmei",
        "HzFstLeRi",
        "HzFstRiLe",
        "VtFstRiLe",
        "KaishoVt6",
        "MdFst",
        "VtFst",
        "KaishoHzFstRiLe",
        "KaishoHzFstLeRi",
        "KaishoVtFstRiLe",
        "Insatsu",
        "KaishoHzLst",
        "Dg3",
        "DgLst",
        "KaishoVtFst",
        "KaishoVt12",
        "LeFst",
        "KaishoMdFst",
        "JinmeiKaisho",
        "MidFst",
        "HyougaiKaishoVtLst",
        "HzFstTenFst",
        "KaishoHzFstTenFst",
        "KaishoLeFst",
        "KaishoTen3",
        "Ten3",
        "KaishoDg3",
        "KaishoDgLst",
        "KaishoMidFst",
        "KaishoVt3VtLst",
        "KaishoVt4",
        "KaishoVt5",
        "HyougaiHzFst",
        "HyougaiKaishoVt3",
        "HyougaiMdFst",
        "HyougaiVtLst",
        "KaishoVt3Vt3",
        "MdLst",
        "TenLst",
        "HyougaiKaishoDg3",
        "HyougaiKaishoDgLst",
        "HyougaiKaishoHzFst",
        "HyougaiKaishoHzFstLeRi",
        "HyougaiKaishoHzFstRiLe",
        "HyougaiKaishoVt6",
        "HyougaiKaishoVtFstRiLe",
        "HyougaiVt6",
        "HzFstHzFst",
        "HzFstLeRiTenFst",
        "HzFstRiLeTenFst",
        "HzFstVtFst",
        "HzFstVtLst",
        "InsatsuKaisho",
        "KaishoVt3HzFst",
        "KaishoVt3HzFstLeRi",
        "KaishoVt3VtFstRiLe",
        "KaishoVt6HzFstLeRi",
        "KaishoVt6HzFstRiLe",
        "KaishoVt6Vt3",
        "KaishoVt6Vt6",
        "KaishoVt6VtFstRiLe",
        "KaishoVtLstVt3",
        "NoDot",
        "RiLe",
        "TenFst",
        "Vt4",
        "Vt6VtLst",
        "VtFstHzFst",
        "VtFstLeRiTenFst",
        "VtFstRiLeTenFst",
        "VtLstHzFst",
        "VtLstHzFstLeRi",
        "VtLstHzFstRiLe",
        "VtLstHzLst",
        "VtLstRiLe",
        "VtLstVt4",
        "VtLstVt6",
        "VtLstVtLst",
    ];
}
