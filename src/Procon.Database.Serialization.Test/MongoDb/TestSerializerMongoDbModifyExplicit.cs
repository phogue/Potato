using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMySqlModifyExplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetNameExplicit).Compile();

            Assert.AreEqual(@"findAndModify", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue""}", serialized.Assignments);
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetNameScoreExplicit).Compile();

            Assert.AreEqual(@"findAndModify", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue"",""Score"":50.0}", serialized.Assignments);
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"findAndModify", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Score"":50.0}", serialized.Assignments);
            Assert.AreEqual(@"{""Name"":""Phogue""}", serialized.Conditions);
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Explicit).Compile();

            Assert.AreEqual(@"findAndModify", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Score"":50.0}", serialized.Assignments);
            Assert.AreEqual(@"{""Name"":""Phogue"",""Rank"":{""$gt"":""10""}}", serialized.Conditions);
        }
    }
}
