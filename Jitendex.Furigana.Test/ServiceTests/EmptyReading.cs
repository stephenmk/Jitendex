/*
Copyright (c) 2025 Stephen Kraus
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

namespace Jitendex.Furigana.Test.ServiceTests;

[TestClass]
public class EmptyReading : ServiceTest
{
    private static readonly IEnumerable<JapaneseCharacter> _nonKanji = ResourceMethods.VocabKanji(new()
    {
        ["・"] = ["・", ""],
        ["、"] = ["、", ""],
        ["？"] = ["？", ""],
    });

    private static readonly SolvableData _data =
    [
        (
            "コード・トップ・レベル・ドメイン",
            "コードトップレベルドメイン",
            "コード[・|]トップ[・|]レベル[・|]ドメイン"
        ),
        (
            "高フルクトース・コーン・シロップ",
            "こうフルクトースコーンシロップ",
            "[高|こう]フルクトース[・|]コーン[・|]シロップ"
        ),
        (
            "ブルータス、お前もか？",
            "ブルータスおまえもか",
            "ブルータス[、|]お[前|まえ]もか[？|]"
        ),
    ];

    private static readonly UnsolvableData _unsolvableData = _data
        .Select(static x => (x.KanjiFormText, x.ReadingText));

    [TestMethod]
    public void TestSolvable()
    {
        var solver = FuriganaSolverProvider.GetFuriganaSolver();
        solver.AddCharacters(_nonKanji);
        TestSolvable(solver, _data);
    }

    [TestMethod]
    public void TestUnsolvable()
    {
        TestUnsolvable(DefaultService, _unsolvableData);
    }
}
