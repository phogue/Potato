using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteModifyExplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue""", new SerializerSqLite().Parse(this.TestModifyPlayerSetNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerSqLite().Parse(this.TestModifyPlayerSetNameScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue""", new SerializerSqLite().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue"" AND `Rank` > 10", new SerializerSqLite().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Explicit).Compile().Compiled.First());
        }
    }
}
