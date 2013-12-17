using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteModifyImplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestModifyPlayerSetNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerSqlLite().Parse(this.TestModifyPlayerSetNameScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue""", new SerializerSqlLite().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue"" AND `Rank` > 10", new SerializerSqlLite().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit).Compile().Compiled.First());
        }
    }
}
