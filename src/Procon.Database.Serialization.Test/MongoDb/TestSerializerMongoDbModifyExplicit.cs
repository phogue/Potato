using System.Linq;
using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMySqlModifyExplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetNameExplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"{""$set"":{""Name"":""Phogue""}}", serialized.Assignments.First());
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetNameScoreExplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"{""$set"":{""Name"":""Phogue"",""Score"":50.0}}", serialized.Assignments.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"{""$set"":{""Score"":50.0}}", serialized.Assignments.First());
            Assert.AreEqual(@"{""Name"":""Phogue""}", serialized.Conditions.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Explicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"{""$set"":{""Score"":50.0}}", serialized.Assignments.First());
            Assert.AreEqual(@"{""Name"":""Phogue"",""Rank"":{""$gt"":""10""}}", serialized.Conditions.First());
        }
    }
}
