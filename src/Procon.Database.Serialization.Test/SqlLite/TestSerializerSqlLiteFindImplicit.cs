using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteFindImplicit : TestSerializerFind {

        [Test]
        public override void TestSelectAllFromPlayer() {
            Assert.AreEqual(@"SELECT * FROM `Player`", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectDistinctAllFromPlayer() {
            Assert.AreEqual(@"SELECT DISTINCT * FROM `Player`", new SerializerSqlLite().Parse(this.TestSelectDistinctAllFromPlayerImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31F() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Kdr` >= 3.1", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31FImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31F() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Kdr` <= 3.1", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereKdrLessThanEqualTo31FImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Player`.`Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWherePlayerNameEqualsPhogueImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT `Score` FROM `Player` WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestSelectScoreFromPlayerWhereNameEqualsPhogueImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT `Score`, `Rank` FROM `Player` WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestSelectScoreRankFromPlayerWhereNameEqualsPhogueImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Name` = ""Phogue"" AND `Score` = 10", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"")", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"") AND `Score` > 10 AND `Score` < 20", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE ((`Name` = ""Phogue"" AND `Score` > 50) OR (`Name` = ""Zaeed"" AND `Score` < 50))", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByScore() {
            Assert.AreEqual(@"SELECT * FROM `Player` ORDER BY `Score`", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerSortByScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByNameThenScoreDescending() {
            Assert.AreEqual(@"SELECT * FROM `Player` ORDER BY `Name`, `Score` DESC", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerSortByNameThenScoreDescendingImplicit).Compile().Compiled.First());
        }
    }
}
