#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Linq;
using NUnit.Framework;
using Potato.Database.Shared.Serializers.Sql;

namespace Potato.Database.Shared.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlFindExplicit : TestSerializerFind {

        [Test]
        public override void TestSelectAllFromPlayer() {
            Assert.AreEqual(@"SELECT * FROM `Player`", new SerializerMySql().Parse(TestSelectAllFromPlayerExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectDistinctAllFromPlayer() {
            Assert.AreEqual(@"SELECT DISTINCT * FROM `Player`", new SerializerMySql().Parse(TestSelectDistinctAllFromPlayerExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31D() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Kdr` >= 3.1", new SerializerMySql().Parse(TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31DExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31D() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Kdr` <= 3.1", new SerializerMySql().Parse(TestSelectAllFromPlayerWhereKdrLessThanEqualTo31DExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Player`.`Name` = ""Phogue""", new SerializerMySql().Parse(TestSelectAllFromPlayerWherePlayerNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT `Score` FROM `Player` WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(TestSelectScoreFromPlayerWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT `Score`, `Rank` FROM `Player` WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(TestSelectScoreRankFromPlayerWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Name` = ""Phogue"" AND `Score` = 10", new SerializerMySql().Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"")", new SerializerMySql().Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"") AND `Score` > 10 AND `Score` < 20", new SerializerMySql().Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE ((`Name` = ""Phogue"" AND `Score` > 50) OR (`Name` = ""Zaeed"" AND `Score` < 50))", new SerializerMySql().Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByScore() {
            Assert.AreEqual(@"SELECT * FROM `Player` ORDER BY `Score`", new SerializerMySql().Parse(TestSelectAllFromPlayerSortByScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByNameThenScoreDescending() {
            Assert.AreEqual(@"SELECT * FROM `Player` ORDER BY `Name`, `Score` DESC", new SerializerMySql().Parse(TestSelectAllFromPlayerSortByNameThenScoreDescendingExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerLimit1() {
            Assert.AreEqual(@"SELECT * FROM `Player` LIMIT 1", new SerializerMySql().Parse(TestSelectAllFromPlayerLimit1Explicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerLimit1Skip2() {
            Assert.AreEqual(@"SELECT * FROM `Player` LIMIT 1 OFFSET 2", new SerializerMySql().Parse(TestSelectAllFromPlayerLimit1Skip2Explicit).Compile().Compiled.First());
        }
    }
}
