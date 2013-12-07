using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteMergeExplicit : TestSerializerMerge {
        // [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreUpdateScore() {
            Assert.AreEqual(@"REPLACE INTO `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerSqlLite().Parse(this.TestMergeCollectionPlayerSaveNameScoreUpdateScoreExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore() {
            Assert.AreEqual(@"REPLACE INTO `Player` SET `Name` = ""Phogue"", `Score` = 50, `Rank` = 10", new SerializerSqlLite().Parse(this.TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScoreExplicit).Compile().Compiled.First());
        }
    }
}
