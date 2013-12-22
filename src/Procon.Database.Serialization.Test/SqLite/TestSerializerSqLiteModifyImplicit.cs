using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteModifyImplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue""", new SerializerSqLite().Parse(this.TestModifyPlayerSetNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            Assert.AreEqual(@"UPDATE `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerSqLite().Parse(this.TestModifyPlayerSetNameScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue""", new SerializerSqLite().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            Assert.AreEqual(@"UPDATE `Player` SET `Score` = 50 WHERE `Name` = ""Phogue"" AND `Rank` > 10", new SerializerSqLite().Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit).Compile().Compiled.First());
        }
    }
}
