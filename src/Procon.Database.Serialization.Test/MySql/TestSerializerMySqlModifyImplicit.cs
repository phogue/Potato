using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlModifyImplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestModifyPlayerSetNameImplicit).Compile().Completed);
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerMySql().Parse(this.TestModifyPlayerSetNameScoreImplicit).Compile().Completed);
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit).Compile().Completed);
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue"" AND `Rank` > 10", new SerializerMySql().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit).Compile().Completed);
        }
    }
}
