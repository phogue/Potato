using System.Linq;
using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbRemoveImplicit : TestSerializerRemove {

        [Test]
        public override void TestRemoveAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerImplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
        }
        
        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueImplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue""}", serialized.Conditions);
        }

        [Test]
        public override void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueImplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue""}", serialized.Conditions);
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue"",""Score"":""10""}", serialized.Conditions);
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}]}", serialized.Conditions);
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit).Compile();

            Assert.AreEqual(@"remove", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}],""Score"":{""$gt"":""10"",""$lt"":""20""}}", serialized.Conditions);
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit).Compile();

            Assert.AreEqual(@"remove", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""$or"":[{""Name"":""Phogue"",""Score"":{""$gt"":""50""}},{""Name"":""Zaeed"",""Score"":{""$lt"":""50""}}]}", serialized.Conditions);
        }
    }
}
