using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared.Serializers.Sql;

namespace Procon.Database.Shared.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlRemoveExplicit : TestSerializerRemove {

        [Test]
        public override void TestRemoveAllFromPlayer() {
            Assert.AreEqual(@"DELETE FROM `Player`", new SerializerMySql().Parse(this.TestRemoveAllFromPlayerExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Player`.`Name` = ""Phogue""", new SerializerMySql().Parse(this.TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Name` = ""Phogue"" AND `Score` = 10", new SerializerMySql().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"")", new SerializerMySql().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"") AND `Score` > 10 AND `Score` < 20", new SerializerMySql().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE ((`Name` = ""Phogue"" AND `Score` > 50) OR (`Name` = ""Zaeed"" AND `Score` < 50))", new SerializerMySql().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit).Compile().Compiled.First());
        }
    }
}
