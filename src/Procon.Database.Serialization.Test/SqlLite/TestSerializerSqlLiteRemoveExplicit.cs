using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteRemoveExplicit : TestSerializerRemove {

        // [Test]
        public override void TestRemoveAllFromPlayer() {
            Assert.AreEqual(@"DELETE FROM `Player`", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Player`.`Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Name` = ""Phogue"" AND `Score` = 10", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"")", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"") AND `Score` > 10 AND `Score` < 20", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE ((`Name` = ""Phogue"" AND `Score` > 50) OR (`Name` = ""Zaeed"" AND `Score` < 50))", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit).Compile().Compiled.First());
        }
    }
}
