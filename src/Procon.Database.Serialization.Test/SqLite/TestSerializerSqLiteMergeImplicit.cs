using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteMergeImplicit : TestSerializerMerge {
        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreUpdateScore() {
            Assert.AreEqual(@"REPLACE INTO `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerSqLite().Parse(this.TestMergeCollectionPlayerSaveNameScoreUpdateScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore() {
            Assert.AreEqual(@"REPLACE INTO `Player` SET `Name` = ""Phogue"", `Score` = 50, `Rank` = 10", new SerializerSqLite().Parse(this.TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScoreImplicit).Compile().Compiled.First());
        }
    }
}
