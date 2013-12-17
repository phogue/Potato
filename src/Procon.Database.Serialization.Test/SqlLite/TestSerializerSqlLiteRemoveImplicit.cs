using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteRemoveImplicit : TestSerializerRemove {

        [Test]
        public override void TestRemoveAllFromPlayer() {
            Assert.AreEqual(@"DELETE FROM `Player`", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogue() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Player`.`Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE `Name` = ""Phogue"" AND `Score` = 10", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"")", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE (`Name` = ""Phogue"" OR `Name` = ""Zaeed"") AND `Score` > 10 AND `Score` < 20", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            Assert.AreEqual(@"DELETE FROM `Player` WHERE ((`Name` = ""Phogue"" AND `Score` > 50) OR (`Name` = ""Zaeed"" AND `Score` < 50))", new SerializerSqlLite().Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit).Compile().Compiled.First());
        }
    }
}
