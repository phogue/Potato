using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.NoSql;
using Procon.Database.Shared;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMySqlModifyImplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetNameImplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue""}}]", serialized.Assignments.First());
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetNameScoreImplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0}}]", serialized.Assignments.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Score"":50.0}}]", serialized.Assignments.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Score"":50.0}}]", serialized.Assignments.First());
            Assert.AreEqual(@"[{""Name"":""Phogue"",""Rank"":{""$gt"":""10""}}]", serialized.Conditions.First());
        }
    }
}
