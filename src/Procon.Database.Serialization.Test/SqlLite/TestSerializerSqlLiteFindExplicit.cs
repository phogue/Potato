using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteFindExplicit : TestSerializerFind {

        [Test]
        public override void TestSelectAllFromPlayer() {
            Assert.AreEqual(@"SELECT * FROM `Player`", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectDistinctAllFromPlayer() {
            Assert.AreEqual(@"SELECT DISTINCT * FROM `Player`", new SerializerSqlLite().Parse(this.TestSelectDistinctAllFromPlayerExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31F() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Kdr` >= 3.1", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31FExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31F() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Kdr` <= 3.1", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereKdrLessThanEqualTo31FExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Player`.`Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWherePlayerNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT `Score` FROM `Player` WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestSelectScoreFromPlayerWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT `Score`, `Rank` FROM `Player` WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestSelectScoreRankFromPlayerWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Name` = ""Phogue"" AND `Score` = 10", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"")", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"") AND `Score` > 10 AND `Score` < 20", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE ((`Name` = ""Phogue"" AND `Score` > 50) OR (`Name` = ""Zaeed"" AND `Score` < 50))", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByScore() {
            Assert.AreEqual(@"SELECT * FROM `Player` ORDER BY `Score`", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerSortByScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByNameThenScoreDescending() {
            Assert.AreEqual(@"SELECT * FROM `Player` ORDER BY `Name`, `Score` DESC", new SerializerSqlLite().Parse(this.TestSelectAllFromPlayerSortByNameThenScoreDescendingExplicit).Compile().Compiled.First());
        }
    }
}
