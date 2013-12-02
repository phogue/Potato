using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlModifyExplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestModifyPlayerSetNameExplicit).Compile().Completed);
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerMySql().Parse(this.TestModifyPlayerSetNameScoreExplicit).Compile().Completed);
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueExplicit).Compile().Completed);
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue"" AND `Rank` > 10", new SerializerMySql().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Explicit).Compile().Completed);
        }
    }
}
