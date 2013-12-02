using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlMergeImplicit : TestSerializerMerge {
        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreUpdateScore() {
            Assert.AreEqual(@"INSERT INTO `Player` SET `Name` = ""Phogue"", `Score` = 50 ON DUPLICATE KEY UPDATE `Score` = 50", new SerializerMySql().Parse(this.TestMergeCollectionPlayerSaveNameScoreUpdateScoreImplicit).Compile().Completed);
        }

        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore() {
            Assert.AreEqual(@"INSERT INTO `Player` SET `Name` = ""Phogue"", `Score` = 50, `Rank` = 10 ON DUPLICATE KEY UPDATE `Score` = 50, `Rank` = 10", new SerializerMySql().Parse(this.TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScoreImplicit).Compile().Completed);
        }
    }
}
