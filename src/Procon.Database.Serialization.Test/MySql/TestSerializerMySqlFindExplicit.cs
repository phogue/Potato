using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlFindExplicit : TestSerializerFind {

        [Test]
        public override void TestSelectAllFromPlayer() {
            Assert.AreEqual(@"SELECT * FROM `Player`", new SerializerMySql().Parse(this.TestSelectAllFromPlayerExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectDistinctAllFromPlayer() {
            Assert.AreEqual(@"SELECT DISTINCT * FROM `Player`", new SerializerMySql().Parse(this.TestSelectDistinctAllFromPlayerExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Player`.`Name` = ""Phogue""", new SerializerMySql().Parse(this.TestSelectAllFromPlayerWherePlayerNameEqualsPhogueExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT `Score` FROM `Player` WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestSelectScoreFromPlayerWhereNameEqualsPhogueExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"SELECT `Score`, `Rank` FROM `Player` WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestSelectScoreRankFromPlayerWhereNameEqualsPhogueExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE `Name` = ""Phogue"" AND `Score` = 10", new SerializerMySql().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"")", new SerializerMySql().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"") AND `Score` > 10 AND `Score` < 20", new SerializerMySql().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            Assert.AreEqual(@"SELECT * FROM `Player` WHERE ((`Name` = ""Phogue"" AND `Score` > 50) OR (`Name` = ""Zaeed"" AND `Score` < 50))", new SerializerMySql().Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByScore() {
            Assert.AreEqual(@"SELECT * FROM `Player` ORDER BY `Score`", new SerializerMySql().Parse(this.TestSelectAllFromPlayerSortByScoreExplicit).Compile().Completed);
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByNameThenScoreDescending() {
            Assert.AreEqual(@"SELECT * FROM `Player` ORDER BY `Name`, `Score` DESC", new SerializerMySql().Parse(this.TestSelectAllFromPlayerSortByNameThenScoreDescendingExplicit).Compile().Completed);
        }
    }
}
